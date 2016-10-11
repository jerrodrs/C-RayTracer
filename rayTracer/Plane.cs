using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Plane: Object
    {
        public Vector normal { get; private set; }
        public double distance { get; private set; }
        public override Color color { get; }

        public Plane()
        {
            normal = new Vector(1, 0, 0);
            distance = 0.0;
            color = new Color(0.5, 0.5, 0.5, 0);
        }

        public Plane(Vector initNormal, double d, Color initColor)
        {
            normal = initNormal;
            distance = d;
            color = initColor;
        }

        public override string getType()
        {
            return "plane";
        }

        public override Vector getNormalAt(Vector point)
        {
            return normal;
        }

        public override double findIntersection(Ray ray)
        {
            Vector rayDirection = ray.direction;
            double a = rayDirection.dotProduct(normal);

            if(a == 0)
            {
                //ray is parallel to plane
                return -1;
            }else
            {
                double b = normal.dotProduct(ray.origin.add(normal.multiply(distance).negate()));
                return -1*b/a;
            }
        }

        public override double[] getProperties()
        {
            return new double[1] { distance };
        }
    }
}
