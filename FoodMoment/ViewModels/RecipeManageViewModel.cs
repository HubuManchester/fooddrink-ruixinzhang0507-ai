using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class RecipeManageViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;

    public ObservableCollection<Recipe> Recipes { get; } = [];

    public RecipeManageViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        Title = "Manage Recipes";
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        var items = await _repository.GetAllAsync();
        Recipes.Clear();
        foreach (var recipe in items)
            Recipes.Add(recipe);
        IsBusy = false;
    }

    [RelayCommand]
    private async Task AddRecipeAsync() =>
        await Shell.Current.GoToAsync("RecipeEditPage");

    [RelayCommand]
    private async Task EditRecipeAsync(Recipe recipe)
    {
        if (recipe is null)
            return;

        await Shell.Current.GoToAsync($"RecipeEditPage?recipeId={recipe.Id}");
    }

    [RelayCommand]
    private async Task DeleteRecipeAsync(Recipe recipe)
    {
        if (recipe is null)
            return;

        var confirm = await Shell.Current.DisplayAlert(
            "Delete Recipe",
            $"Delete \"{recipe.Title}\"? This cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirm)
            return;

        await _repository.DeleteAsync(recipe.Id);
        Recipes.Remove(recipe);
    }
}
