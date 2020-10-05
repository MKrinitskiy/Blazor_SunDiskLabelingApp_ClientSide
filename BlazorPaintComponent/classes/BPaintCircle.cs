using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;
//using System.Text.Json;
//using System.Text.Json.Serialization;
using Newtonsoft.Json;


namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintCircle : BPaintObject
    {
        private BPaintVertex _end;

        public BPaintCircle()
        {
            ObjectType = BPaintOpbjectType.Circle;
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
