namespace Ocluse.LiquidSnow.Utils;
internal static class CacheKeyHelper
{
    public static string GetKey(Type type)
    {
        return type.FullName ?? throw new InvalidOperationException("Type has no full name");
    }

    public static string GetKey(Type type1, Type type2)
    {
        if (type1.FullName == null || type2.FullName == null)
        {
            throw new InvalidOperationException("Could not determine the names of the types");
        }

        return $"{type1.FullName}::{type2.FullName}";
    }
}