using FoodMoment.Models;
using FoodMoment;

namespace FoodMoment.Services;

/// <summary>
/// Loads a second embedded JSON bundle when the primary recipes.json is missing or invalid.
/// </summary>
public class LocalFallbackRecipeDataService
{
    private readonly EmbeddedRecipeDataService _embedded;

    public LocalFallbackRecipeDataService(EmbeddedRecipeDataService embedded) =>
        _embedded = embedded;

    public Task<List<Recipe>> LoadFromFallbackJsonAsync() =>
        _embedded.LoadFromEmbeddedJsonAsync(DataConfig.FallbackRecipesAsset);
}
