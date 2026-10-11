using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;
using System;
using System.IO;

namespace Wizard5
{
    public class Game
    {
        public static Game Instance;

        public List<GameEntity> Entities { get; set; } = new List<GameEntity>();
        public GameEntity Player;

        public Game()
        {
            string filePath = "maps/map2";

            if (File.Exists(filePath))
            {
                // Reads all text into a single string
                string[] lines = File.ReadAllLines(filePath);
                for (int y = 0; y < lines.Length; y++)
                {

                    for (int x = 0; x < 50; x++)
                    {
                        var charpt = lines[y][x];
                        if (charpt == '#')
                        {
                            CreateWall(x,y);
                        }
                        if (charpt == '@')
                        {
                            CreatePlayer(x, y);
                        }
                    }
                }
            }

            Instance = this;

        }

        private void CreatePlayer(int x, int y)
        {

            Player = new GameEntity();
            Player.Components.Add(new Transform(Player));
            Player.Components.Add(new Renderer(Player, Color.White, Color.Transparent, '@'));
            Player.Components.Add(new Collider(Player));
            Player.ResolveGameEvent(new GE_SetPosition(x, y));
            Entities.Add(Player);

        }

        private void CreateWall(int x, int y)
        {
            var otherEnt = new GameEntity();
            otherEnt.Components.Add(new Transform(otherEnt));
            otherEnt.Components.Add(new Renderer(otherEnt, Color.White, Color.Transparent, '#'));
            otherEnt.Components.Add(new Collider(otherEnt));
            otherEnt.ResolveGameEvent(new GE_SetPosition(x, y));
            Entities.Add(otherEnt);
        }

        public void Draw(ScreenSurface mainSurface)
        {
            var drawEvent = new GE_Draw(mainSurface);
            foreach (var entity in Entities)
            {
                entity.ResolveGameEvent(drawEvent);
            }   
        }

       public List<GameEntity> GetEntitiesAtPosition(int x, int y)
       {
            var entities = new List<GameEntity>();
            var checkEntityEvent = new GE_RequestPosition();
            foreach (var entity in Entities)
            {
                var pos = (R_RequestPosition)entity.ResolveGameEvent(checkEntityEvent);
                if (pos.X == x && pos.Y == y)
                {
                    entities.Add(entity);
                }
            }

            return entities;
       }

        public void ProcessKeyboard(Keyboard keyboard)
        {
            foreach(var key in keyboard.KeysPressed)
            {
                 
                if ((key.Key == Keys.Left))
                {
                    Player.ResolveGameEvent(new GE_Move(-1, 0));

                }
                if ((key.Key == Keys.Down))
                {
                    Player.ResolveGameEvent(new GE_Move(0, 1));

                }
                if ((key.Key == Keys.Right))
                {
                    Player.ResolveGameEvent(new GE_Move(1, 0));

                }
                if ((key.Key == Keys.Up))
                {
                    Player.ResolveGameEvent(new GE_Move(0, -1));

                }
            }
        }

      
    }
}
