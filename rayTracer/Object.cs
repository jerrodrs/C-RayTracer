using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Object
    {
        public virtual Color color { get; }

        public virtual Vector getNormalAt(Vector point)
        {
            return new Vector(0, 0, 0);
        }

        public virtual double findIntersection(Ray ray)
        {
            return 0;
        }
        
        public virtual string getType()
        {
            return "object";
        }

        public virtual double[] getProperties()
        {
            return new double[] { };
        }
    }
}
