using SQLite;

namespace FoodMoment.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _connection;

    public AppDatabase(string dbPath)
    {
        _connection = new SQLiteAsyncConnection(dbPath);
    }

    public async Task InitializeAsync()
    {
        await _connection.CreateTableAsync<RecipeEntity>();
        await _connection.CreateTableAsync<CookRecordEntity>();
        await EnsureRecipeNutritionColumnsAsync();
    }

    private async Task EnsureRecipeNutritionColumnsAsync()
    {
        var columns = await _connection.GetTableInfoAsync("recipes");
        var names = columns.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!names.Contains("Calories"))
            await _connection.ExecuteAsync("ALTER TABLE recipes ADD COLUMN Calories INTEGER NOT NULL DEFAULT 0");
        if (!names.Contains("Protein"))
            await _connection.ExecuteAsync("ALTER TABLE recipes ADD COLUMN Protein INTEGER NOT NULL DEFAULT 0");
        if (!names.Contains("Carbs"))
            await _connection.ExecuteAsync("ALTER TABLE recipes ADD COLUMN Carbs INTEGER NOT NULL DEFAULT 0");
        if (!names.Contains("Fat"))
            await _connection.ExecuteAsync("ALTER TABLE recipes ADD COLUMN Fat INTEGER NOT NULL DEFAULT 0");
    }

    public async Task<int> GetRecipeCountAsync() =>
        await _connection.Table<RecipeEntity>().CountAsync();

    public async Task<List<RecipeEntity>> GetAllRecipesAsync() =>
        await _connection.Table<RecipeEntity>().OrderByDescending(r => r.Id).ToListAsync();

    public async Task<RecipeEntity?> GetRecipeAsync(int id) =>
        await _connection.Table<RecipeEntity>().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<List<RecipeEntity>> GetFavoriteRecipesAsync() =>
        await _connection.Table<RecipeEntity>().Where(r => r.IsFavorite).ToListAsync();

    public async Task<int> InsertRecipeAsync(RecipeEntity recipe) =>
        await _connection.InsertAsync(recipe);

    public async Task InsertRecipesAsync(IEnumerable<RecipeEntity> recipes) =>
        await _connection.InsertAllAsync(recipes);

    public async Task UpdateRecipeAsync(RecipeEntity recipe) =>
        await _connection.UpdateAsync(recipe);

    public async Task DeleteRecipeAsync(int id) =>
        await _connection.DeleteAsync<RecipeEntity>(id);

    public async Task<List<CookRecordEntity>> GetCookRecordsAsync() =>
        await _connection.Table<CookRecordEntity>().OrderByDescending(r => r.CreatedAt).ToListAsync();

    public async Task InsertCookRecordAsync(CookRecordEntity record) =>
        await _connection.InsertAsync(record);

    public async Task DeleteCookRecordAsync(int id) =>
        await _connection.DeleteAsync<CookRecordEntity>(id);
}
