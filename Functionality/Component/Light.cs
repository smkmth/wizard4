using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality.Component
{
    public class Light : IGameComponent
    {

        private int Radius;
        private bool Lit;

        private int FOVID = -1;

        public Light(GameEntity owner, int radius, bool lit)
        {
            Owner = owner;
            Radius = radius;
            Lit = lit;
          
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "Light";



        public IGameComponent Copy(GameEntity newOwner)
        {
            //dispose of old fov
            if (Owner != null )
            {
                Game.Instance.FOV.UnSubscribeToFOV(FOVID);
            }
            return new Light(newOwner, Radius, Lit) { };
        }

        private void ActivateLight(bool lit)
        {
            Lit = lit;
            if (lit)
            {
                FOVID = Game.Instance.FOV.SubscribeToFOV(Radius, FOVType.Light, Owner);
            }
            else
            {
                Game.Instance.FOV.UnSubscribeToFOV(FOVID);
                FOVID = -1;
            }
        }

        public void ReceiveEvent(GameEvent gameEvent)
        {
            if (gameEvent is GE_GameStart) 
            {
                if (Lit)
                {
                    ActivateLight(true);
                }
            }
            if (gameEvent is GE_Death)
            {
                if (Lit)
                {
                    ActivateLight(false);
                }
            }



        }
    }
}
