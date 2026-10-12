using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Wizard5.Functionality.Component
{


    public class AbilityHolder : IGameComponent
    {

        public Dictionary<char, AbilityContainer> Abilities { get; set; } = new Dictionary<char, AbilityContainer>();

        public AbilityHolder(GameEntity owner)
        {
            Owner = owner;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Player";



        public IGameComponent Copy(GameEntity newOwner)
        {
            return new AbilityHolder(newOwner) { };
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_PlayerPossess possess)
            {
                if (possess.Player == Owner)
                {
                    Abilities.Clear();

                    var abilityevent = new GE_RegisterAbilities();
                    abilityevent.Response = new R_RegisterAbility(); //make this here so not every ability is newing the response
                    Owner.ResolveGameEvent(abilityevent);
                    foreach (var ability in (abilityevent.Response as R_RegisterAbility).AbilityContainers)
                    {
                        if (!Abilities.ContainsKey('z'))
                        {
                            Abilities.Add('z', new AbilityContainer(ability.Item1, 'z', ability.Item2));
                            break;
                        }
                        if (!Abilities.ContainsKey('x'))
                        {
                            Abilities.Add('x', new AbilityContainer(ability.Item1, 'x', ability.Item2));        
                            break;
                        }
                        if (!Abilities.ContainsKey('c'))
                        {
                            Abilities.Add('c', new AbilityContainer(ability.Item1, 'c', ability.Item2));
                            break;
                        }
                        //ERROR need max three abilites
                    }
                    Game.Instance.UI.RegisterAbilities(Abilities);
                }   
            }

            if (gameEvent is GE_TriggerAbilityKey trigger)
            {
                if (Abilities.ContainsKey(trigger.AbilityKey))
                {
                    if (Abilities[trigger.AbilityKey].AbilityType == AbilityType.TargetEntity)
                    {
                        Game.Instance.RequestTarget((target) => {
                            Owner.ResolveGameEvent(new GE_TriggerAbilityTargetEntity(target, Abilities[trigger.AbilityKey].AbilityName));
                        });
                    }
                    else
                    {
                        Owner.ResolveGameEvent(new GE_TriggerAbility(Abilities[trigger.AbilityKey].AbilityName));
                    }
                }   
            }
        }
    }
}

