using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ehrungsprogramm.Converters
{
    public class NumericComparisonGreaterToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int comparisonValue = 0;
            if (int.TryParse(parameter as string, out comparisonValue))
            {
                return (((int)value) > comparisonValue) ? Visibility.Visible : Visibility.Hidden;
            }

            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
