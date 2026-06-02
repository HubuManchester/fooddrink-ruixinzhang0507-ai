using System.Diagnostics;
using FoodMoment.Models;
using FoodMoment;

namespace FoodMoment.Services;

/// <summary>
/// Dual-insurance loader: primary embedded JSON, then secondary embedded JSON, then built-in list.
/// </summary>
public class RecipeDataLoaderService
{
    private readonly EmbeddedRecipeDataService _embedded;
    private readonly LocalFallbackRecipeDataService _localFallback;

    public RecipeDataLoaderService(
        EmbeddedRecipeDataService embedded,
        LocalFallbackRecipeDataService localFallback)
    {
        _embedded = embedded;
        _localFallback = localFallback;
    }

    public async Task<DataLoadResult> LoadWithFallbackAsync()
    {
        try
        {
            var recipes = await _embedded.LoadFromEmbeddedJsonAsync();
            var message = $"Loaded {recipes.Count} items from embedded {DataConfig.PrimaryRecipesAsset}.";
            Debug.WriteLine(message);
            return new DataLoadResult(recipes, DataLoadSource.EmbeddedJson, message);
        }
        catch (Exception primaryEx)
        {
            Debug.WriteLine($"Primary JSON failed: {primaryEx.Message}. Trying {DataConfig.FallbackRecipesAsset}…");

            try
            {
                var recipes = await _localFallback.LoadFromFallbackJsonAsync();
                var message =
                    $"Loaded {recipes.Count} items from embedded {DataConfig.FallbackRecipesAsset}.";
                Debug.WriteLine(message);
                return new DataLoadResult(recipes, DataLoadSource.LocalStaticJson, message);
            }
            catch (Exception fallbackEx)
            {
                Debug.WriteLine($"Fallback JSON failed: {fallbackEx.Message}. Using built-in catalog.");

                var recipes = StaticRecipeCatalog.GetHardcodedRecipes();
                var message =
                    $"Both JSON bundles failed. Using built-in fallback ({recipes.Count} items).";
                return new DataLoadResult(recipes, DataLoadSource.HardcodedFallback, message);
            }
        }
    }
}
