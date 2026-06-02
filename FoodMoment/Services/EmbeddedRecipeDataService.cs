using System.Text.Json;
using FoodMoment.Models;
using FoodMoment;

namespace FoodMoment.Services;

public class EmbeddedRecipeDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Task<List<Recipe>> LoadFromEmbeddedJsonAsync() =>
        LoadFromEmbeddedJsonAsync(DataConfig.PrimaryRecipesAsset);

    public async Task<List<Recipe>> LoadFromEmbeddedJsonAsync(string assetFileName)
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(assetFileName);
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();

        var collection = JsonSerializer.Deserialize<RecipeCollection>(json, JsonOptions)
            ?? throw new InvalidOperationException($"Embedded {assetFileName} is empty or invalid.");

        if (collection.Recipes.Count == 0)
            throw new InvalidOperationException($"Embedded {assetFileName} contains no recipes.");

        return collection.Recipes;
    }
}
