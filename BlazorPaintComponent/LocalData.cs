using BlazorPaintComponent.classes;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent
{
    public static class LocalData
    {

        public static PointD SVGPosition = new PointD(0, 0);

        //[JSInvokable]
        //public static void invokeFromjs_UpdateSVGPosition(string par_x, string par_y)
        //{
        //    double x = Convert.ToDouble(par_x);
        //    double y = Convert.ToDouble(par_y);
        //    SVGPosition = new PointD(x, y);
        //}

        [JSInvokable]
        public static void invokeFromjs_UpdateSVGPosition(string id, double rect_left, double rect_top, double rect_width, double rect_height, double window_scrollX, double window_scrollY)
        {
            Console.WriteLine("id = " + id + "; left = " + rect_left + "; top = " + rect_top + "; width = " +
                              rect_width + "; height = " + rect_height + "; w_scrollX = " + window_scrollX +
                              "; w_scrollY = " + window_scrollY);

            //SVGPosition = new PointD(rect_left + window_scrollX, rect_top + window_scrollY);
            SVGPosition = new PointD(rect_left, rect_top);
        }



    }
}
