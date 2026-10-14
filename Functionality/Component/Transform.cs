using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Transform : IGameComponent 
    {
        private int x;
        private int y;

        public Transform(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Transform";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Transform(newOwner) { x = this.x, y = this.y };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_RequestPosition requestPosition)
            {
                requestPosition.Response = new R_RequestPosition { X = this.x, Y = this.y };
            }

            if (gameEvent is GE_SetPosition setPos)
            {
                x = setPos.X;
                y = setPos.Y;
            }
            if  (gameEvent is GE_Move move)
            {
                var entsAtFuturePosition = Game.Instance.Map.GetEntitiesAtPosition(x + move.DeltaX, y + move.DeltaY);
                var canMove = true;
                var tryingToCollide = new GE_Collide(Owner);
                var oldpos = (x, y);

                foreach (var ent in entsAtFuturePosition)
                {
                    var result = (R_CollideResult)ent.ResolveGameEvent(tryingToCollide);
                    if (result != null && result.canMove == false)
                    {
                        var attackResult = (R_FormAttackResult)Owner.ResolveGameEvent(new GE_FormAttack(ent));
                        if (attackResult != null)
                        {
                            ent.ResolveGameEvent(new GE_TakeDamage(attackResult.damage));
                            Owner.ResolveGameEvent(new GE_FinishAction(oldpos,(x,y)));

                        }
                        return; //we cant move
                    }
                }

                x += move.DeltaX;
                y += move.DeltaY;
                Owner.ResolveGameEvent(new GE_FinishAction(oldpos, (x, y)));
                Game.Instance.Map.TriggerGlobalEvent(new GE_SomthingMoved(Owner));
            }
        }
    }
}
