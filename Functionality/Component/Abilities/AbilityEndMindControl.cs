using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component.Skills;

namespace Wizard5.Functionality.Component.Abilities
{
    public class AbilityEndMindControl : AbilityBase
    {

        public AbilityEndMindControl(GameEntity owner) : base(owner)
        {
            Owner = owner;
            abilityName = "End Mind Control";
        }

        protected override IGameComponent CopyImplementation(GameEntity newOwner)
        {
            return new AbilityEndMindControl(newOwner);
        }

        protected override (string, AbilityType) RegisterAbility()
        {
            return (abilityName, AbilityType.TargetSelf);
        }
        protected override void TriggerAbility(GE_TriggerAbility trigger)
        {
            var targetAbility = trigger as GE_TriggerAbilityTargetEntity;

            Owner.ResolveGameEvent(new GE_PlayerUnPossess());

            Game.Instance.UI.OverlaySurfaceEffect("Possession Ended!", 1.0f, Owner.GetPosition());


        }

    }
}
