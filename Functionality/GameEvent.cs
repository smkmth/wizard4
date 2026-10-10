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

}
