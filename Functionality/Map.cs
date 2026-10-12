using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality
{
    public class Map
    {
        public List<GameEntity> Entities { get; set; } = new List<GameEntity>();
        public GameEntity Player;

        public void LoadMap(string mapPath)
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

            var startGameEvent = new GE_GameStart();
            foreach (var ent in Entities)
            {
                ent.ResolveGameEvent(startGameEvent);
            }
        }

        public void TriggerGlobalEvent(GameEvent gameEvent)
        {
            foreach (var ent in Entities)
            {
                ent.ResolveGameEvent(gameEvent);
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
            if (charatpt == 'g')
            {
                CreateEntity("Creatures", "Goblin", x, y);
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

        public List<GameEntity>? GetEntitiesAtPosition(int x, int y)
        {
            if (x < 0 || x >= GameSettings.GAME_WIDTH || y < 0 || y >= GameSettings.GAME_HEIGHT)
            {
                return null;
            }

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

    }
}
