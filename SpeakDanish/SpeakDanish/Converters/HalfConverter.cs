using System;
using System.Globalization;
using Xamarin.Forms;

namespace SpeakDanish.Converters
{
	public class HalfConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is double doubleValue)
            {
                return doubleValue / 2f;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

