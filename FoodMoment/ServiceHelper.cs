using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;

namespace FoodMoment;

public static class ServiceHelper
{
    public static T GetService<T>() where T : notnull =>
        Current.GetRequiredService<T>();

    private static IServiceProvider Current =>
        IPlatformApplication.Current?.Services
        ?? throw new InvalidOperationException("Application services are not ready.");
}
