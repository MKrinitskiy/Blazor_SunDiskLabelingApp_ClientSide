using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent.classes
{
    [Serializable]
    public class BPaintHandDraw: BPaintObject
    {
        public PointD Position { get; set; }
        public List<PointD> data;
        
        public bool IsValid()
        {

            return (data.Any());

        }

    }
}
