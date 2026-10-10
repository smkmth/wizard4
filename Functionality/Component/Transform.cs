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

        public GameEntity? Owner { get; set; }

        public string ComponentName { get; } = "Transform";

        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Transform { Owner = newOwner, X = this.X, Y = this.Y };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_RequestPosition requestPosition)
            {
                requestPosition.Response = new R_RequestPosition { X = this.X, Y = this.Y };
            }
        }
    }
}
