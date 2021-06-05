using System;
using System.Diagnostics;
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
                //For testing purposes. It seems like sometimes boolean strings will be converted here as the GameSettings are moved around during categorization,
                //but everything works out eventually
                Debug.WriteLine("Error converting " + value + " to double");
                if (value is string sValue)
                {
                    switch (sValue.ToLower())
                    {
                        case "true":
                            return 1.0;
                        case "false":
                            return 0.0;
                        default:
                            return 3.0;
                    }
                }
                return 3.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
