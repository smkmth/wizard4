using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Possesable : IGameComponent
    {
        bool isPlayer= false;
        GameEntity currentController;

        public Possesable(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Possesable";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new Possesable(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {

            if (gameEvent is GE_FinishAction)
            {
                if (isPlayer)
                {
                    Game.Instance.EndTurn();
                }
            }
            if (gameEvent is GE_PlayerUnPossess unpossess)
            {
                Owner.ResolveGameEvent(new GE_ChangeRenderColor(Color.White, Color.Transparent));
                isPlayer = false;
            }
            if (gameEvent is GE_PossessEntity possess)
            {
                if (currentController != null)
                {
                    currentController.ResolveGameEvent(new GE_PlayerUnPossess());
                }
                currentController = possess.Controller;
                Possess();
            }
            if (gameEvent is GE_Death death)
            {
                if (currentController == null)
                {
                    Game.Instance.Map.LoadMap(Game.Instance.Map.CurrentMap);
                }
                else
                {
                    currentController.ResolveGameEvent(new GE_PossessEntity(null));
                }
            }
        }

        private void Possess()
        {
            Game.Instance.Map.Player = Owner;
            Game.Instance.Map.TriggerGlobalEvent(new GE_PlayerPossess(Owner));
            Owner.ResolveGameEvent(new GE_ChangeRenderColor(Color.Gold, Color.Transparent));
            isPlayer = true;
        }
    }
}
