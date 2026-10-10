using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5
{
    public class Game
    {

        public List<GameEntity> Entities { get; set; } = new List<GameEntity>();    

        Game()
        {

        }

        public void Draw(ScreenSurface mainSurface)
        {
            foreach (var entity in Entities)
            {
                if (entity.Renderer != null)
                {
                    entity.Renderer.Draw(mainSurface);
                }
            }   
        }
    }
}
