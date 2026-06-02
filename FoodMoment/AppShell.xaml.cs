using FoodMoment.Views;

namespace FoodMoment;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        HomeTab.Content = new HomePage();
        ExploreTab.Content = new ExplorePage();
        ProfileTab.Content = new ProfilePage();

        Routing.RegisterRoute(nameof(RecipeDetailPage), typeof(RecipeDetailPage));
        Routing.RegisterRoute(nameof(RecipeManagePage), typeof(RecipeManagePage));
        Routing.RegisterRoute(nameof(RecipeEditPage), typeof(RecipeEditPage));
    }
}
