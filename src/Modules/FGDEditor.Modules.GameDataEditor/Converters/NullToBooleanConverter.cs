using System;
using System.Globalization;
using System.Windows.Data;

namespace FGDEditor.Modules.GameDataEditor.Converters
{
    /// <summary>
    /// Converts object references to boolean values
    /// </summary>
    public sealed class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(value is null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
