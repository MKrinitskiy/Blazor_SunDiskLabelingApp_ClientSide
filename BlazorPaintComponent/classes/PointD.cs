using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class PointD
    {
        public double x { get; set; }
        public double y { get; set; }

        public PointD(double _x, double _y)
        {
            x = _x;
            y = _y;
        }

        public PointD(PointD pt)
        {
            x = pt.x;
            y = pt.y;
        }

        public static PointD operator +(PointD pt1, PointD pt2)
        {

        }

        public double DistanceTo(PointD pt2)
        {
            return Math.Sqrt((x-pt2.x)* (x - pt2.x) + (y-pt2.y)* (y - pt2.y));
        }
    }
}
