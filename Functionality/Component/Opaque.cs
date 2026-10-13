using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Opaque : IGameComponent
    {

        bool isOpen;

        public Opaque(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Opaque";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Opaque(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
        }
    }
}
