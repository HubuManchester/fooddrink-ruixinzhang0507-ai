using FoodMoment.Models;

namespace FoodMoment.Services;

/// <summary>
/// Built-in recipes used when both embedded JSON bundles are unavailable.
/// </summary>
public static class StaticRecipeCatalog
{
    public static List<Recipe> GetHardcodedRecipes() =>
    [
        new()
        {
            Id = 101,
            Title = "Classic Tomato Eggs",
            Description = "Built-in fallback — quick home-style eggs and tomatoes.",
            ImageUrl = "tomato_eggs.jpg",
            Category = "Chinese",
            Ingredients = ["3 eggs", "2 tomatoes", "Salt", "Sugar"],
            Steps = ["Beat eggs.", "Stir-fry tomatoes.", "Combine and serve."]
        },
        new()
        {
            Id = 102,
            Title = "Iced Coffee",
            Description = "Built-in fallback — cold brew style coffee drink.",
            ImageUrl = "orange_juice.jpg",
            Category = "Drinks",
            Ingredients = ["Cold brew coffee", "Ice", "Milk optional"],
            Steps = ["Fill glass with ice.", "Pour cold brew.", "Add milk if desired."]
        },
        new()
        {
            Id = 103,
            Title = "Mango Smoothie",
            Description = "Built-in fallback — tropical blended drink.",
            ImageUrl = "smoothie_bowl.jpg",
            Category = "Drinks",
            Ingredients = ["1 mango", "1 banana", "Yogurt", "Ice"],
            Steps = ["Peel and chop fruit.", "Blend until smooth.", "Serve chilled."]
        }
    ];
}
