using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodMoment.Models;
using FoodMoment.Services;

namespace FoodMoment.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
    private readonly IRecipeRepository _repository;

    public ObservableCollection<Recipe> FavoriteRecipes { get; } = [];
    public ObservableCollection<CookRecord> CookRecords { get; } = [];

    [ObservableProperty]
    private string _dataSourceStatus = "Data source: not loaded yet.";

    [ObservableProperty]
    private int _selectedThemeIndex;

    public IReadOnlyList<string> ThemeOptions { get; } = ThemePreferenceService.ThemeOptions;

    public ProfileViewModel()
    {
        _repository = ServiceHelper.GetService<IRecipeRepository>();
        Title = "Profile";
        SelectedThemeIndex = ThemePreferenceService.GetSavedIndex();
    }

    partial void OnSelectedThemeIndexChanged(int value)
    {
        ThemePreferenceService.SaveIndex(value);
        ThemePreferenceService.Apply(value);
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        UpdateDataSourceStatus();

        var favorites = await _repository.GetFavoritesAsync();
        FavoriteRecipes.Clear();
        foreach (var recipe in favorites)
            FavoriteRecipes.Add(recipe);

        var records = await _repository.GetCookRecordsAsync();
        CookRecords.Clear();
        foreach (var record in records)
            CookRecords.Add(record);

        IsBusy = false;
    }

    [RelayCommand]
    private async Task ReloadDataAsync()
    {
        IsBusy = true;
        await _repository.RefreshWithFallbackAsync();
        UpdateDataSourceStatus();
        await LoadDataAsync();
        IsBusy = false;

        await Toast("Data reloaded using local-first fallback chain.");
    }

    [RelayCommand]
    private async Task OpenManagePageAsync() =>
        await Shell.Current.GoToAsync("RecipeManagePage");

    [RelayCommand]
    private async Task ClearCookingPhotosAsync()
    {
        var page = Shell.Current?.CurrentPage;
        if (page is null)
            return;

        var confirm = await page.DisplayAlert(
            "Clear cooking photos",
            "Delete all captured dish photos and cooking history? This cannot be undone.",
            "Clear",
            "Cancel");

        if (!confirm)
            return;

        IsBusy = true;
        await _repository.ClearCookRecordsAsync();
        CookRecords.Clear();
        IsBusy = false;

        await Toast("Cooking photos and history cleared.");
    }

    private void UpdateDataSourceStatus()
    {
        var source = _repository.LastLoadSource?.ToString() ?? "SQLite cache";
        DataSourceStatus = $"Last load: {source} — {_repository.LastLoadMessage}";
    }

    private static async Task Toast(string message)
    {
        try
        {
            var page = Shell.Current?.CurrentPage;
            if (page is not null)
                await page.DisplayAlert("FoodMoment", message, "OK");
        }
        catch
        {
        }
    }
}
