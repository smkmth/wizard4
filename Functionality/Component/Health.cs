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

        public Health(GameEntity owner, int _baseValue, GameEntity _corpse)
        {
            Owner = owner;
            baseValue = _baseValue;
            currentValue= baseValue;
            corpse = _corpse;
        }

        public GameEntity Owner { get; }

        public string ComponentName { get; } = "Health";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Health(Owner, baseValue, corpse) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_TakeDamage damage)
            {
                currentValue -= damage.damageAmount;
                if (currentValue <= 0)
                {
                    var pos = Owner.GetPosition();
                    Game.Instance.CreateEntity(corpse,pos.Item1, pos.Item2);
                    Game.Instance.DestroyEntity(Owner);
                }
            }
        }
    }
}

