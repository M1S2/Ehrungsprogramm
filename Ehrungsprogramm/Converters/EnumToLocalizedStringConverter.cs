using System;
using System.Globalization;
using System.Resources;
using System.Windows.Data;

namespace Ehrungsprogramm.Converters
{
    /// <summary>
    /// Get a localized string based on the enum value to convert.
    /// The entries in the Resources file must have the format "Enum_{EnumType}_{EnumValue}"
    /// </summary>
    public class EnumToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumValue = (Enum)value;

            // https://stackoverflow.com/questions/17380900/enum-localization
            ResourceManager rm = new ResourceManager(typeof(Properties.Resources));
            string resourceDisplayName = rm.GetString("Enum_" + enumValue.GetType().Name + "_" + enumValue);
            return string.IsNullOrWhiteSpace(resourceDisplayName) ? string.Format("{0}", enumValue) : resourceDisplayName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
