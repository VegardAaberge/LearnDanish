using System;
using System.Globalization;
using Xamarin.Forms;

namespace SpeakDanish.Converters
{
	public class StringToBoolConverter : IValueConverter
    {
		public StringToBoolConverter()
		{
		}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            return value.ToString().Trim().Length > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

