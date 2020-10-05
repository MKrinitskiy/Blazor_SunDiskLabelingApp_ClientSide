using System;
using System.Collections.Generic;
using System.Text;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;
using Newtonsoft.Json;


namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintVertex
    {
        //public int ObjectID { get; set; }
        [JsonIgnore]
        public bool Selected;
        //public bool EditMode { get; set; }
        //public int SequenceNumber { get; set; }


        [JsonIgnore]
        public string Color;
        //public double width { get; set; }

        public PointD PtD { get; set; }
        //public PointD PositionChange { get; set; } = new PointD(0,0);
        //public PointD Scale { get; set; } = new PointD(0, 0);
        //public BPaintOpbjectType ObjectType { get; set; }


        [JsonIgnore]
        public SVGtransform transform { get; set; } = new SVGtransform();

        [JsonIgnore]
        public double x
        {
            get { return PtD.X; }
        }

        [JsonIgnore]
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
