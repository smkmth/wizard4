using System;
using System.Collections.Generic;
using System.Text;
using Wizard5.Functionality.Component;

namespace Wizard5.Functionality
{

    public enum FOVType
    {
        Render,
        Light,
        Sight
    }

    public struct FOVSubscriber
    {
        public FOVType Type;

        public static int FOVIDCounter = 0;

        public int FOVID;
        public int Radius;

        public int Falloff1;
        public int Falloff2;

        public (int,int) OldPos =(-1,-1);
        public GameEntity Entity;

        public FOVSubscriber(int radius, GameEntity entity, FOVType type)
        {
            Radius = radius;
            FOVID = FOVIDCounter++;
            Entity = entity;
            Type = type;
            Falloff1 = 0;
            Falloff1 = 1;

        }
    }



    public class FOV
    {


        public FOVMap FOVMap;
        public FieldOfView LightFOV;

        public List<FOVSubscriber> FOVSubscribers = new List<FOVSubscriber>();

        public void LoadFOV()
        {
            FOVMap = new FOVMap();
            foreach (var ents in Game.Instance.Map.Entities)
            {
                if (ents.HasComponent<Opaque>())
                {
                    var pos = ents.GetPosition();
                    FOVMap.GetCell(pos.Item1, pos.Item2).transparent = false;
                }
            }
            foreach (Cell cell in FOVMap.cells)
            {
                cell.light = 0;
            }

            LightFOV = new FieldOfView(FOVMap);

        }


        public void CalculateFOVs()
        {
            LightFOV.ClearFov();
            foreach (var cell in FOVMap.cells)
            {
                cell.light = 0;
            }
            for(int i = 0; i< FOVSubscribers.Count; i++)
            {
                var light = FOVSubscribers[i];
                if (light.Type == FOVType.Light)
                {
                    var pos = light.Entity.GetPosition();
                    if (light.OldPos != pos)
                    {
                        light.OldPos = pos;
                        LightFOV.AppendFov(pos.Item1, pos.Item2, light.Radius, true, light.Falloff1, light.Falloff2);
                    }
                }
            }

            Game.Instance.UnderlaySurface.Clear();
      
            foreach (var cell in FOVMap.cells)
            {
                Game.Instance.UnderlaySurface.SetBackground(cell.x, cell.y, Color.DarkBlue * cell.light);
            }


        }

        public int SubscribeToFOV(int radius, FOVType type, GameEntity ent)
        {
            var sub = new FOVSubscriber(radius, ent, type);
            FOVSubscribers.Add(sub);
            return sub.FOVID;
        }


        public void UnSubscribeToFOV(int fOVID)
        {
            foreach (var sub in FOVSubscribers)
            {
                if (sub.FOVID == fOVID)
                {
                    FOVSubscribers.Remove(sub);
                    break;
                }
            }
        }



    }



}
