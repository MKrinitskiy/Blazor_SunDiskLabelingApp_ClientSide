using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorPaintComponent.classes
{
    public enum BPaintOpbjectType
    {
        Rectangle = 0,
        Line =1,
        Circle = 2,
        Ellipse = 3,

    }


    public enum BPaintMoveDirection
    {
        left = 0,
        right = 1,
        up = 2,
        down = 3,
    }

    public enum BPaintMode
    {
        idle = 0,
        drawing = 1,
        movingAnElement = 3,
        hoveredAVertex = 4,
    }


    public enum SelectionMode
    {
        idle = 0,
        movingAnElement = 1,
        hoveredAVertex = 2,
    }


    public enum OperationalMode
    {
        select = 0,
        draw = 1,
    }
}
