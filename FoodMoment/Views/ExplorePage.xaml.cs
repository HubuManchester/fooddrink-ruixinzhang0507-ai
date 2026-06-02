using FoodMoment.ViewModels;

namespace FoodMoment.Views;

public partial class ExplorePage : ContentPage
{
    public ExplorePage()
    {
        InitializeComponent();
        BindingContext = new ExploreViewModel();
    }
}
