using FoodMoment.ViewModels;

namespace FoodMoment.Views;

[QueryProperty(nameof(RecipeId), "recipeId")]
public partial class RecipeDetailPage : ContentPage
{
    private readonly RecipeDetailViewModel _viewModel;

    public int RecipeId
    {
        set => _viewModel.RecipeId = value;
    }

    public RecipeDetailPage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new RecipeDetailViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadRecipeAsync();
    }
}
