using System;
using System.Globalization;
using Xamarin.Forms;

namespace SpeakDanish.Converters
{
    public class BoolAppResourcesWithDataConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 4)
                return "";

            if(values[0] is bool picker
                && values[1]?.ToString() is string data
                && values[2] is string option1
                && values[3] is string option2)
            {
                var option = picker ? option1 : option2;

                return string.Format(option, data);
            }

            return "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

