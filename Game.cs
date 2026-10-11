using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;
using System;
using System.IO;
using System.Linq.Expressions;

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
                for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
                {

                    for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
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

            Player = CreateGenericEntity(Color.White, Color.Transparent, '@');
            Player.ResolveGameEvent(new GE_SetPosition(x, y));
            Player.Components.Add(new Health(Player, 10, CreateGenericEntity(Color.Red, Color.Transparent, '\'')));
            Player.Components.Add(new Attack(Player, 3));

            Entities.Add(Player);

        }

        private void CreateWall(int x, int y)
        {
            var otherEnt = CreateGenericEntity(Color.White, Color.Transparent, '#');
            otherEnt.ResolveGameEvent(new GE_SetPosition(x, y));

            var wallcorpse = new GameEntity();
            wallcorpse.Components.Add(new Transform(wallcorpse));
            wallcorpse.Components.Add(new Renderer(wallcorpse, Color.White, Color.Transparent, '\''));

            otherEnt.Components.Add(new Health(otherEnt, 10, wallcorpse));
            Entities.Add(otherEnt);
        }

        public GameEntity CreateGenericEntity(Color color, Color bgColor,char symbol = '#')
        {
            var ent = new GameEntity();
            ent.Components.Add(new Transform(ent));
            ent.Components.Add(new Renderer(ent, color, bgColor, symbol));
            ent.Components.Add(new Collider(ent));
            return ent;
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

        public void CreateEntity(GameEntity entity, int x, int y)
        {
            entity.ResolveGameEvent(new GE_SetPosition(x, y));
            Entities.Add(entity);
        }

        public void DestroyEntity(GameEntity entity)
        {
            Entities.Remove(entity);    

        }
    }
}
