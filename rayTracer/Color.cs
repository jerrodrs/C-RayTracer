using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rayTracer
{
    class Color
    {
        public double red { get; private set; }
        public double green { get; private set; }
        public double blue { get; private set; }
        public double special { get; private set; }

        public Color()
        {
            red = 0.5;
            green = 0.5;
            blue = 0.5;
        }

        public Color(double r, double g, double b)
        {
            red = r;
            green = g;
            blue = b;
        }

        public Color(double r, double g, double b, double s)
        {
            red = r;
            green = g;
            blue = b;
            special = s;
        }

        public void setRed(double r)
        {
            red = r;
        }

        public void setGreen(double g)
        {
            green = g;
        }

        public void setBlue(double b)
        {
            blue = b;
        }

        public double brightness()
        {
            return (red + green + blue) / 3;
        }

        public Color colorScalar(double scalar)
        {
            return new Color(red * scalar, green * scalar, blue * scalar, special);
        }

        public Color colorAdd(Color color)
        {
            return new Color(red + color.red, green + color.green, blue + color.blue, special);
        }

        public Color colorMultiply(Color color)
        {
            return new Color(red * color.red, green * color.green, blue * color.blue, special);
        }

        public Color colorAverage(Color color)
        {
            return new Color((red + color.red) / 2, (green + color.green) / 2, (blue + color.blue) / 2, special);
        }
        
        public Color clip()
        {
            double allLight = red + blue + green;
            double excesslight = allLight - 3;
            if(excesslight > 0)
            {
                red = red + excesslight * (red / allLight);
                green = green + excesslight * (green / allLight);
                blue = blue + excesslight * (blue / allLight);
            }
            if (red > 1) { red = 1; }
            if (green > 1) { green = 1; }
            if (blue > 1) { blue = 1; }
            if (red < 0) { red = 0; }
            if (green < 0) { green = 0; }
            if (blue < 0) { blue = 0; }

            return new Color(red, green, blue, special);
        }

    }
}
