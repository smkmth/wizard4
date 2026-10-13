using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class KillOnTouch : IGameComponent
    {

        public KillOnTouch(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "KillOnTouch";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new KillOnTouch(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_Collide move)
            {
                move.movingEntity.ResolveGameEvent(new GE_TakeDamage(20000));
            }
        }
    }
}
