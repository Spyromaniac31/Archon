using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace Archon.Helpers
{
    public class StringToBooleanConverter : IValueConverter
    {
        //This does the string to bool conversion
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return System.Convert.ToBoolean((string)value);
            }
            catch
            {
                Debug.WriteLine("Error converting " + value + " to boolean");
                //True is much rarer as a default value, so it's easier to identify if something goes wrong
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
