using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintRectangle : BPaintObject
    {
        private BPaintVertex _end;
        

        public BPaintRectangle()
        {
            ObjectType = BPaintOpbjectType.Rectangle;
            MandatoryVerticesCount = 2;
        }

        public BPaintVertex end
        {
            get { return _end; }
            set
            {
                _end = value;
                VerticesList.Add(_end);
            }
        }


        public bool IsValid()
        {
            return (end.PtD.DistanceTo(Position.PtD) >= validityTolerance);
        }


        public override RectD BoundingRectD(bool padding = true)
        {
            return BPaintFunctions.Get_Border_Points(this, padding:padding);
        }

    }
}
