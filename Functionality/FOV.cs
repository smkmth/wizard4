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
            Falloff2 = 0;

        }
    }


    public class Cell
    {
        public float light;
        public int x;
        public int y;
        public bool transparent = true;

        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }


    public class FOV
    {

        public Cell[] cells = new Cell[GameSettings.GAME_HEIGHT * GameSettings.GAME_WIDTH];
        public FieldOfView LightFOV;
        public Dictionary<int,FieldOfView> EnemyFOV = new Dictionary<int, FieldOfView>();

        public List<FOVSubscriber> FOVSubscribers = new List<FOVSubscriber>();

        public FOV()
        {
            for (int x = 0; x < GameSettings.GAME_WIDTH; x++)
            {
                for (int y = 0; y < GameSettings.GAME_HEIGHT; y++)
                {
                    cells[IndexFor(x, y)] = new Cell(x, y);
                }
            }
        }



        public void LoadFOV()
        {
            foreach (var ents in Game.Instance.Map.Entities)
            {
                var opaque = ents.ResolveGameEvent(new GE_CheckOpaque(true)) as R_Opaque;
                if (opaque != null && opaque.isOpaque)
                {
                    var pos = ents.GetPosition();
                    GetCell(pos.X, pos.Y).transparent = false;
                }

            }
            foreach (Cell cell in cells)
            {
                cell.light = 0;
            }

            LightFOV = new FieldOfView(this);

        }


        public void CalculateFOVs()
        {
            LightFOV.ClearFov();
            foreach (var cell in cells)
            {
                cell.light = 0;
            }
            for(int i = 0; i< FOVSubscribers.Count; i++)
            {
                var sub = FOVSubscribers[i];
              
                var pos = sub.Entity.GetPosition();
                if (sub.OldPos != pos)
                {
                    sub.OldPos = pos;
                    if (sub.Type == FOVType.Light)
                    {
                        LightFOV.AppendFov(pos.X, pos.Y, sub.Radius, true);
                    }
                    else
                    {
                        EnemyFOV[sub.FOVID].ComputeFov(pos.X, pos.Y,sub.Radius);
                    }

                }
                
               
            }

        


        }

        public int SubscribeToFOV(int radius, FOVType type, GameEntity ent)
        {
            var sub = new FOVSubscriber(radius, ent, type);
            FOVSubscribers.Add(sub);
            if (type == FOVType.Sight)
            {
                EnemyFOV.Add(sub.FOVID, new FieldOfView(this));
            }
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
            EnemyFOV.Remove(fOVID);

        }



        private int ClampX(int x)
        {
            return (x < 0) ? 0 : (x > GameSettings.GAME_WIDTH - 1) ? GameSettings.GAME_WIDTH - 1 : x;
        }

        private int ClampY(int y)
        {
            return (y < 0) ? 0 : (y > GameSettings.GAME_HEIGHT - 1) ? GameSettings.GAME_HEIGHT - 1 : y;
        }
        public Cell GetCell(int x, int y)
        {
            return cells[IndexFor(x, y)];
        }

        private bool AddToHashSet(HashSet<int> hashSet, int x, int y, out Cell cell)
        {
            cell = GetCell(x, y);
            return hashSet.Add(IndexFor(cell));
        }

        private bool AddToHashSet(HashSet<int> hashSet, int x, int y, Cell centerCell, out Cell cell)
        {
            cell = GetCell(x, y);
            if (cell.Equals(centerCell))
            {
                return false;
            }

            return hashSet.Add(IndexFor(cell));
        }

        private bool AddToHashSet(HashSet<int> hashSet, Cell cell)
        {
            return hashSet.Add(IndexFor(cell));
        }

        public Cell CellFor(int index)
        {
            int x = index % GameSettings.GAME_WIDTH;
            int y = index / GameSettings.GAME_WIDTH;

            return GetCell(x, y);
        }

        /// <summary>
        /// Get an IEnumerable of Cells in a line from the Origin Cell to the Destination Cell
        /// The resulting IEnumerable includes the Origin and Destination Cells
        /// Uses Bresenham's line algorithm to determine which Cells are in the closest approximation to a straight line between the two Cells
        /// </summary>
        /// <param name="xOrigin">X location of the Origin Cell at the start of the line with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Origin Cell at the start of the line with 0 as the top</param>
        /// <param name="xDestination">X location of the Destination Cell at the end of the line with 0 as the farthest left</param>
        /// <param name="yDestination">Y location of the Destination Cell at the end of the line with 0 as the top</param>
        /// <returns>IEnumerable of Cells in a line from the Origin Cell to the Destination Cell which includes the Origin and Destination Cells</returns>
        public IEnumerable<Cell> GetCellsAlongLine(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                yield return GetCell(xOrigin, yOrigin);
                if (xOrigin == xDestination && yOrigin == yDestination)
                {
                    break;
                }
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }

        public IEnumerable<Cell> CastRay(int xOrigin, int yOrigin, int xDestination, int yDestination)
        {
            xOrigin = ClampX(xOrigin);
            yOrigin = ClampY(yOrigin);
            xDestination = ClampX(xDestination);
            yDestination = ClampY(yDestination);

            int dx = Math.Abs(xDestination - xOrigin);
            int dy = Math.Abs(yDestination - yOrigin);

            int sx = xOrigin < xDestination ? 1 : -1;
            int sy = yOrigin < yDestination ? 1 : -1;
            int err = dx - dy;
            bool first = true;
            while (true)
            {
                yield return GetCell(xOrigin, yOrigin);

                if (GetCell(xOrigin, yOrigin).transparent == false)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        break;
                    }
                }
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err = err - dy;
                    xOrigin = xOrigin + sx;
                }
                if (e2 < dx)
                {
                    err = err + dx;
                    yOrigin = yOrigin + sy;
                }
            }
        }



        /// <summary>
        /// Get an IEnumerable of outermost border Cells in a square area around the center Cell up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Cell with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Cell with 0 as the top</param>
        /// <param name="distance">The number of Cells to get in each direction from the center Cell</param>
        /// <returns>IEnumerable of outermost border Cells in a square area around the center Cell</returns>
        public IEnumerable<Cell> GetBorderCellsInSquare(int xCenter, int yCenter, int distance)
        {
            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(GameSettings.GAME_WIDTH - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(GameSettings.GAME_HEIGHT - 1, yCenter + distance);
            var borderCells = new List<Cell>();

            for (int x = xMin; x <= xMax; x++)
            {
                borderCells.Add(GetCell(x, yMin));
                borderCells.Add(GetCell(x, yMax));
            }
            for (int y = yMin + 1; y <= yMax - 1; y++)
            {
                borderCells.Add(GetCell(xMin, y));
                borderCells.Add(GetCell(xMax, y));
            }

            Cell centerCell = GetCell(xCenter, yCenter);
            borderCells.Remove(centerCell);

            return borderCells;
        }


        /// <summary>
        /// Get an IEnumerable of outermost border Cells in a diamond (Rhombus) shape around the center Cell up to the specified distance
        /// </summary>
        /// <param name="xCenter">X location of the center Cell with 0 as the farthest left</param>
        /// <param name="yCenter">Y location of the center Cell with 0 as the top</param>
        /// <param name="distance">The number of Cells to get in a distance from the center Cell</param>
        /// <returns>IEnumerable of outermost border Cells in a diamond (Rhombus) shape around the center Cell</returns>
        public IEnumerable<Cell> GetBorderCellsInDiamond(int xCenter, int yCenter, int distance)
        {
            var discovered = new HashSet<int>();

            int xMin = Math.Max(0, xCenter - distance);
            int xMax = Math.Min(GameSettings.GAME_WIDTH - 1, xCenter + distance);
            int yMin = Math.Max(0, yCenter - distance);
            int yMax = Math.Min(GameSettings.GAME_HEIGHT - 1, yCenter + distance);

            Cell centerCell = GetCell(xCenter, yCenter);
            if (AddToHashSet(discovered, xCenter, yMin, centerCell, out Cell cell))
            {
                yield return cell;
            }
            if (AddToHashSet(discovered, xCenter, yMax, centerCell, out cell))
            {
                yield return cell;
            }
            for (int i = 1; i <= distance; i++)
            {
                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
                {
                    yield return cell;
                }
                if (AddToHashSet(discovered, Math.Max(xMin, xCenter - i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
                {
                    yield return cell;
                }
                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Min(yMax, yCenter + distance - i), centerCell, out cell))
                {
                    yield return cell;
                }
                if (AddToHashSet(discovered, Math.Min(xMax, xCenter + i), Math.Max(yMin, yCenter - distance + i), centerCell, out cell))
                {
                    yield return cell;
                }
            }
        }

        public int IndexFor(Cell cell)
        {
            return (cell.y * GameSettings.GAME_WIDTH) + cell.x;
        }


        public int IndexFor(int x, int y)
        {
            return (y * GameSettings.GAME_WIDTH) + x;
        }
        public int IndexFor(Point p)
        {
            return (p.Y * GameSettings.GAME_WIDTH) + p.X;
        }


    }



}
