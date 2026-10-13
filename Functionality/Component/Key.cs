using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Key : IGameComponent
    {

        public Key(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Key";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Key(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_UnlockDoor unlock)
            {
                gameEvent.Response = new R_UnlockDoor(true);

            }
        }
    }
}
