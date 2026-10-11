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
            Instance = this;

            LoadMap("maps/map2");

            var startGameEvent = new GE_GameStart();
            foreach (var ent in Entities)
            {
                ent.ResolveGameEvent(startGameEvent);
            }

        }

        private void LoadMap(string mapPath)
        {

            if (File.Exists(mapPath))
            {
                // Reads all text into a single string
                string[] lines = File.ReadAllLines(mapPath);
                for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
                {

                    for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
                    {
                        var charatpt = lines[y][x];
                        PlaceObjectAtPlace(y, x, charatpt);
                    }
                }
            }
        }

        private void PlaceObjectAtPlace(int y, int x, char charatpt)
        {
            if (charatpt == '#')
            {
                CreateEntity("Items", "Wall", x, y);
            }
            if (charatpt == '@')
            {
                CreateEntity("Creatures", "Player", x, y);
            }
        }

        private void CreateEntity(string directory, string name, int x, int y)
        {

            var gameEntity = YamlDataReader.CreateEntityCached(directory, name);
            PlaceEntity(gameEntity, x, y);

        }

        public void PlaceEntity(GameEntity entity, int x, int y)
        {
            entity.ResolveGameEvent(new GE_SetPosition(x, y));
            Entities.Add(entity);
        }

        public void DestroyEntity(GameEntity entity)
        {
            Entities.Remove(entity);

        }

       public List<GameEntity> GetEntitiesAtPosition(int x, int y)
       {
            var entities = new List<GameEntity>();
            var checkEntityEvent = new GE_RequestPosition();
            foreach (var entity in Entities)
            {
                var pos = entity.GetPosition();
                if (pos.Item1 == x && pos.Item2 == y)
                {
                    entities.Add(entity);
                }
            }

            return entities;
       }

        public void Draw(ScreenSurface mainSurface)
        {
            var drawEvent = new GE_Draw(mainSurface);
            foreach (var entity in Entities)
            {
                entity.ResolveGameEvent(drawEvent);
            }
            Player.ResolveGameEvent(drawEvent);//lazy just draw player twice
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
