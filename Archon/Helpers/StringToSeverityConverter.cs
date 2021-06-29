using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Data;

namespace Archon.Helpers
{
    public class StringToSeverityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string severityValue)
            {
                bool valueIsSeverity = Enum.TryParse(typeof(InfoBarSeverity), severityValue, out object severity);

                if (valueIsSeverity)
                {
                    return (InfoBarSeverity)severity;
                }
                throw new ArgumentException("Value must be an InfoBar severity!");
            }

            throw new ArgumentException("paramter must be an Enum name!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }
    }
}
