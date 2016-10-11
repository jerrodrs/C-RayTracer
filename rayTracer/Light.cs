using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Light : Source
    {
        public override Vector position { get; }
        public override Color color { get; }

        public Light()
        {
            position = new Vector(0, 0, 0);
            color = new Color(1, 1, 1, 0);
        }

        public Light(Vector initVector, Color initColor)
        {
            position = initVector;
            color = initColor;
        }
    }
}
