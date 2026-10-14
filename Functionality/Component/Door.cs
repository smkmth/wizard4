using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Door : IGameComponent
    {

        bool isOpen;

        public Door(GameEntity owner, bool isopen)
        {
            Owner = owner;
            isOpen = isopen;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Door";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Door(newOwner, isOpen) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_Collide move)
            {
                if (isOpen)
                {
                    gameEvent.Response = new R_CollideResult(true);

                }
                else
                {
                    gameEvent.Response = new R_CollideResult(false);
                    Game.Instance.UI.OverlaySurfaceEffect("Locked!", 1.0f, Owner.GetPosition());
                    var respose = move.movingEntity.ResolveGameEvent(new GE_UnlockDoor()) as R_UnlockDoor;
                    if (respose != null && respose.Success)
                    {
                        Game.Instance.UI.OverlaySurfaceEffect("Unlocked!", 1.0f, Owner.GetPosition());
                        isOpen = true;
                        Owner.ResolveGameEvent(new GE_ChangeRenderSymbol('-'));
                        Owner.ResolveGameEvent(new GE_SetOpaque(true));
                    }
                }
            }
        }
    }
}
