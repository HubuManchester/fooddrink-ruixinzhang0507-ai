using System.Text.Json;
using FoodMoment.Data;
using FoodMoment.Models;

namespace FoodMoment.Services;

public interface IRecipeRepository
{
    string LastLoadMessage { get; }
    DataLoadSource? LastLoadSource { get; }

    Task InitializeAsync();
    Task<DataLoadResult> RefreshWithFallbackAsync();
    Task<List<Recipe>> GetAllAsync(string? search = null, string? category = null);
    Task<Recipe?> GetByIdAsync(int id);
    Task<List<Recipe>> GetFavoritesAsync();
    Task ToggleFavoriteAsync(int id);
    Task<int> AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(int id);
    Task AddCookRecordAsync(int recipeId, string recipeTitle, string photoPath);
    Task<List<CookRecord>> GetCookRecordsAsync();
    Task ClearCookRecordsAsync();
}

public class RecipeRepository : IRecipeRepository
{
    private readonly AppDatabase _database;
    private readonly RecipeDataLoaderService _dataLoader;

    public string LastLoadMessage { get; private set; } = "Not loaded yet.";
    public DataLoadSource? LastLoadSource { get; private set; }

    public RecipeRepository(AppDatabase database, RecipeDataLoaderService dataLoader)
    {
        _database = database;
        _dataLoader = dataLoader;
    }

    public async Task InitializeAsync()
    {
        await _database.InitializeAsync();
        await RefreshWithFallbackAsync();
    }

    public async Task<DataLoadResult> RefreshWithFallbackAsync()
    {
        var result = await _dataLoader.LoadWithFallbackAsync();
        await MergeRecipesIntoDatabaseAsync(result.Recipes);

        LastLoadSource = result.Source;
        LastLoadMessage = result.Message;
        return result;
    }

    private async Task MergeRecipesIntoDatabaseAsync(IReadOnlyList<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            if (recipe.Id > 0)
            {
                var existing = await _database.GetRecipeAsync(recipe.Id);
                if (existing is not null)
                {
                    var wasFavorite = existing.IsFavorite;
                    existing.Title = recipe.Title;
                    existing.Description = recipe.Description;
                    existing.ImageUrl = recipe.ImageUrl;
                    existing.Category = recipe.Category;
                    existing.IngredientsJson = JsonSerializer.Serialize(recipe.Ingredients);
                    existing.StepsJson = JsonSerializer.Serialize(recipe.Steps);
                    existing.Calories = recipe.Calories;
                    existing.Protein = recipe.Protein;
                    existing.Carbs = recipe.Carbs;
                    existing.Fat = recipe.Fat;
                    existing.IsFavorite = wasFavorite;
                    await _database.UpdateRecipeAsync(existing);
                    continue;
                }
            }

            var entity = ToEntity(recipe);
            entity.Id = 0;
            await _database.InsertRecipeAsync(entity);
        }
    }

    public async Task<List<Recipe>> GetAllAsync(string? search = null, string? category = null)
    {
        var entities = await _database.GetAllRecipesAsync();
        var recipes = entities.Select(ToModel).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(category) && category != AppConstants.CategoryAll)
            recipes = recipes.Where(r => r.Category == category);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            recipes = recipes.Where(r =>
                r.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        return recipes.ToList();
    }

    public async Task<Recipe?> GetByIdAsync(int id)
    {
        var entity = await _database.GetRecipeAsync(id);
        return entity is null ? null : ToModel(entity);
    }

    public async Task<List<Recipe>> GetFavoritesAsync()
    {
        var entities = await _database.GetFavoriteRecipesAsync();
        return entities.Select(ToModel).ToList();
    }

    public async Task ToggleFavoriteAsync(int id)
    {
        var entity = await _database.GetRecipeAsync(id)
            ?? throw new InvalidOperationException($"Recipe {id} not found.");

        entity.IsFavorite = !entity.IsFavorite;
        await _database.UpdateRecipeAsync(entity);
    }

    public async Task<int> AddAsync(Recipe recipe)
    {
        if (string.IsNullOrWhiteSpace(recipe.Title))
            throw new ArgumentException("Recipe title is required.");

        var entity = ToEntity(recipe);
        entity.Id = 0;
        await _database.InsertRecipeAsync(entity);
        return entity.Id;
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        if (recipe.Id <= 0)
            throw new ArgumentException("Invalid recipe id.");

        if (string.IsNullOrWhiteSpace(recipe.Title))
            throw new ArgumentException("Recipe title is required.");

        var existing = await _database.GetRecipeAsync(recipe.Id)
            ?? throw new InvalidOperationException($"Recipe {recipe.Id} not found.");

        existing.Title = recipe.Title.Trim();
        existing.Description = recipe.Description.Trim();
        existing.ImageUrl = recipe.ImageUrl.Trim();
        existing.Category = recipe.Category;
        existing.IngredientsJson = JsonSerializer.Serialize(recipe.Ingredients);
        existing.StepsJson = JsonSerializer.Serialize(recipe.Steps);
        existing.Calories = recipe.Calories;
        existing.Protein = recipe.Protein;
        existing.Carbs = recipe.Carbs;
        existing.Fat = recipe.Fat;
        existing.IsFavorite = recipe.IsFavorite;

        await _database.UpdateRecipeAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid recipe id.");

        var existing = await _database.GetRecipeAsync(id)
            ?? throw new InvalidOperationException($"Recipe {id} not found.");

        await _database.DeleteRecipeAsync(existing.Id);
    }

    public async Task AddCookRecordAsync(int recipeId, string recipeTitle, string photoPath)
    {
        await _database.InsertCookRecordAsync(new CookRecordEntity
        {
            RecipeId = recipeId,
            RecipeTitle = recipeTitle,
            PhotoPath = photoPath,
            CreatedAt = DateTime.Now
        });
    }

    public async Task<List<CookRecord>> GetCookRecordsAsync()
    {
        var records = await _database.GetCookRecordsAsync();
        return records.Select(r => new CookRecord
        {
            Id = r.Id,
            RecipeId = r.RecipeId,
            RecipeTitle = r.RecipeTitle,
            PhotoPath = r.PhotoPath,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task ClearCookRecordsAsync()
    {
        var records = await _database.GetCookRecordsAsync();
        await _database.DeleteAllCookRecordsAsync();

        var recordsDir = Path.Combine(FileSystem.AppDataDirectory, "records");
        if (!Directory.Exists(recordsDir))
            return;

        foreach (var record in records)
        {
            if (!string.IsNullOrWhiteSpace(record.PhotoPath) && File.Exists(record.PhotoPath))
            {
                try
                {
                    File.Delete(record.PhotoPath);
                }
                catch
                {
                }
            }
        }

        foreach (var file in Directory.EnumerateFiles(recordsDir))
        {
            try
            {
                File.Delete(file);
            }
            catch
            {
            }
        }
    }

    private static Recipe ToModel(RecipeEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        ImageUrl = entity.ImageUrl,
        Category = entity.Category,
        Ingredients = JsonSerializer.Deserialize<List<string>>(entity.IngredientsJson) ?? [],
        Steps = JsonSerializer.Deserialize<List<string>>(entity.StepsJson) ?? [],
        IsFavorite = entity.IsFavorite,
        Calories = entity.Calories,
        Protein = entity.Protein,
        Carbs = entity.Carbs,
        Fat = entity.Fat
    };

    private static RecipeEntity ToEntity(Recipe recipe) => new()
    {
        Id = recipe.Id,
        Title = recipe.Title,
        Description = recipe.Description,
        ImageUrl = recipe.ImageUrl,
        Category = recipe.Category,
        IngredientsJson = JsonSerializer.Serialize(recipe.Ingredients),
        StepsJson = JsonSerializer.Serialize(recipe.Steps),
        IsFavorite = recipe.IsFavorite,
        Calories = recipe.Calories,
        Protein = recipe.Protein,
        Carbs = recipe.Carbs,
        Fat = recipe.Fat
    };
}
