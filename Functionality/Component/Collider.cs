using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Collider : IGameComponent
    {

        public Collider(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Collider";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Collider(newOwner) {};
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_Collide move)
            {
                gameEvent.Response = new R_CollideResult(false);
            }
        }
    }
}
