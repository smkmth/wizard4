using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Opaque : IGameComponent
    {

        bool isStatic;
        bool isTransparent =false;

        public Opaque(GameEntity owner, bool isStatic)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Opaque";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Opaque(newOwner, isStatic) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_CheckOpaque opaque)
            {
                if (opaque.StaticCheck)
                {

                    gameEvent.Response = new R_Opaque(isStatic);
                }
            }
            if (gameEvent is GE_FinishAction move) 
            {
                if (!isStatic && move.moved)
                {
                    Game.Instance.FOV.GetCell(move.oldPos.X, move.oldPos.Y).transparent = true;
                    Game.Instance.FOV.GetCell(move.newPos.X, move.newPos.Y).transparent = false;
                }
            }
            if (gameEvent is GE_GameStart)
            {
                if (!isStatic )
                {
                    var pos = Owner.GetPosition();
                    Game.Instance.FOV.GetCell(pos.X, pos.Y).transparent = false;
                }
            }
            if (gameEvent is GE_SetOpaque opaqueset)
            {
                isTransparent = opaqueset.Transparent;
                var pos = Owner.GetPosition();
                Game.Instance.FOV.GetCell(pos.X, pos.Y).transparent = isTransparent;
            }

        }
    }
}
