using FoodMoment.Services;

namespace FoodMoment;

public partial class App : Application
{
    private readonly IRecipeRepository _recipeRepository;

    public App(IRecipeRepository recipeRepository)
    {
        InitializeComponent();

        // Follow system light/dark mode (required).
        UserAppTheme = AppTheme.Unspecified;

        _recipeRepository = recipeRepository;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Seed SQLite before Shell creates HomePage (avoids empty list on Android).
        _recipeRepository.InitializeAsync().GetAwaiter().GetResult();
        return new Window(new AppShell());
    }
}
