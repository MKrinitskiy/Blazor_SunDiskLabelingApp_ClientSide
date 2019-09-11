using System;
using System.Collections.Generic;
using System.Text;
using Geometry;

namespace Blazor2PythonWebAPI_interfaces.LabelClasses
{
    [Serializable]
    public class CircleLabel : ILabelShape
    {
        public string uid { get; set; }
        public PointD StartPoint { get; set; } // center point
        public PointD EndPoint { get; set; } // any point at the circle
    }
}
