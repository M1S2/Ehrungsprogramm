using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ehrungsprogramm.Converters
{
    /// <summary>
    /// Converter that is returning the result of logical OR of multiple values as visiblity
    /// true -> visible
    /// false -> collapsed
    /// </summary>
    /// https://stackoverflow.com/questions/945427/c-sharp-wpf-isenabled-using-multiple-bindings
    public class BooleanOrVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Any(v => v as bool? == true) ? Visibility.Visible : Visibility.Collapsed;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
