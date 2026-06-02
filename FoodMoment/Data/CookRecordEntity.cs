using SQLite;

namespace FoodMoment.Data;

[Table("cook_records")]
public class CookRecordEntity
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int RecipeId { get; set; }
    public string RecipeTitle { get; set; } = string.Empty;
    public string PhotoPath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
