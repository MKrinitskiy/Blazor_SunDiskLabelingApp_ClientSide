using System;
using System.Collections.Generic;
using System.Text;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintVertex
    {
        //public int ObjectID { get; set; }
        public bool Selected { get; set; }
        //public bool EditMode { get; set; }
        //public int SequenceNumber { get; set; }


        public string Color { get; set; }
        //public double width { get; set; }

        public PointD PtD { get; set; }
        //public PointD PositionChange { get; set; } = new PointD(0,0);
        //public PointD Scale { get; set; } = new PointD(0, 0);
        //public BPaintOpbjectType ObjectType { get; set; }


        public SVGtransform transform { get; set; } = new SVGtransform();


        public double x
        {
            get { return PtD.X; }
        }

        public double y
        {
            get { return PtD.Y; }
        }

        public BPaintVertex()
        {
        }

        public BPaintVertex(PointD pt, string color)
        {
            PtD = new PointD(pt);
            Color = color;
        }
    }
}
