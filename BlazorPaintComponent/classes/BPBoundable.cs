using System;
using System.Collections.Generic;
using System.Text;
using Geometry;

namespace BlazorPaintComponent.classes
{
    public abstract class BPBoundable
    {
        public abstract RectD BoundingRectD(bool padding = true);
    }
}
