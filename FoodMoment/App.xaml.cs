namespace FoodMoment;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        ThemePreferenceService.Apply(ThemePreferenceService.GetSavedIndex());
    }

    protected override Window CreateWindow(IActivationState? activationState) =>
        new(new AppShell());
}
