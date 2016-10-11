using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Ray
    {
        public Vector origin { get; private set; }
        public Vector direction { get; private set; }

        public Ray()
        {
            origin = new Vector(0, 0, 0);
            direction = new Vector(1, 0, 0);
        }

        public Ray(Vector initOrigin, Vector initDirection)
        {
            origin = initOrigin;
            direction = initDirection;
        }
    }
}
