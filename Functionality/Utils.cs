using System;
using System.Collections.Generic;
using System.Text;

namespace Wizard5.Functionality
{
    public static class ColorUtils
    {
        /// <summary>
        /// Linearly interpolates (Lerps) between two RGBA colors.
        /// </summary>
        /// <param name="colorA">The starting color.</param>
        /// <param name="colorB">The ending color.</param>
        /// <param name="t">The interpolation factor, clamped between 0.0 and 1.0.</param>
        /// <returns>The blended RGBA Color.</returns>
        public static Color Lerp(Color colorA, Color colorB, float t)
        {
            // Clamp t between 0.0 and 1.0 to ensure correct interpolation boundaries
            t = Math.Max(0f, Math.Min(1f, t));

            // Perform linear interpolation for each RGBA channel independently
            int a = (int)(colorA.A + (colorB.A - colorA.A) * t);
            int r = (int)(colorA.R + (colorB.R - colorA.R) * t);
            int g = (int)(colorA.G + (colorB.G - colorA.G) * t);
            int b = (int)(colorA.B + (colorB.B - colorA.B) * t);

            // Return the newly constructed color
            return new Color(a, r, g, b);
        }
    }
}
