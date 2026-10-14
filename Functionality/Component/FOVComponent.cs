using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using static SadConsole.UI.Controls.Table.TableCells.Layout;

namespace Wizard5.Functionality.Component
{
    public class FOVComponent : IGameComponent
    {

        private int Radius;
        private bool FOVon =true;
        public FOVType FovType;

        private int FOVID = -1;

        public FOVComponent(GameEntity owner, int radius, FOVType fovType)
        {
            Owner = owner;
            Radius = radius;
            FovType = fovType;
        }

        public GameEntity Owner { get; set; }

        public string ComponentName { get; } = "FOVComponent";



        public IGameComponent Copy(GameEntity newOwner)
        {
            //dispose of old fov
            if (Owner != null )
            {
                Game.Instance.FOV.UnSubscribeToFOV(FOVID);
            }
            return new FOVComponent(newOwner, Radius, FovType) { };
        }

        private void ActivateFOV(bool on)
        {
            FOVon = on;
            if (on)
            {
                FOVID = Game.Instance.FOV.SubscribeToFOV(Radius, FovType, Owner);
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
                if (FOVon)
                {
                    ActivateFOV(true);
                }
            }
            if (gameEvent is GE_Death)
            {
                if (FOVon)
                {
                    ActivateFOV(false);
                }
            }
            if (FovType== FOVType.Sight && gameEvent is GE_CheckSight)
            {
                foreach (var cells in Game.Instance.FOV.EnemyFOV[FOVID].CellsInFov())
                {
                    foreach (var ent in Game.Instance.Map.GetEntitiesAtPosition(cells.x, cells.y))
                    {
                        if (ent == Game.Instance.Map.Player)
                        {
                            Game.Instance.UI.OverlaySurfaceEffect("!", 1.0f, Owner.GetPosition());

                        }
                    }

                }

            }
         

        }
    }
}
