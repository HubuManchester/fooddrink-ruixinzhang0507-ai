namespace FoodMoment.Services;

public enum DataLoadSource
{
    EmbeddedJson,
    LocalStaticJson,
    HardcodedFallback
}

public sealed class DataLoadResult
{
    public DataLoadResult(List<Models.Recipe> recipes, DataLoadSource source, string message)
    {
        Recipes = recipes;
        Source = source;
        Message = message;
    }

    public List<Models.Recipe> Recipes { get; }
    public DataLoadSource Source { get; }
    public string Message { get; }
}
