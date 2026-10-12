using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component.Skills
{
    public class AbilityBase : IGameComponent
    {
        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "SkillControlMind";

        protected string abilityName;

        public AbilityBase(GameEntity owner)
        {
            Owner = owner;
        }
      
        IGameComponent IGameComponent.Copy(GameEntity newOwner)
        {
            return new AbilityBase(newOwner);
        }

        void IGameComponent.ReceiveEvent(GameEvent gameEvent)
        {
            RecieveEventImplementation(gameEvent);
        }

        protected virtual void RecieveEventImplementation(GameEvent gameEvent)
        {
            if (gameEvent is GE_RegisterAbilities register)
            {
                var response = register.Response as R_RegisterAbility;

                response?.AbilityContainers.Add(RegisterAbility());
            }
            if (gameEvent is GE_TriggerAbility trigger)
            {
                if (trigger.AbilityName == abilityName)
                {
                    TriggerAbility(trigger);
                }
            }
        }

        protected virtual (string, AbilityType) RegisterAbility()
        {
            return ("default", AbilityType.TargetSelf);   
        }

        protected virtual void TriggerAbility(GE_TriggerAbility trigger) 
        {
        
        }
    }
}
