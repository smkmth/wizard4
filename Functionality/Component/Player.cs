using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Player : IGameComponent
    {
        int damage;

        public Player(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Player";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Player(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_GameStart attack)
            {
                Possess();
            }
        }

        private void Possess()
        {
            Game.Instance.Map.Player = Owner;
            Game.Instance.Map.TriggerGlobalEvent(new GE_PlayerPossess(Owner));
        }
    }
}

