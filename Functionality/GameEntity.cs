using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Wizard5.Functionality;
using Wizard5.Functionality.Component;

namespace Wizard5
{


    public class GameEntity
    {
        public static int EntityCounter;
        public int EntityID;
        public string Name;

  

        public List<IGameComponent> Components { get; set; } = new List<IGameComponent>();


        public GameEntity(string name, List<IGameComponent> components)
        {
            this.Name = name;
            this.Components = components;
            EntityID = EntityCounter;
            EntityCounter++;
        }

        public GameEntity()
        {
            EntityID = EntityCounter;
            EntityCounter++;
        }


        public EventResponse? ResolveGameEvent(GameEvent gameEvent)
        {
            foreach (var component in Components)
            {
                component.ReceiveEvent(gameEvent);
            }

            return gameEvent.Response;
        }

        public GameEntity Copy(GameEntity newOwner)
        {
            newOwner.Name = Name;

            foreach (IGameComponent comp in Components)
            {
                newOwner.Components.Add(comp.Copy(newOwner));
            }
            return newOwner;
        }

        public Point GetPosition()
        {
            var pos = (R_RequestPosition)ResolveGameEvent(new GE_RequestPosition());
            if (pos != null)
            {
                return (pos.X, pos.Y);
            }
            return (-1, 1);
        }
        public bool HasComponent<T>() where T : IGameComponent
        {
            var comp = TryGetComponent<T>();
            return (comp != null);
        }

        public IGameComponent TryGetComponent<T>() where T : IGameComponent
        {
            foreach (IGameComponent check in Components)
            {
                if (check.GetType() == typeof(T))
                {
                    return check;
                }

            }
            return null;
        }

      
    }
}
