using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using Cloo;
using System.IO;
using System.Threading.Tasks;

namespace rayTracer
{
    public delegate void workerDelegate();

    public partial class rayTracerUI : Form
    {
        private const int screenWidth = 640;
        private const int screenHeight = 480;
        private const double aspectRatio = (double)screenWidth / (double)screenHeight;
        private const double ambientlight = 0.2;
        private const double accuracy = 0.000001;
        private const double aathreshold = 0.1;
        private int aaDepth = 1;
        private int threadCount = 20;

        DirectBitmap bmp = new DirectBitmap(screenWidth, screenHeight);


        // pick first platform
        ComputePlatform platform;

        // create context with all gpu devices
        ComputeContext context;

        // create a command queue with first gpu found
        ComputeCommandQueue queue;

        // load opencl source
        StreamReader streamReader;

        // create program with opencl source
        ComputeProgram program;

        // load chosen kernel from program
        ComputeKernel kernel;

        private int winningObjectIndex(List<double> intersections)
        {
            //returns the index of winning intersection
            int minimumIndex = -1;

            //prevent useless calculations
            if(intersections.Count() == 0)
            {
                return -1;
            }else if (intersections.Count() == 1)
            {
                if (intersections.ElementAt(0) > 0)
                {
                    //if that intersection is greater than zero then its first intersection
                    return 0;
                }else
                {
                    //otherwise the only intersection value is negative, missed
                    return -1;
                }
            }else
            {
                //otherwise there is more than one and we have to find the max
                double maxValue = 0;
                for (int index = 0; index < intersections.Count(); index++)
                {
                    if (maxValue < intersections.ElementAt(index))
                    {
                        maxValue = intersections.ElementAt(index);
                    }
                }
                //then starting from max, find the min positive value
                if(maxValue > 0)
                {
                    //we only want positive intersections
                    for (int index = 0; index < intersections.Count(); index++)
                    {
                        if(intersections.ElementAt(index) > 0 && intersections.ElementAt(index) <= maxValue)
                        {
                            maxValue = intersections.ElementAt(index);
                            minimumIndex = index;
                        }
                    }
                    return minimumIndex;
                }else
                {
                    //all intersections negative
                    return -1;
                }
            }
        }

        private Color getColorAt(Vector intersectionPosition, Vector intersectionRayDirection, List<Object> sceneObjects, List<Source> lightSources, int winnerIndex, double accurary, double ambientlight, int bounces)
        {
            Color winningObjectColor = sceneObjects.ElementAt(winnerIndex).color;
            Vector winningObjectNormal = sceneObjects.ElementAt(winnerIndex).getNormalAt(intersectionPosition);

            Color finalColor = winningObjectColor.colorScalar(ambientlight);

            if(winningObjectColor.special == 2)
            {
                int square = (int)Math.Floor(intersectionPosition.x) + (int)Math.Floor(intersectionPosition.z);

                if((square % 2) == 0)
                {
                    winningObjectColor.setRed(0);
                    winningObjectColor.setGreen(0);
                    winningObjectColor.setBlue(0);
                }
                else
                {
                    winningObjectColor.setRed(1);
                    winningObjectColor.setGreen(1);
                    winningObjectColor.setBlue(1);
                }
            }

            if(winningObjectColor.special > 0 && winningObjectColor.special <= 1)
            {
                //reflection from objects that have specular value
                double dot1 = winningObjectNormal.dotProduct(intersectionRayDirection.negate());
                Vector scalar1 = winningObjectNormal.multiply(dot1);
                Vector add1 = scalar1.add(intersectionRayDirection);
                Vector scalar2 = add1.multiply(2);
                Vector add2 = intersectionRayDirection.negate().add(scalar2);
                Vector reflectionDirection = add2.normalize();

                Ray reflectionRay = new Ray(intersectionPosition, reflectionDirection);

                //determine what ray intersects first
                List<double> reflectionIntersections = new List<double>();

                foreach(Object reflection in sceneObjects)
                {
                    reflectionIntersections.Add(reflection.findIntersection(reflectionRay));
                }


                int winnerIndexReflection = winningObjectIndex(reflectionIntersections);

                if(winnerIndexReflection != -1)
                {
                    //reflection ray missed everything else
                    //recursive number to keep track of light bounces
                    int numberOfBounces = bounces;
                    if(numberOfBounces < 2)
                    {
                        numberOfBounces++;
                        //determine the position and direction at the point of intersection with the ray
                        //the ray only affects the color if it reflects off something
                        Vector reflectionIntersectionPosition = intersectionPosition.add(reflectionDirection.multiply(reflectionIntersections.ElementAt(winnerIndexReflection)));
                        Vector reflectionIntersectionRayDirection = reflectionDirection;
                        Color reflectionIntersectionColor = getColorAt(reflectionIntersectionPosition, reflectionIntersectionRayDirection, sceneObjects, lightSources, winnerIndexReflection, accuracy, ambientlight, numberOfBounces);

                        finalColor = finalColor.colorAdd(reflectionIntersectionColor.colorScalar(winningObjectColor.special));
                    }
                }
            }

            foreach (Source light in lightSources)
            {
                Vector lightDirection = light.position.add(intersectionPosition.negate()).normalize();

                double cosine_angle = winningObjectNormal.dotProduct(lightDirection);

                if(cosine_angle > 0)
                {
                    //test for shadows
                    bool shadowed = false;

                    Vector distanceToLight = light.position.add(intersectionPosition.negate()).normalize();
                    double distanceToLightMagnitude = distanceToLight.magnitude();

                    Ray shadowRay = new Ray(intersectionPosition, light.position.add(intersectionPosition.negate()).normalize());

                    List<double> secondaryIntersections = new List<double>();

                    for(int objectIndex = 0; objectIndex < sceneObjects.Count() && shadowed == false; objectIndex++)
                    {
                        secondaryIntersections.Add(sceneObjects.ElementAt(objectIndex).findIntersection(shadowRay));
                    }

                    for(int c = 0; c < secondaryIntersections.Count(); c++)
                    {
                        if(secondaryIntersections.ElementAt(c) > accuracy)
                        {
                            if(secondaryIntersections.ElementAt(c) <= distanceToLightMagnitude)
                            {
                                shadowed = true;
                            }
                            break;
                        }
                    }
                    if(shadowed == false)
                    {
                        finalColor = finalColor.colorAdd(winningObjectColor.colorMultiply(light.color).colorScalar(cosine_angle));

                        if(winningObjectColor.special > 0 && winningObjectColor.special <= 1)
                        {
                            //special 0 to 1
                            double dot1 = winningObjectNormal.dotProduct(intersectionRayDirection.negate());
                            Vector scalar1 = winningObjectNormal.multiply(dot1);
                            Vector add1 = scalar1.add(intersectionRayDirection);
                            Vector scalar2 = add1.multiply(2);
                            Vector add2 = intersectionRayDirection.negate().add(scalar2);
                            Vector reflectionDirection = add2.normalize();

                            double specular = reflectionDirection.dotProduct(lightDirection);
                            if(specular > 0)
                            {
                                specular = Math.Pow(specular, 10);
                                finalColor = finalColor.colorAdd(light.color.colorScalar(specular * winningObjectColor.special));
                            }
                        }
                    }
                }
            }
            return finalColor.clip();
        }

        private async Task<Tuple<int, bool>> doWork(int section)
        {
            //axis vectors
            Vector O = new Vector(0, 0, 0);
            Vector X = new Vector(1, 0, 0);
            Vector Y = new Vector(0, 1, 0);
            Vector Z = new Vector(0, 0, 1);

            //camera properties
            Vector cameraTarget = new Vector(0, 0, 0);
            Vector cameraPosition = new Vector(5, 1.5, -4);
            Vector targetDifference = new Vector(cameraPosition.x - cameraTarget.x, cameraPosition.y - cameraTarget.y, cameraPosition.z - cameraTarget.z);

            Vector cameraDirection = targetDifference.negate().normalize();

            //cross product between camera direction and Y axis
            Vector cameraRight = Y.crossProduct(cameraDirection).normalize();

            //cross product between camera direction and cameraRight axis
            Vector cameraDown = cameraRight.crossProduct(cameraDirection);

            //camera
            Camera cam = new Camera(cameraPosition, cameraDirection, cameraRight, cameraDown);

            //scene light
            Color whiteLight = new Color(1.0, 1.0, 1.0, 0);
            Color green = new Color(0.5, 1.0, 0.5, 0.3);
            Color red = new Color(1.0, 0.5, 0.5, 0.8);
            Color blue = new Color(0.5, 0.5, 1.0, 0.3);
            Color grey = new Color(0.5, 0.5, 0.5, 0);
            Color maroon = new Color(0.5, 0.25, 0.25, 2);
            Color black = new Color(0.0, 0.0, 0.0, 0);

            Vector lightPosition = new Vector(-7, 10, -10);
            Light sceneLight = new Light(lightPosition, whiteLight);

            List<Source> lightSources = new List<Source>();
            lightSources.Add((Source)sceneLight);

            //objects
            Sphere sphere1 = new Sphere(O, 1.0, green);
            Sphere sphere2 = new Sphere(new Vector(0.0, 2.0, 0.0), 1.0, red);
            Sphere sphere3 = new Sphere(new Vector(0.0, 0.0, 4.0), 1.0, blue);
            Plane plane = new Plane(Y, -1, maroon);

            List<Object> sceneObjects = new List<Object>();
            sceneObjects.Add((Object)sphere1);
            sceneObjects.Add((Object)sphere2);
            sceneObjects.Add((Object)sphere3);
            sceneObjects.Add((Object)plane);

            double xAmnt, yAmnt;
            int currentPixel, currentDrawnPixel, aaIndex;

            int startRender = ((screenHeight / threadCount) * (section - 1));
            int endRender = ((screenHeight / threadCount) * (section));

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = startRender; y < endRender; y++)
                {
                    currentPixel = x + y * screenWidth;
                    currentDrawnPixel = x + ((screenHeight - 1) - y) * screenWidth;

                    //blank pixel
                    double[] tempRed = new double[aaDepth * aaDepth];
                    double[] tempGreen = new double[aaDepth * aaDepth];
                    double[] tempBlue = new double[aaDepth * aaDepth];


                    for(int aax = 0; aax < aaDepth; aax++)
                    {
                        for(int aay = 0; aay < aaDepth; aay++)
                        {

                            aaIndex = aay * aaDepth + aax;

                            if (aaDepth == 1)
                            {
                                if (screenWidth > screenHeight)
                                {
                                    xAmnt = ((x + 0.5) / screenWidth) * aspectRatio - (((screenWidth - screenHeight) / (double)screenHeight) / 2);
                                    yAmnt = ((screenHeight - y) + 0.5) / screenHeight;
                                }
                                else if (screenHeight > screenWidth)
                                {
                                    xAmnt = (x + 0.5) / screenWidth;
                                    yAmnt = (((screenHeight - y) + 0.5) / screenHeight) / aspectRatio - (((screenHeight - screenWidth) / (double)screenWidth) / 2);
                                }
                                else
                                {
                                    xAmnt = (x + 0.5) / screenWidth;
                                    yAmnt = ((screenHeight - y) + 0.5) / screenHeight;
                                }
                            }
                            else
                            {
                                if (screenWidth > screenHeight)
                                {
                                    xAmnt = ((x + (double)aax / ((double)aaDepth - 1)) / screenWidth) * aspectRatio - (((screenWidth - screenHeight) / (double)screenHeight) / 2);
                                    yAmnt = ((screenHeight - y) + (double)aax / ((double)aaDepth - 1)) / screenHeight;
                                }
                                else if (screenHeight > screenWidth)
                                {
                                    xAmnt = (x + (double)aax / ((double)aaDepth - 1)) / screenWidth;
                                    yAmnt = (((screenHeight - y) + (double)aax / ((double)aaDepth - 1)) / screenHeight) / aspectRatio - (((screenHeight - screenWidth) / (double)screenWidth) / 2);
                                }
                                else
                                {
                                    xAmnt = (x + (double)aax / ((double)aaDepth - 1)) / screenWidth;
                                    yAmnt = ((screenHeight - y) + (double)aax / ((double)aaDepth - 1)) / screenHeight;
                                }
                            }
                            
                            Vector cameraRayOrigin = cam.position;
                            Vector cameraRayDirection = cameraDirection.add(cameraRight.multiply(xAmnt - 0.5).add(cameraDown.multiply(yAmnt - 0.5))).normalize();

                            Ray cameraRay = new Ray(cameraRayOrigin, cameraRayDirection);

                            List<double> intersections = new List<double>();

                            foreach(Object obj in sceneObjects){
                                intersections.Add(obj.findIntersection(cameraRay));
                            }
                            
                            
                            //now we have the find the winning object in the list of intersections
                            int winnerIndex = winningObjectIndex(intersections);

                            if(winnerIndex == -1)
                            {
                                //missed
                                tempRed[aaIndex] = 0;
                                tempGreen[aaIndex] = 0;
                                tempBlue[aaIndex] = 0;

                                bmp.Bits[currentDrawnPixel] = System.Drawing.Color.FromArgb(0, 0, 0).ToArgb();
                            }
                            else
                            {
                                //index corresponds to a color
                                if (intersections.ElementAt(winnerIndex) > accuracy)
                                {
                                    //determine the position and direction vectors at the point of intersection

                                    Vector intersectionPosition = cameraRayOrigin.add(cameraRayDirection.multiply(intersections.ElementAt(winnerIndex)));
                                    Vector intersectionRayDirection = cameraRayDirection;

                                    Color intersectionColor = getColorAt(intersectionPosition, intersectionRayDirection, sceneObjects, lightSources, winnerIndex, accuracy, ambientlight, 0);
                                    tempRed[aaIndex] = intersectionColor.red * 255;
                                    tempGreen[aaIndex] = intersectionColor.green * 255;
                                    tempBlue[aaIndex] = intersectionColor.blue * 255;
                                }
                            }

                        }
                    }
                    double totalRed = 0;
                    double totalGreen = 0;
                    double totalBlue = 0;

                    for (int iRed = 0; iRed < aaDepth * aaDepth; iRed++)
                    {
                        totalRed = totalRed + tempRed[iRed];
                    }
                    for (int iGreen = 0; iGreen < aaDepth * aaDepth; iGreen++)
                    {
                        totalGreen = totalGreen + tempGreen[iGreen];
                    }
                    for (int iBlue = 0; iBlue < aaDepth * aaDepth; iBlue++)
                    {
                        totalBlue = totalBlue + tempBlue[iBlue];
                    }

                    double avgRed = totalRed / (aaDepth * aaDepth);
                    double avgGreen = totalGreen / (aaDepth * aaDepth);
                    double avgBlue = totalBlue / (aaDepth * aaDepth);

                    bmp.Bits[currentDrawnPixel] = System.Drawing.Color.FromArgb((int)(avgRed), (int)(avgGreen), (int)(avgBlue)).ToArgb();


                }
            }

            return Tuple.Create(section, true);
        }

        public rayTracerUI()
        {
            InitializeComponent();

            // pick first platform
            platform = ComputePlatform.Platforms[0];

            // create context with all gpu devices
            context = new ComputeContext(ComputeDeviceTypes.Gpu,
                new ComputeContextPropertyList(platform), null, IntPtr.Zero);

            // create a command queue with first gpu found
            queue = new ComputeCommandQueue(context,
                context.Devices[0], ComputeCommandQueueFlags.None);

            // load opencl source
            streamReader = new StreamReader("../../kernels.cl");
            string clSource = streamReader.ReadToEnd();
            streamReader.Close();

            // create program with opencl source
            program = new ComputeProgram(context, clSource);

            // compile opencl source
            //program.Build(null, null, null, IntPtr.Zero);

            // load chosen kernel from program
           // kernel = program.CreateKernel("findIntersection");

            threadsTxt.Text = Environment.ProcessorCount.ToString();
        }

        private async void renderBtn_Click(object sender, EventArgs e)
        {
            renderBtn.Enabled = false;
            statusTxt.Text = "Rendering...";
            if (!int.TryParse(aaSamplesTxt.Text, out aaDepth))
            {
                aaSamplesTxt.Text = "Positive integers";
                return;
            }
            if (aaDepth == 0)
            {
                aaSamplesTxt.Text = "Positive integers";
                return;
            }

            if (!int.TryParse(threadsTxt.Text, out threadCount))
            {
                threadsTxt.Text = "Positive integers";
                return;
            }
            if (threadCount == 0)
            {
                threadsTxt.Text = "Positive integers";
                return;
            }

            Application.DoEvents();

            var series = Enumerable.Range(1, threadCount).ToList();
            var tasks = new List<Task<Tuple<int, bool>>>();

            //execution timer
            var watch = System.Diagnostics.Stopwatch.StartNew();

            foreach (var i in series)
            {
                Console.WriteLine();
                Console.WriteLine("Starting Process {0}", i);
                tasks.Add(Task.Run(() => doWork(i)));
            }
            int totalCompleted = 0;
            foreach (var task in await Task.WhenAll(tasks))
            {
                if (task.Item2)
                {
                    Console.WriteLine();
                    Console.WriteLine("Ending Process {0}", task.Item1);
                    totalCompleted++;
                }
            }

            if (totalCompleted == threadCount)
            {
                //end execution timer and render out image
                renderBox.Image = bmp.Bitmap;
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                double frames = 1000 / elapsedMs;
                double fps = Math.Floor(frames);
                renderTimeTxt.Text = elapsedMs.ToString() + "ms";
                statusTxt.Text = "Render complete";
                fpsTxt.Text = fps.ToString();
                renderBtn.Enabled = true;
            }

        }
    }
}
