using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Possesable : IGameComponent
    {
        GameEntity currentController;

        public Possesable(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Player";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Possesable(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_MindControl mindControl)
            {
                currentController = mindControl.OldEntity;
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
