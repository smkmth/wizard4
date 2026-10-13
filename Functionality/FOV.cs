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


        public GameEntity Entity;

        public FOVSubscriber(int radius, GameEntity entity, FOVType type)
        {
            Radius = radius;
            FOVID = FOVIDCounter++;
            Entity = entity;
            Type = type;
            
        }
    }

 
    public class Cell
    {
        public bool IsTransparent()
        {
            return true;
        }

        public bool IsWalkable()
        {
            return true;
        }

        public float light;
        public int x;
        public int y;

    }

    public class FOV
    {

        public Cell[,] Cells { get; set; }

        public List<FOVSubscriber> FOVSubscribers { get; set; } = new List<FOVSubscriber>();

        HashSet<(int, int)> opaquePositions = new HashSet<(int, int)>();
   
        public void LoadFOV()
        {
            Cells = new Cell[GameSettings.GAME_HEIGHT, GameSettings.GAME_WIDTH];
            for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
            {
                for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
                {
                    Cells[y, x] = new Cell();
                }
            }

            foreach (var ent in Game.Instance.Map.Entities)
            {
                if (ent.HasComponent<Opaque>())
                {
                    opaquePositions.Add(ent.GetPosition());
                }

            }

        }


        public void CalculateFOVs()
        {
            ClearAllVisibility();
       
            Game.Instance.UnderlaySurface.Fill(Color.Transparent,Color.Blue);
            foreach (var sub in FOVSubscribers)
            {
                CalculateFOV(sub, opaquePositions);
            }
            foreach (var cell in LightVisibility)
            {
                Game.Instance.UnderlaySurface.SetBackground(cell.X, cell.Y, Color.LightYellow);
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

        public bool CanISee(int x, int y, int fovID)
        {
            return Cells[x, y].CanEntitySee(fovID);
        }

        public List<Point> GetVisibleCells(int fovID)
        {
            var viscells = new List<Point>();
            for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
            {
                for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
                {
                    if (Cells[y, x].CanEntitySee(fovID))
                    {
                        viscells.Add(new Point(x, y));
                    }
                }
            }
            return viscells;
        }
        public List<Point> LightVisibility { get; set; } = new List<Point>();


        public void CalculateFOV(FOVSubscriber sub, HashSet<(int, int)> opaquePositions)
        {
            var pt = sub.Entity.GetPosition();
            var sourceY = pt.Item1;
            var sourceX = pt.Item2;

            Cells[sourceY, sourceX].SetVisible(sub.FOVID, true);
            for (int y = Math.Max(0, sourceY - sub.Radius); y <= Math.Min(GameSettings.GAME_HEIGHT - 1, sourceY + sub.Radius); y++)
            {
                for (int x = Math.Max(0, sourceX - sub.Radius); x <= Math.Min(GameSettings.GAME_WIDTH - 1, sourceX + sub.Radius); x++)
                {
                    // Skip the source cell (already marked visible)
                    if (x == sourceX && y == sourceY)
                        continue;

                    // Check distance
                    int dx = x - sourceX;
                    int dy = y - sourceY;
                    int distSquared = dx * dx + dy * dy;
                    if (distSquared > sub.Radius * sub.Radius)
                        continue;

                    // Check line of sight using Bresenham's line algorithm
                    bool canSee = IsLineOfSightClear(sourceX, sourceY, x, y, opaquePositions);
                    if (sub.Type == FOVType.Light)
                    {
                        if (canSee)
                        {
                            LightVisibility.Add((y,x));
                        }

                    }
                    else
                    {
                        Cells[y, x].SetVisible(sub.FOVID, canSee);
                    }
                }
            }
        }

    
        private bool IsLineOfSightClear(int x0, int y0, int x1, int y1, HashSet<(int, int)> opaquePositions)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            int x = x0;
            int y = y0;

            while (true)
            {
                // Check if this position is blocked (but allow the target position)
                if ((x != x0 || y != y0) && (x != x1 || y != y1) && opaquePositions.Contains((x, y)))
                {
                    return false; // Line of sight blocked
                }

                // Reached target
                if (x == x1 && y == y1)
                {
                    return true;
                }

                // Bresenham's line algorithm
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }
        }

        public void ClearAllVisibility()
        {
            LightVisibility.Clear();
            for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
            {
                for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
                {
                    Cells[y, x].ClearVisibility();
                }
            }
        }
    }
}
