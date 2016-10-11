using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Source
    {
        public virtual Vector position { get; }
        public virtual Color color { get; }
    }
}
