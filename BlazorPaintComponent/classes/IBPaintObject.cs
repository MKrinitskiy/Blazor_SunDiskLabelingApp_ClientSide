using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Geometry;

namespace BlazorPaintComponent.classes
{
    public interface IBPaintObject
    {
        int ObjectID { get; set; }
        bool Selected { get; set; }

        bool EditMode { get; set; }

        int SequenceNumber { get; set; }

        //List<BPaintVertex> VerticesList { get; set; }

        string Color { get; set; }
        double LineWidth { get; set; }


        BPaintVertex Position { get; set; }
        
        PointD Scale { get; set; }

        BPaintOpbjectType ObjectType { get;  }

    }
}
