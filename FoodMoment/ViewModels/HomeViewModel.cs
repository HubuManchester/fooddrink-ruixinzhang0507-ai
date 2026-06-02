using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;

    public ObservableCollection<Recipe> Recipes { get; } = [];

    public IReadOnlyList<string> Categories { get; } = AppConstants.BrowseCategories;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = AppConstants.CategoryAll;

    [ObservableProperty]
    private bool _isRefreshing;

    public HomeViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        Title = AppConstants.AppName;
    }

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        var items = await _repository.GetAllAsync(SearchText, SelectedCategory);
        Recipes.Clear();
        foreach (var recipe in items)
            Recipes.Add(recipe);
        IsBusy = false;
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsRefreshing = true;
        await _repository.RefreshWithFallbackAsync();
        await LoadRecipesAsync();
    }

    [RelayCommand]
    private async Task RecipeSelectedAsync(Recipe recipe)
    {
        if (recipe is null)
            return;

        await Shell.Current.GoToAsync($"RecipeDetailPage?recipeId={recipe.Id}");
    }

    partial void OnSearchTextChanged(string value) =>
        _ = LoadRecipesAsync();

    partial void OnSelectedCategoryChanged(string value) =>
        _ = LoadRecipesAsync();
}
