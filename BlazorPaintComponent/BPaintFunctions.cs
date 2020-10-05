using BlazorPaintComponent.classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent
{
    public static class BPaintFunctions
    {

        public static RectD Get_Border_Points(BPaintHandDraw Par_obj, bool padding = true)
        {
            RectD result = new RectD();
            
            List<PointD> data = Par_obj.data.ToList();
            data.Add(Par_obj.Position.PtD);

            result.x = data.Min(j => j.X);
            result.y = data.Min(j => j.Y);
            result.width = data.Max(j => j.X) - result.x;
            result.height = data.Max(j => j.Y) - result.y;

            if (padding)
            {
                Set_Padding(result);
            }
            
            return result;


        }


        public static RectD Get_Border_Points(BPaintLine Par_obj, bool padding = true)
        {

            RectD result = new RectD();


            List<PointD> data = new List<PointD>();
            data.Add(Par_obj.Position.PtD);
            data.Add(Par_obj.end.PtD);

            result.x = data.Min(j => j.X);
            result.y = data.Min(j => j.Y);
            result.width = data.Max(j => j.X) - result.x;
            result.height = data.Max(j => j.Y) - result.y;

            if (padding)
            {
                Set_Padding(result);
            }

            return result;


        }




        public static RectD Get_Border_Points(BPaintVertex Par_obj)
        {
            RectD result = new RectD();
            
            result.x = Par_obj.x - 10;
            result.y = Par_obj.y - 10;
            result.width = 20;
            result.height = 20;

            return result;
        }



        public static RectD Get_Border_Points(BPaintCircle Par_obj, bool padding = true)
        {
            RectD result = new RectD();
            List<PointD> data = new List<PointD>();

            PointD cPoint = Par_obj.Position.PtD;
            double r = Par_obj.Position.PtD.DistanceTo(Par_obj.end.PtD);
            
            data.Add(cPoint + new SizeD(0, r));
            data.Add(cPoint + new SizeD(0, -r));
            data.Add(cPoint + new SizeD(r, 0));
            data.Add(cPoint + new SizeD(-r, 0));

            result.x = data.Min(j => j.X);
            result.y = data.Min(j => j.Y);
            result.width = data.Max(j => j.X) - result.x;
            result.height = data.Max(j => j.Y) - result.y;

            if (padding)
            {
                Set_Padding(result);
            }

            return result;
        }



        public static RectD Get_Border_Points(BPaintRectangle Par_obj, bool padding = true)
        {
            RectD result = new RectD();
            
            List<PointD> data = new List<PointD>();
            data.Add(Par_obj.Position.PtD);
            data.Add(Par_obj.end.PtD);

            result.x = data.Min(j => j.X);
            result.y = data.Min(j => j.Y);
            result.width = data.Max(j => j.X) - result.x;
            result.height = data.Max(j => j.Y) - result.y;

            if (padding)
            {
                Set_Padding(result);
            }

            return result;
        }




        public static RectD Get_Border_Points(BPaintEllipse Par_obj, bool padding = true)
        {
            RectD result = new RectD();
            SizeD bRectSizeD = Par_obj.boundingRectangleSizeD;
            PointD cPoint = Par_obj.centerPointD;

            result.x = cPoint.X - 0.5 * bRectSizeD.Width;
            result.y = cPoint.Y - 0.5 * bRectSizeD.Height;
            result.width = bRectSizeD.Width;
            result.height = bRectSizeD.Height;

            if (padding)
            {
                Set_Padding(result);
            }

            return result;
        }



        public static void Set_Padding(RectD r)
        {
            int padding = 5;

            r.x -= padding;
            r.y -= padding;
            r.width += padding * 2;
            r.height += padding * 2;
        }


       


        public static T CopyObject<T>(object objSource)

        {

            using (MemoryStream stream = new MemoryStream())

            {

                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(stream, objSource);

                stream.Position = 0;

                return (T)formatter.Deserialize(stream);

            }

        }
    }
}
