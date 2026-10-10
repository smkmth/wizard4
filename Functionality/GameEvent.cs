using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5
{
    public class GameEvent
    {
        public EventResponse? Response { get; set; }
    }

    public class EventResponse
    {
    }

    public class GE_RequestPosition : GameEvent
    {
    }

    public class  R_RequestPosition : EventResponse
    {
        public int X { get; set; }
        public int Y { get; set; }

    }

    public class GE_SetPosition : GameEvent
    {
        public int X;
        public int Y;

        public GE_SetPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class GE_Move : GameEvent
    {
        public int DeltaX;
        public int DeltaY;

        public GE_Move(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }

    public class GE_Draw : GameEvent 
    {
        public ScreenSurface Surface;

        public GE_Draw(ScreenSurface surface)
        {
            Surface = surface;
        }
    }


}
