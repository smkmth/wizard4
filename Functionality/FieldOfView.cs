using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security;
    using Wizard5.Functionality.Component;

 

    /// <summary>
    /// A class for performing field-of-view calculations to determine what is observable in a Map from a given Cell within a given light radius
    /// </summary>
    /// <seealso href="https://sites.google.com/site/jicenospam/visibilitydetermination">Based on the visibility determination algorithm described here</seealso>
    public class FieldOfView
    {
        private readonly HashSet<int> _inFov;
        private readonly FOV _map;
        /// <summary>
        /// Constructs a new FieldOfView class for the specified Map
        /// </summary>
        /// <param name="map">The Map that this FieldOfView class will use to perform its field-of-view calculations</param>
        public FieldOfView(FOV map)
        {
            _map = map;
            _inFov = new HashSet<int>();
        }

        internal FieldOfView(FOV map, HashSet<int> inFov)
        {
            _map = map;
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

            var startcell = _map.GetCell(xOrigin, yOrigin);
            var wastransparent = startcell.transparent;
            startcell.transparent = true;
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

                    if (cell.transparent)
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
            startcell.transparent = wastransparent;


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

        public void ClearFov()
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
            if (!IsInFov(x, y) && _map.GetCell(x, y).transparent == false)
            {
                if ((_map.GetCell(x1, y1).transparent && IsInFov(x1, y1)) || (_map.GetCell(x2, y2).transparent && IsInFov(x2, y2))
                        || (_map.GetCell(x2, y1).transparent && IsInFov(x2, y1)))
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
