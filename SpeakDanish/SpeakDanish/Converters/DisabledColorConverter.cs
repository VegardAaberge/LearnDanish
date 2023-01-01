using System;
using System.Globalization;
using SpeakDanish.Extensions;
using Xamarin.Forms;

namespace SpeakDanish.Converters
{
	public class DisabledColorConverter : IValueConverter
	{
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Boolean enabled && !enabled){
                return Color.LightGray;
            }

            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

