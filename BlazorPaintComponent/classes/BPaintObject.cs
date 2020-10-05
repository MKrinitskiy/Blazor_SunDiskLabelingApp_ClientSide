using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;
//using System.Xml;
//using System.Xml.Serialization;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace BlazorPaintComponent.classes
{
    [Serializable]
    //[XmlInclude(typeof(BPaintCircle))]
    //[XmlInclude(typeof(BPaintEllipse))]
    //[XmlInclude(typeof(BPaintHandDraw))]
    //[XmlInclude(typeof(BPaintLine))]
    //[XmlInclude(typeof(BPaintRectangle))]
    public abstract class BPaintObject : BPBoundable, IBPaintObject
    {
        public int ObjectID { get; set; }

        [JsonIgnore]
        public bool Selected { get; set; }

        [JsonIgnore]
        public bool EditMode { get; set; }

        [JsonIgnore]
        public int SequenceNumber { get; set; }

        [JsonIgnore]
        public string Color { get; set; }

        [JsonIgnore]
        public double LineWidth { get; set; }

        [JsonIgnore]
        public double validityTolerance { get; set; } = 10.0d;

        private BPaintVertex _position;
        public BPaintVertex Position
        {
            get { return _position; }
            set
            {
                _position = value;
                VerticesList.Add(_position);
            }
        }

        public PointD Scale { get; set; } = new PointD(1.0, 1.0);
        public BPaintOpbjectType ObjectType { get; set; }

        
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();

        [JsonIgnore]
        public SVGtransform transform { get; set; } = new SVGtransform();

        [JsonIgnore]
        public int MandatoryVerticesCount { get; set; }

        public override RectD BoundingRectD(bool padding = true)
        {
            throw new NotImplementedException();
        }
    }
}
