using FoodMoment.Services;
using FoodMoment.ViewModels;

namespace FoodMoment.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new HomeViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ServiceHelper.GetService<IRecipeRepository>().InitializeAsync();
        await _viewModel.LoadRecipesCommand.ExecuteAsync(null);
    }
}
