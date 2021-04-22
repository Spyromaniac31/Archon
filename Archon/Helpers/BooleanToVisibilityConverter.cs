using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Archon.Helpers
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        //This does the bool to visibility conversion
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is bool visible
                ? visible ? Visibility.Visible : Visibility.Collapsed
                : throw new ArgumentException("Value must be a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility visibility
                ? visibility == Visibility.Visible
                : throw new ArgumentException("Value must be a Visibility");
        }
    }
}
