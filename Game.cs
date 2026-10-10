using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;

namespace Wizard5
{
    public class Game
    {

        public List<GameEntity> Entities { get; set; } = new List<GameEntity>();    

        public Game()
        {
            var player = new GameEntity();
            player.Components.Add(new Transform(player));
            player.Components.Add(new Renderer(player, Color.White, Color.Transparent, '@'));
            player.ResolveGameEvent(new GE_SetPosition(10, 10));
            Entities.Add(player);

        }

        public void Draw(ScreenSurface mainSurface)
        {
            var drawEvent = new GE_Draw(mainSurface);
            foreach (var entity in Entities)
            {
                entity.ResolveGameEvent(drawEvent);
            }   
        }
    }
}
