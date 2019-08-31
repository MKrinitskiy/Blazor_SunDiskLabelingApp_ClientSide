using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintObject : BPBoundable, IBPaintObject
    {
        public int ObjectID { get; set; }
        public bool Selected { get; set; }
        public bool EditMode { get; set; }
        public int SequenceNumber { get; set; }

        public string Color { get; set; }
        public double LineWidth { get; set; }

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

        public PointD Scale { get; set; } = new PointD(0, 0);
        public BPaintOpbjectType ObjectType { get; set; }

        
        public List<BPaintVertex> VerticesList = new List<BPaintVertex>();

        public SVGtransform transform { get; set; } = new SVGtransform();

        public int MandatoryVerticesCount { get; set; }

        public override RectD BoundingRectD(bool padding = true)
        {
            throw new NotImplementedException();
        }
    }
}
