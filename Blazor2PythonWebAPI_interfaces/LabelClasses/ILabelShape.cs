using System;
using System.Collections.Generic;
using System.Text;
using Geometry;

namespace Blazor2PythonWebAPI_interfaces.LabelClasses
{
    public interface ILabelShape
    {
        string uid { get; set; }
        PointD StartPoint { get; set; }
        PointD EndPoint { get; set; }
    }
}
