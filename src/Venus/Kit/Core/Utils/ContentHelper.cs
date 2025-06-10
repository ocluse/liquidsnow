namespace Ocluse.LiquidSnow.Venus.Kit.Utils;

public static class ContentHelper
{
    public const string ContentPath = "/_content/Ocluse.LiquidSnow.Venus.Kit";

    public static string GetContentPath(string path)
    {
        if (path.StartsWith('/'))
            path = path[1..];
        return $"{ContentPath}/{path}";
    }
}