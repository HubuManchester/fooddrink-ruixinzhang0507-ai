using FoodMoment.ViewModels;

namespace FoodMoment.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public ProfilePage()
    {
        InitializeComponent();
        BindingContext = _viewModel = new ProfileViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
