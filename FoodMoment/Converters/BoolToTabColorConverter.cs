using System.Globalization;

namespace FoodMoment.Converters;

public class BoolToTabColorConverter : IValueConverter
{
    private static readonly Color ActiveLight = Color.FromArgb("#E85D04");
    private static readonly Color InactiveLight = Color.FromArgb("#F5F5F5");
    private static readonly Color ActiveDark = Color.FromArgb("#E85D04");
    private static readonly Color InactiveDark = Color.FromArgb("#2A2A2A");

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var isActive = value is bool b && b;
        if (parameter?.ToString() == "invert")
            isActive = !isActive;

        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;
        return isActive
            ? (isDark ? ActiveDark : ActiveLight)
            : (isDark ? InactiveDark : InactiveLight);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
