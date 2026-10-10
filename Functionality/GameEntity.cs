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
        public Renderer? Renderer { get; set; }  // faster access to the renderer component

        public EventResponse? ResolveGameEvent(GameEvent gameEvent)
        {
            foreach (var component in Components)
            {
                component.ReceiveEvent(gameEvent);
            }

            return gameEvent.Response;
        }

    }
}
