using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Geometry
{
    [Serializable]
    public class RectD
    {
        private const double tolerance = double.Epsilon;

        public double x { get; set; }
        public double y { get; set; }

        public double width { get; set; }
        public double height { get; set; }



        public bool Equals(RectD r2)
        {
            if (r2 is null)
            {
                return false;
            }

            double dx1 = (x - r2.x);
            double dx2 = (x + width - (r2.x + r2.width));
            double dy1 = (y - r2.y);
            double dy2 = (y + height - (r2.y + r2.height));

            double res = Math.Sqrt(dx1 * dx1 + dx2 * dx2 + dy1 * dy1 + dy2 * dy2);
            
            return (res <= double.Epsilon);
        }


        public static bool operator ==(RectD r1, RectD r2)
        {
            if (r1 is null)
            {
                return false;
            }

            return r1.Equals(r2);
        }

        public static bool operator !=(RectD r1, RectD r2)
        {
            return !(r1 == r2);
        }

        public override string ToString()
        {
            return "xl: " + x + "; yt: " + y + "; xr: " + (x+width) + "; yb: " + (y+height);
        }
    }
}
