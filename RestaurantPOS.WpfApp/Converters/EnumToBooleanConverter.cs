using System.Globalization;
using System.Windows.Data;

namespace RestaurantPOS.WpfApp.Converters;

// Binds a RadioButton's IsChecked to one value of an enum-typed property.
public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString() == parameter?.ToString();
    }

    public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (bool)value ? Enum.Parse(targetType, (string)parameter!) : Binding.DoNothing;
    }
}
