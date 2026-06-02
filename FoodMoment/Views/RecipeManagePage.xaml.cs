using FoodMoment.ViewModels;

namespace FoodMoment.Views;

public partial class RecipeManagePage : ContentPage
{
    private readonly RecipeManageViewModel _viewModel;

    public RecipeManagePage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new RecipeManageViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadRecipesCommand.ExecuteAsync(null);
    }
}
