using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;


namespace TextEditor.Converters
{
    internal class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string colorString && !string.IsNullOrEmpty(colorString))
            {
                try
                {
                    return (SolidColorBrush)(new BrushConverter().ConvertFromString(colorString));
                }
                catch
                {
                    return Brushes.Black;
                }
            }
            return Brushes.Black;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
