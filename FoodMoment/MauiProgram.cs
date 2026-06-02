using FoodMoment.Data;
using FoodMoment.Services;
using Microsoft.Extensions.Logging;

namespace FoodMoment;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "foodmoment.db3");
        builder.Services.AddSingleton(new AppDatabase(dbPath));
        builder.Services.AddSingleton<EmbeddedRecipeDataService>();
        builder.Services.AddSingleton<LocalFallbackRecipeDataService>();
        builder.Services.AddSingleton<ICameraPhotoService, CameraPhotoService>();
        builder.Services.AddSingleton<RecipeDataLoaderService>();
        builder.Services.AddSingleton<IRecipeRepository, RecipeRepository>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
