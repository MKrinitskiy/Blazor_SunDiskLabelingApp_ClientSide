using System;
using System.Collections.Generic;
using System.Text;
using Blazor2PythonWebAPI_interfaces.LabelClasses;
using Geometry;

namespace Blazor2PythonWebAPI_interfaces
{
    [Serializable]
    public class Label
    {
        public LabelTypes labelType = LabelTypes.Rectangle;
        public SizeD OriginalExampleSize = SizeD.Empty;
        public RectD ViewportRectangle = null;

        public ILabelShape Shape { get; set; }
    }
}
