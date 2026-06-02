using System.Globalization;

namespace FoodMoment.Converters;

/// <summary>
/// Resolves recipe images from bundled MAUI assets, local file paths, or HTTP URLs.
/// </summary>
public class RecipeImageSourceConverter : IValueConverter
{
    public const string DefaultRecipeImage = "default_recipe.jpg";

    private static readonly HashSet<string> BundledRecipeImages = new(StringComparer.OrdinalIgnoreCase)
    {
        DefaultRecipeImage,
        "tomato_eggs.jpg",
        "avocado_toast.jpg",
        "matcha_latte.jpg",
        "orange_juice.jpg",
        "smoothie_bowl.jpg",
        "stir_fry_greens.jpg",
        "lava_cake.jpg",
        "honey_lemon_tea.jpg",
        "chicken_caesar_salad.jpg",
        "mushroom_risotto.jpg",
        "beef_noodle_soup.jpg",
        "mango_milk_tea.jpg",
        "yogurt_parfait.jpg",
        "salmon_bowl.jpg"
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string path || string.IsNullOrWhiteSpace(path))
            return ImageSource.FromFile(DefaultRecipeImage);

        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return ImageSource.FromUri(new Uri(path));

        if (Path.IsPathRooted(path) || path.Contains('\\') || path.Contains('/'))
            return ImageSource.FromFile(path);

        var fileName = Path.GetFileName(path);
        if (!BundledRecipeImages.Contains(fileName))
            return ImageSource.FromFile(DefaultRecipeImage);

        return ImageSource.FromFile(fileName);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
