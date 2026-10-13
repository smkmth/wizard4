using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;
using System;
using System.IO;
using System.Linq.Expressions;
using Wizard5.Functionality;

namespace Wizard5
{
    public class Game
    {
        public static Game Instance;
        public Map Map;
        public UI UI;
        public FOV FOV;
        public ScreenSurface MapSurface;
        public ScreenSurface UISurface;
        public ScreenSurface OverlaySurface;
        public ScreenSurface UnderlaySurface;


        public Game(ScreenSurface mapSurface, ScreenSurface uiSurface, ScreenSurface overlaySurface, ScreenSurface underlaySurface)
        {
            Instance = this;
            Map = new Map();
            UI = new UI();
            FOV = new FOV();
            Map.LoadMap(Map.CurrentMap);
            MapSurface = mapSurface;
            UISurface = uiSurface;
            OverlaySurface = overlaySurface;
            UnderlaySurface = underlaySurface;
            EndTurn();
        }


        public void Draw(TimeSpan delta)
        {
            MapSurface.Clear();
            Map.Draw(delta);
            UI.Draw(delta);
        }


        public void EndTurn()
        {

            foreach (var ent in Map.Entities)
            {
                ent.ResolveGameEvent(new GE_TurnEnd());
                
            }
            FOV.CalculateFOVs();
        }

        public void ProcessKeyboard(Keyboard keyboard)
        {
            foreach(var key in keyboard.KeysPressed)
            {
                 
                if ((key.Key == Keys.Left))
                {
                    Map.Player?.ResolveGameEvent(new GE_Move(-1, 0));

                }
                if ((key.Key == Keys.Down))
                {
                    Map.Player?.ResolveGameEvent(new GE_Move(0, 1));

                }
                if ((key.Key == Keys.Right))
                {
                    Map.Player?.ResolveGameEvent(new GE_Move(1, 0));

                }
                if ((key.Key == Keys.Up))
                {
                    Map.Player?.ResolveGameEvent(new GE_Move(0, -1));

                }
                if ((key.Key == Keys.Z))
                {   
                    Map.Player?.ResolveGameEvent(new GE_TriggerAbilityKey('z'));
                }
                if ((key.Key == Keys.X))
                {
                    Map.Player?.ResolveGameEvent(new GE_TriggerAbilityKey('x'));
                }
                if ((key.Key == Keys.C))
                {
                    Map.Player?.ResolveGameEvent(new GE_TriggerAbilityKey('c'));
                }
            }
        }

        Action<GameEntity> FoundTarget = null;

        public void RequestTarget(Action<GameEntity> foundTarget)
        {
            FoundTarget = foundTarget;
        }

        internal void ProcessMouse(MouseScreenObjectState state)
        {
            var ents = Map.GetEntitiesAtPosition(state.SurfaceCellPosition.X, state.SurfaceCellPosition.Y);
            if (ents != null)
            {
                if (ents.Count == 0)
                {
                    UI.MouseOver = state.SurfaceCellPosition.ToString() + " Floor";
                    return;
                }
                string entNames = "";   
                foreach (var ent in ents)
                {
                    entNames += ent.Name + " "; 
                }
                UI.MouseOver = state.SurfaceCellPosition.ToString() + " " +  entNames;

                if (state.Mouse.LeftClicked) 
                {
                    if (ents.Count > 0)
                    {
                      
                        FoundTarget?.Invoke(ents[0]);  //deal with stacks of targets later
                        FoundTarget = null;
                        
                    }
                }
            }
            else
            {
                UI.MouseOver = UI.DefaultMouseText;
            }
        }

        internal void ProcessUIMouse(MouseScreenObjectState args)
        {
            UI.MouseOver = UI.DefaultMouseText;

        }
    }
}
