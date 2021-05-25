using System;
using Windows.UI.Xaml.Data;

namespace Archon.Helpers
{
    public class StringToDoubleConverter : IValueConverter
    {
        //This does the string to double conversion
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return System.Convert.ToDouble((string)value);
            }
            catch
            {
                return 4.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
