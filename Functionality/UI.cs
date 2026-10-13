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

        public void Draw(TimeSpan delta)
        {
            Game.Instance.UISurface.Clear();
            Game.Instance.UISurface.Print(2, 2, MouseOver);
            Game.Instance.UISurface.Print(2, 4, "Z = " + ZKey);
            Game.Instance.UISurface.Print(2, 6, "X = " + XKey);
            Game.Instance.UISurface.Print(2, 8, "C = " + CKey);
            Game.Instance.UISurface.Print(2, 15, DebugString);
            UpdateFloatingText(delta);
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

        List<FloatingText> floatingTexts = new List<FloatingText>();

        public void OverlaySurfaceEffect(string text, float timetoshow, Point pos, bool increaseHeight = true)
        {
            if (increaseHeight)
            {
                pos = new Point(pos.X, pos.Y - 1);
            }
            var floatingText = new FloatingText(text, pos, timetoshow);
            floatingTexts.Add(floatingText);
        }

        public void UpdateFloatingText(TimeSpan delta)
        {
            for (int i = floatingTexts.Count - 1; i >= 0; i--)
            {
                floatingTexts[i].Update(delta);
                if (floatingTexts[i].Lifetime <= 0)
                {
                    floatingTexts[i].Finish();
                    floatingTexts.RemoveAt(i);
                }
            }
        }

    }

    public class FloatingText
    {
        public Console Console { get; }
        public Point Position { get; set; }
        public float Lifetime { get; set; }

        public FloatingText(string text,  Point location, float lifetime)
        {
            Lifetime = lifetime;
            Console = new Console(text.Length, 1);
            Console.Position = location;
            Console.Print(0, 0, text,Color.White,Color.Black);
            Game.Instance.OverlaySurface.Children.Add(Console);
        }

        public void Update(TimeSpan delta)
        {
            if (Lifetime > 0)
            {
                Lifetime -= (float)delta.TotalSeconds;
            }
        }

        public void Finish()
        {
            Game.Instance.OverlaySurface.Children.Remove(Console);
            Console.Dispose();
        }
    }
    
}
