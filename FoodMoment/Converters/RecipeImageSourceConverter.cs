using System.Globalization;

namespace FoodMoment.Converters;

/// <summary>
/// Resolves recipe images from bundled MAUI assets, local file paths, or HTTP URLs.
/// </summary>
public class RecipeImageSourceConverter : IValueConverter
{
    public const string DefaultRecipeImage = "default_recipe.jpg";

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path))
            return ImageSource.FromFile(DefaultRecipeImage);

        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return ImageSource.FromUri(new Uri(path));

        if (Path.IsPathRooted(path) || path.Contains('\\') || path.Contains('/'))
            return ImageSource.FromFile(path);

        return ImageSource.FromFile(path);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
