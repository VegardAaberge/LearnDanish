using System;
using System.Globalization;
using SpeakDanish.Extensions;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace SpeakDanish.Converters
{
	public class DisabledColorConverter : IValueConverter
	{
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is Boolean enabled && !enabled){
                return Colors.LightGray;
            }

            return parameter;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

