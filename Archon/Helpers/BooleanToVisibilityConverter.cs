using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Archon.Helpers
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        //This does the bool to visibility conversion
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool visible)
            {
                if (visible)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            throw new ArgumentException("Value must be a boolean");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                if (visibility == Visibility.Visible)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            throw new ArgumentException("Value must be a Visibility");
        }
    }
}
