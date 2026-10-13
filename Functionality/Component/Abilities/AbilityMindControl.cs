using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component.Skills;

namespace Wizard5.Functionality.Component.Abilities
{
    public class AbilityMindControl : AbilityBase
    {

        public AbilityMindControl(GameEntity owner) : base(owner)
        {
            Owner = owner;
            abilityName = "MindControl";
        }

        protected override IGameComponent CopyImplementation(GameEntity newOwner)
        {
            return new AbilityMindControl(newOwner);
        }

        protected override (string, AbilityType) RegisterAbility()
        {
            return (abilityName, AbilityType.TargetEntity);
        }
        protected override void TriggerAbility(GE_TriggerAbility trigger)
        {
            var targetAbility = trigger as GE_TriggerAbilityTargetEntity;
            targetAbility.Target.ResolveGameEvent(new GE_PossessEntity(Owner));


        }

    }
}
