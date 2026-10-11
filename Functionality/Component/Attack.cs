using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Attack : IGameComponent
    {
        int damage;

        public Attack(GameEntity owner, int damage)
        {
            Owner = owner;
            this.damage = damage;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Attacker";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Attack(newOwner, damage) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_FormAttack attack)
            {
                attack.Response = new R_FormAttackResult(damage);
            }
        }
    }
}

