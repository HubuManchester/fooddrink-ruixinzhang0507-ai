using SQLite;

namespace FoodMoment.Data;

[Table("recipes")]
public class RecipeEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string IngredientsJson { get; set; } = "[]";
    public string StepsJson { get; set; } = "[]";
    public bool IsFavorite { get; set; }
}
