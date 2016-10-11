using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Cube : Object
    {
        public Vector center { get; private set; }
        public double radius { get; private set; }
        public override Color color { get; }

        public Cube()
        {
            center = new Vector(0, 0, 0);
            radius = 1.0;
            color = new Color(0.5, 0.5, 0.5, 0);
        }

        public Cube(Vector initCenter, double r, Color initColor)
        {
            center = initCenter;
            radius = r;
            color = initColor;
        }

        public override Vector getNormalAt(Vector point)
        {
            //normal always points away from center of sphere
            Vector normalVector = point.add(center.negate()).normalize();
            return normalVector;
        }

        public override double findIntersection(Ray ray)
        {
            Vector rayOrigin = ray.origin;
            double rayOriginX = rayOrigin.x;
            double rayOriginY = rayOrigin.y;
            double rayOriginZ = rayOrigin.z;

            Vector rayDirection = ray.direction;
            double rayDirectionX = rayDirection.x;
            double rayDirectionY = rayDirection.y;
            double rayDirectionZ = rayDirection.z;

            Vector sphereCenter = center;
            double sphereCenterX = sphereCenter.x;
            double sphereCenterY = sphereCenter.y;
            double sphereCenterZ = sphereCenter.z;

            double a = 1; //normalized
            double b = (2 * (rayOriginX - sphereCenterX) * rayDirection.x) + (2 * (rayOriginY - sphereCenterY) * rayDirection.y) + (2 * (rayOriginZ - sphereCenterZ) * rayDirection.z);
            double c = Math.Pow(rayOriginX - sphereCenterX, 2) + Math.Pow(rayOriginY - sphereCenterY, 2) + Math.Pow(rayOriginZ - sphereCenterZ, 2) - Math.Pow(radius, 2);

            double discriminant = b * b - 4 * c;

            if (discriminant > 0)
            {
                //ray intersects
                double rootOne = ((-1 * b - Math.Sqrt(discriminant)) / 2) - 0.000001;

                if (rootOne > 0)
                {
                    //first root is the smallest positive root
                    return rootOne;
                }
                else
                {
                    //second root is the smallest positive root
                    return ((Math.Sqrt(discriminant) - b) / 2) - 0.000001;
                }
            }
            else
            {
                //ray missed sphere
                return -1;
            }

            return 0.0;
        }
    }
}
