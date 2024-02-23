using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace SpeakDanish.Converters
{
	public class SizePercentageConverter : IMultiValueConverter
    {
		public SizePercentageConverter()
		{
		}

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return 0;

            if (values[0] is double width && values[1] is double height && parameter is string parameterString)
            {
                double parameterDouble;
                if (width < 0 || height < 0)
                    return 0;

                if (double.TryParse(parameterString, NumberStyles.Number, CultureInfo.InvariantCulture, out parameterDouble))
                {
                    return Math.Min(height, width) * parameterDouble;
                }
            }

            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

