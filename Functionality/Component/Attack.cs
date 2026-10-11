using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Attack : IGameComponent
    {
        int damage;

        public Attack(GameEntity owner, int _damage)
        {
            Owner = owner;
            damage = _damage;
        }

        public GameEntity Owner { get; }

        public string ComponentName { get; } = "Attacker";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Attack(Owner, damage) { };
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

