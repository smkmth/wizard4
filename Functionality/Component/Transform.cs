using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Transform : IGameComponent 
    {
        private int X;
        private int Y;

        public Transform(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Transform";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Transform(Owner){ X = this.X, Y = this.Y };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_RequestPosition requestPosition)
            {
                requestPosition.Response = new R_RequestPosition { X = this.X, Y = this.Y };
            }

            if (gameEvent is GE_SetPosition setPos)
            {
                X = setPos.X;
                Y = setPos.Y;
            }
            if  (gameEvent is GE_Move move)
            {
                X += move.DeltaX;
                Y += move.DeltaY;
            }
        }
    }
}
