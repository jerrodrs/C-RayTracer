using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Camera
    {
        public Vector position { get; private set; }
        public Vector direction { get; private set; }
        public Vector right { get; private set; }
        public Vector down { get; private set; }

        public Camera()
        {
            position = new Vector(0, 0, 0);
            direction = new Vector(1, 0, 0);
            right = new Vector(0, 0, 0);
            down = new Vector(0, 0, 0);
        }

        public Camera(Vector initPosition, Vector initDirection, Vector initRight, Vector initDown)
        {
            position = initPosition;
            direction = initDirection;
            right = initRight;
            down = initDown;
        }
    }
}
