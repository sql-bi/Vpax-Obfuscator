namespace Dax.Vpax.Obfuscator.Extensions;

internal static class StringExtensions
{
    public static string DaxEscape(this string value) 
        => value.Replace("\"", "\"\"").Replace("'", "''");
}
