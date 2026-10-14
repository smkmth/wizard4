using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class AI : IGameComponent
    {


        public AI(GameEntity owner)
        {
            Owner = owner;
    
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "AI";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new AI(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_SomthingMoved move)
            {
                Owner.ResolveGameEvent(new GE_CheckSight());
            }
        }
    }
}
