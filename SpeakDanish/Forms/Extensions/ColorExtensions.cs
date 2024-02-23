using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace SpeakDanish.Extensions
{
	public static class ColorExtensions
	{
        public static Color Darker(this Color color, float mult)
        {
            return Color.FromRgba(color.Red * mult, color.Green * mult, color.Blue * mult, color.Alpha);
        }
    }
}

