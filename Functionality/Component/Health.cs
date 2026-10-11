using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Health : IGameComponent
    {

        private int baseValue;
        private int currentValue;
        private GameEntity corpse;

        public Health(GameEntity owner, int baseValue, GameEntity corpse)
        {
            Owner = owner;
            this.baseValue = baseValue;
            currentValue = this.baseValue;
            this.corpse = corpse;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Health";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Health(newOwner, baseValue, corpse) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_TakeDamage damage)
            {
                currentValue -= damage.damageAmount;
                if (currentValue <= 0)
                {
                    var pos = Owner.GetPosition();
                    Game.Instance.PlaceEntity(corpse,pos.Item1, pos.Item2);
                    Game.Instance.DestroyEntity(Owner);
                }
            }
        }
    }
}

