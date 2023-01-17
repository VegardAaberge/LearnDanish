using System;
using System.Globalization;
using Xamarin.Forms;

namespace SpeakDanish.Converters
{
	public class PercentageConverter : IValueConverter
    {
		public PercentageConverter()
		{
		}

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double valueDouble && parameter is string parameterString)
            {
                double parameterDouble;
                if (valueDouble < 0)
                    return 0;

                if (double.TryParse(parameterString, NumberStyles.Number, CultureInfo.InvariantCulture, out parameterDouble))
                {
                    return valueDouble * parameterDouble;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

