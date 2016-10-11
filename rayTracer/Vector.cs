using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Vector
    {
        public double x { get; private set; }
        public double y { get; private set; }
        public double z { get; private set; }

        public Vector() {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vector(double initX, double initY, double initZ)
        {
            x = initX;
            y = initY;
            z = initZ;
        }

        public double magnitude()
        {
            return Math.Sqrt((x*x) + (y*y) + (z*z));
        }

        public Vector normalize()
        {
            double mag = magnitude();
            return new Vector(x / mag, y / mag, z / mag);
        }

        public Vector negate()
        {
            return new Vector(-x, -y, -z);
        }

        public double dotProduct(Vector v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        public Vector crossProduct(Vector v)
        {
            return new Vector(y * v.z - z * v.y, z * v.x - x * v.z, x * v.y - y * v.x);
        }

        public Vector add(Vector v)
        {
            return new Vector(x + v.x, y + v.y, z + v.z);
        }

        public Vector multiply(double scalar)
        {
            return new Vector(x * scalar, y * scalar, z * scalar);
        }
    }
}
