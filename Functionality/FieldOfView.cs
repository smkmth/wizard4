using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.CompilerServices;
    using Wizard5.Functionality.Component;
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

    public class FOVMap
    {
        public Cell[] cells = new Cell[GameSettings.GAME_HEIGHT * GameSettings.GAME_WIDTH];
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

                if (GetCell(xOrigin, yOrigin).IsWalkable() == false)
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

    /// <summary>
    /// A class for performing field-of-view calculations to determine what is observable in a Map from a given Cell within a given light radius
    /// </summary>
    /// <seealso href="https://sites.google.com/site/jicenospam/visibilitydetermination">Based on the visibility determination algorithm described here</seealso>
    public class FieldOfView
    {
        private readonly HashSet<int> _inFov;
        private readonly FOVMap _map;
        /// <summary>
        /// Constructs a new FieldOfView class for the specified Map
        /// </summary>
        /// <param name="map">The Map that this FieldOfView class will use to perform its field-of-view calculations</param>
        public FieldOfView(FOVMap map)
        {
            _inFov = new HashSet<int>();
        }

        internal FieldOfView(FOVMap map, HashSet<int> inFov)
        {
            _inFov = inFov;
        }

        public float InverseLerp(float a, float b, float v)
        {
            return (v - a) / (b - a);
        }

        /// <summary>
        /// Create and return a deep copy of an existing FieldOfView class
        /// </summary>
        /// <returns>A deep copy of an existing FieldOfViewClass</returns>
        public FieldOfView Clone()
        {
            var inFovCopy = new HashSet<int>();
            foreach (int i in _inFov)
            {
                inFovCopy.Add(i);
            }
            return new FieldOfView(_map, inFovCopy);
        }

        /// <summary>
        /// Check if the Cell is in the currently computed field-of-view
        /// Field-of-view must first be calculated by calling ComputeFov and/or AppendFov
        /// </summary>
        /// <remarks>
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius
        /// </remarks>
        /// <example>
        /// Field-of-view can be used to simulate a character holding a light source and exploring a Map representing a dark cavern
        /// Any Cells within the FOV would be what the character could see from their current location and lighting conditions
        /// </example>
        /// <param name="x">X location of the Cell to check starting with 0 as the farthest left</param>
        /// <param name="y">Y location of the Cell to check, starting with 0 as the top</param>
        /// <returns>True if the Cell is in the currently computed field-of-view, false otherwise</returns>
        public bool IsInFov(int x, int y)
        {
            return _inFov.Contains(_map.IndexFor(x, y));
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius.
        /// Any existing field-of-view calculations will be overwritten when calling this method.
        /// </summary>
        /// <param name="xOrigin">X location of the Cell to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Cell to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Cells in which the field-of-view extends from the origin Cell. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        /// <returns>List of Cells representing what is observable in the Map based on the specified parameters</returns>
        public void ComputeFov(int xOrigin, int yOrigin, int radius, bool calcLight = false, float falloff = 0.3f)
        {
            ClearFov();
            AppendFov(xOrigin, yOrigin, radius, calcLight, falloff);
        }

        /// <summary>
        /// Performs a field-of-view calculation with the specified parameters and appends it any existing field-of-view calculations.
        /// Field-of-view (FOV) is basically a calculation of what is observable in the Map from a given Cell with a given light radius.
        /// </summary>
        /// <example>
        /// When a character is holding a light source in a large area that also has several other sources of light such as torches along the walls
        /// ComputeFov could first be called for the character and then AppendFov could be called for each torch to give us the final combined FOV given all the light sources
        /// </example>
        /// <param name="xOrigin">X location of the Cell to perform the field-of-view calculation with 0 as the farthest left</param>
        /// <param name="yOrigin">Y location of the Cell to perform the field-of-view calculation with 0 as the top</param>
        /// <param name="radius">The number of Cells in which the field-of-view extends from the origin Cell. Think of this as the intensity of the light source.</param>
        /// <param name="lightWalls">True if walls should be included in the field-of-view when they are within the radius of the light source. False excludes walls even when they are within range.</param>
        /// <returns>List of Cells representing what is observable in the Map based on the specified parameters</returns>
        public void AppendFov(int xOrigin, int yOrigin, int radius, bool calcLight, float falloff = 0.3f, float atin = .8f)
        {
            //todo optimisation NOT ALL CELLS MATTER here - some are out of bounds

            foreach (Cell borderCell in _map.GetBorderCellsInSquare(xOrigin, yOrigin, radius))
            {
                foreach (Cell cell in _map.GetCellsAlongLine(xOrigin, yOrigin, borderCell.x, borderCell.y))
                {
                    float dist = (Math.Abs(cell.x - xOrigin) + Math.Abs(cell.y - yOrigin));
                    if (dist > radius)
                    {
                        break;
                    }
                    if (calcLight)
                    {

                        cell.light += 1 / (1.0f + falloff * dist + atin * dist * dist);

                        if (cell.light > 1.5f)
                        {
                            cell.light = 1.5f;
                        }

                    }

                    if (cell.IsTransparent())
                    {
                        _inFov.Add(_map.IndexFor(cell));
                    }
                    else
                    {

                        _inFov.Add(_map.IndexFor(cell));

                        break;
                    }
                }
            }

        }

        public ReadOnlyCollection<Cell> CellsInFov()
        {
            var cells = new List<Cell>();
            foreach (int index in _inFov)
            {
                cells.Add(_map.CellFor(index));
            }
            return new ReadOnlyCollection<Cell>(cells);
        }

        private void ClearFov()
        {
            _inFov.Clear();
        }

        private void PostProcessFovQuadrant(int x, int y, Quadrant quadrant)
        {
            int x1 = x;
            int y1 = y;
            int x2 = x;
            int y2 = y;
            switch (quadrant)
            {
                case Quadrant.NE:
                    {
                        y1 = y + 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SE:
                    {
                        y1 = y - 1;
                        x2 = x - 1;
                        break;
                    }
                case Quadrant.SW:
                    {
                        y1 = y - 1;
                        x2 = x + 1;
                        break;
                    }
                case Quadrant.NW:
                    {
                        y1 = y + 1;
                        x2 = x + 1;
                        break;
                    }
            }
            if (!IsInFov(x, y) && _map.GetCell(x, y).IsTransparent() == false)
            {
                if ((_map.GetCell(x1, y1).IsTransparent() && IsInFov(x1, y1)) || (_map.GetCell(x2, y2).IsTransparent() && IsInFov(x2, y2))
                        || (_map.GetCell(x2, y1).IsTransparent() && IsInFov(x2, y1)))
                {
                    _inFov.Add(_map.IndexFor(x, y));
                }
            }
        }

        private enum Quadrant
        {
            NE = 1,
            SE = 2,
            SW = 3,
            NW = 4
        }
    }

}
