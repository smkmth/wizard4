using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;

namespace Wizard5.Functionality
{
    public enum AbilityType
    {
        TargetSelf,
        TargetEntity
    }


    public struct AbilityContainer
    {
        public AbilityContainer(string abilityName, char abilityKey, AbilityType ablityType  )
        {
            AbilityName = abilityName;
            AbilityKey = abilityKey;
            AbilityType = ablityType;
        }

        public string AbilityName { get; }
        public char AbilityKey { get; }
        public AbilityType AbilityType { get; }   
    }
    

    public class UI
    {

        public string ZKey { get; set; } = "defaultZKey";   
        public string XKey { get; set; } = "defaultXKey";   
        public string CKey { get; set; } = "defaultCKey";   
        public string MouseOver { get; set; } = DefaultMouseText;

        public const string DefaultMouseText = "Hover Mouse To See What's Here";

        public string DebugString { get; set; } = "";   

        public void Draw(ScreenSurface uiSurface)
        {
            uiSurface.Print(2, 2, MouseOver);
            uiSurface.Print(2, 4, "Z = " + ZKey);
            uiSurface.Print(2, 6, "X = " + XKey);
            uiSurface.Print(2, 8, "C = " + CKey);
            uiSurface.Print(2, 15, DebugString);
        }

        public void RegisterAbilities(Dictionary<char, AbilityContainer> abilities)
        {
            if (abilities.ContainsKey('c'))
            {
                CKey =abilities['c'].AbilityName;
            }
            else
            {
                CKey = "No Ability";
            }
            if (abilities.ContainsKey('x'))
            {
                XKey = abilities['x'].AbilityName;
            }
            else
            {
                XKey = "No Ability";
            }
            if (abilities.ContainsKey('z'))
            {
                ZKey = abilities['z'].AbilityName;
            }
            else
            {
                ZKey = "No Ability";
            }

        }
    }
}
