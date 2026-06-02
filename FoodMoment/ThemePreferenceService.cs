namespace FoodMoment;

public static class ThemePreferenceService
{
    private const string PreferenceKey = "app_theme_index";

    public static readonly string[] ThemeOptions =
        ["Use system theme", "Light theme", "Dark theme"];

    public static int GetSavedIndex() => Preferences.Get(PreferenceKey, 0);

    public static void SaveIndex(int index)
    {
        if (index < 0 || index >= ThemeOptions.Length)
            index = 0;

        Preferences.Set(PreferenceKey, index);
    }

    public static void Apply(int index)
    {
        if (Application.Current is null)
            return;

        Application.Current.UserAppTheme = index switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };
    }
}
