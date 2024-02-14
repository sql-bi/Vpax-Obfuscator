namespace Dax.Vpax.Obfuscator.Common.Extensions;

internal static class StringExtensions
{
    public static bool IsValidDictionaryId(this string? value)
    {
        if (value == null) return false;
        if (!Guid.TryParseExact(value, "D", out var guid)) return false;
        if (guid == Guid.Empty) return false;

        return true;
    }
}
