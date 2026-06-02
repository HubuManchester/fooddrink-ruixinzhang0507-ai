using FoodMoment.ViewModels;

namespace FoodMoment.Views;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeEditPage : ContentPage
{
    private readonly RecipeEditViewModel _viewModel;

    public string RecipeId
    {
        set
        {
            if (int.TryParse(value, out var id))
                _viewModel.RecipeId = id;
        }
    }

    public RecipeEditPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new RecipeEditViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadAsync();
    }
}
