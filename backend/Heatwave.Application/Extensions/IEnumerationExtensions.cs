namespace Heatwave.Application.Extensions;
internal static class IEnumerationExtensions
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collect)
    {
        return collect is null || !collect.Any();
    }
    public static bool IsNotNullOrEmpty<T>(this IEnumerable<T>? collect)
    {
        return !(collect is null || !collect.Any());
    }

    public static string Join<T>(this IEnumerable<T>? collect, char separator = ',')
    {
        return string.Join(separator, collect);
    }
}
