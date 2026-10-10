using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;

namespace Wizard5
{
    public class Game
    {

        public List<GameEntity> Entities { get; set; } = new List<GameEntity>();
        public GameEntity Player;

        public Game()
        {
            Player = new GameEntity();
            Player.Components.Add(new Transform(Player));
            Player.Components.Add(new Renderer(Player, Color.White, Color.Transparent, '@'));
            Player.ResolveGameEvent(new GE_SetPosition(10, 10));
            Entities.Add(Player);


        }

        public void Draw(ScreenSurface mainSurface)
        {
            var drawEvent = new GE_Draw(mainSurface);
            foreach (var entity in Entities)
            {
                entity.ResolveGameEvent(drawEvent);
            }   
        }

       

        public void ProcessKeyboard(Keyboard keyboard)
        {
            foreach(var key in keyboard.KeysPressed)
            {
                if (key.Character == 'a')
                {
                    Player.ResolveGameEvent(new GE_Move(-1, 0));

                }
                if (key.Character == 's')
                {
                    Player.ResolveGameEvent(new GE_Move(0, 1));

                }
                if (key.Character == 'd')
                {
                    Player.ResolveGameEvent(new GE_Move(1, 0));

                }
                if (key.Character == 'w')
                {
                    Player.ResolveGameEvent(new GE_Move(0, -1));

                }
            }
        }
    }
}
