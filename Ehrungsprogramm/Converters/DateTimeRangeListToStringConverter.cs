using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Ehrungsprogramm.Core.Models;

namespace Ehrungsprogramm.Converters
{
    /// <summary>
    /// Convert a list of DateTimeRanges to a string representation
    /// </summary>
    public class DateTimeRangeListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnVal = "";
            List<DateTimeRange> dateTimeRanges = value as List<DateTimeRange>;
            foreach(DateTimeRange range in dateTimeRanges)
            {
                returnVal += range?.Start.ToString("d") + " - " + range?.End.ToString("d") + "; ";
            }
            returnVal = returnVal.Trim(' ').Trim(';');
            return returnVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
