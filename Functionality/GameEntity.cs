using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Wizard5.Functionality.Component;

namespace Wizard5
{
    public class GameEntity
    {

        public List<IGameComponent> Components { get; set; } = new List<IGameComponent>();

        public EventResponse? ResolveGameEvent(GameEvent gameEvent)
        {
            foreach (var component in Components)
            {
                component.ReceiveEvent(gameEvent);
            }

            return gameEvent.Response;
        }

        public (int,int) GetPosition()
        {
            var pos = (R_RequestPosition)ResolveGameEvent(new GE_RequestPosition());
            if (pos != null)
            {
                return (pos.X, pos.Y);
            }
            return (-1, 1);
        }

    }
}
