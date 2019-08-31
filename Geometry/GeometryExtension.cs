using System;
using System.Collections.Generic;
using System.Text;
using CommonLibs;

namespace Geometry
{
    public static class GeometryExtension
    {
        public static RectD RectD_intersection(RectD r1, RectD r2)
        {
            double x = Math.Max(r1.x, r2.x);
            double y = Math.Max(r1.y, r2.y);
            double w = Math.Min(r1.x + r1.width, r2.x + r2.width) - x;
            double h = Math.Min(r1.y + r1.height, r2.y + r2.height) - y;

            if (w < 0.0 || h < 0.0)
            {
                return null;
            }

            return new RectD()
            {
                x = x,
                y = y,
                height = h,
                width = w
            };
        }

        
        public static RectD IntersectionWith(this RectD r1, RectD r2) => RectD_intersection(r1, r2);


        public static RectD RectD_union(RectD r1, RectD r2)
        {
            double x = Math.Min(r1.x, r2.x);
            double y = Math.Min(r1.y, r2.y);
            double w = Math.Max(r1.x + r1.width, r2.x + r2.width) - x;
            double h = Math.Max(r1.y + r1.height, r2.y + r2.height) - y;
            
            if (w < 0.0 || h < 0.0)
            {
                return null;
            }

            return new RectD()
            {
                x = x,
                y = y,
                height = h,
                width = w
            };
        }


        public static RectD UnionWith(this RectD r1, RectD r2) => RectD_union(r1, r2);



        public static bool RectD_intersect(RectD r1, RectD r2)
        {
            return (r1.IntersectionWith(r2) != null);
        }


        public static bool IsInRect(this RectD r1, RectD r2)
        {
            if (r1 is null || r2 is null)
            {
                return false;
            }

            RectD intersection = RectD_intersection(r1, r2);
            if (intersection is null)
            {
                return false;
            }
            else
            {
                return intersection.Equals(r1);
            }
        }


        
    }
}
