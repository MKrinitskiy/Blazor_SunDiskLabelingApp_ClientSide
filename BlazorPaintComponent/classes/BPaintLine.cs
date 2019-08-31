using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorSvgHelper.Classes.SubClasses;
using Geometry;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintLine : BPaintObject
    {
        private BPaintVertex _end;

        

        public BPaintLine()
        {
            ObjectType = BPaintOpbjectType.Line;
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
            return (!end.Equals(Position));
        }


        public override RectD BoundingRectD(bool padding = true)
        {
            return BPaintFunctions.Get_Border_Points(this, padding: padding);
        }
    }
}
