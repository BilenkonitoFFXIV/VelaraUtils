namespace VelaraUtils.Utils;

public static class StringExtensions
{
    public static string ToString<T>(this T self) =>
        self?.ToString() ?? "null";
}
