using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class RecipeEditViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;

    public IReadOnlyList<string> Categories { get; } = AppConstants.RecipeCategories;

    [ObservableProperty]
    private int _recipeId;

    [ObservableProperty]
    private string _titleText = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _imageUrl = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "Chinese";

    [ObservableProperty]
    private string _ingredientsText = string.Empty;

    [ObservableProperty]
    private string _stepsText = string.Empty;

    public bool IsEditing => RecipeId > 0;

    public RecipeEditViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        Title = "Add Recipe";
    }

    public async Task LoadAsync()
    {
        if (RecipeId <= 0)
        {
            Title = "Add Recipe";
            return;
        }

        IsBusy = true;
        var recipe = await _repository.GetByIdAsync(RecipeId);
        if (recipe is null)
        {
            IsBusy = false;
            await Shell.Current.GoToAsync("..");
            return;
        }

        Title = "Edit Recipe";
        TitleText = recipe.Title;
        Description = recipe.Description;
        ImageUrl = recipe.ImageUrl;
        SelectedCategory = recipe.Category;
        IngredientsText = string.Join(Environment.NewLine, recipe.Ingredients);
        StepsText = string.Join(Environment.NewLine, recipe.Steps);
        IsBusy = false;
    }

    partial void OnRecipeIdChanged(int value) =>
        Title = value > 0 ? "Edit Recipe" : "Add Recipe";

    [RelayCommand]
    private async Task SaveAsync()
    {
        var validationError = ValidateInput();
        if (validationError is not null)
        {
            await Shell.Current.DisplayAlert("Validation", validationError, "OK");
            return;
        }

        var recipe = BuildRecipeFromInput();

        IsBusy = true;
        if (IsEditing)
        {
            recipe.Id = RecipeId;
            var existing = await _repository.GetByIdAsync(RecipeId);
            recipe.IsFavorite = existing?.IsFavorite ?? false;
            await _repository.UpdateAsync(recipe);
        }
        else
        {
            await _repository.AddAsync(recipe);
        }

        IsBusy = false;
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");

    private string? ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(TitleText))
            return "Please enter a recipe title.";

        if (string.IsNullOrWhiteSpace(Description))
            return "Please enter a description.";

        if (string.IsNullOrWhiteSpace(SelectedCategory))
            return "Please select a category.";

        if (ParseLines(StepsText).Count == 0)
            return "Add at least one cooking step (one step per line).";

        return null;
    }

    private Recipe BuildRecipeFromInput() => new()
    {
        Title = TitleText.Trim(),
        Description = Description.Trim(),
        ImageUrl = string.IsNullOrWhiteSpace(ImageUrl)
            ? Converters.RecipeImageSourceConverter.DefaultRecipeImage
            : ImageUrl.Trim(),
        Category = SelectedCategory,
        Ingredients = ParseLines(IngredientsText),
        Steps = ParseLines(StepsText)
    };

    private static List<string> ParseLines(string text) =>
        text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
}
