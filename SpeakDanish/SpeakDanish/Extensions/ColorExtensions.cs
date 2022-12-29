using System;
using Xamarin.Forms;

namespace SpeakDanish.Extensions
{
	public static class ColorExtensions
	{
        public static Color Darker(this Color color, float mult)
        {
            return Color.FromRgba(color.R * mult, color.G * mult, color.B * mult, color.A);
        }
    }
}

