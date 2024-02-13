namespace Dax.Vpax.Obfuscator.Comparers;

internal sealed class DaxTextValueEqualityComparer : EqualityComparer<DaxText>
{
    public static readonly DaxTextValueEqualityComparer Instance = new();

    public override bool Equals(DaxText? x, DaxText? y) => string.Equals(x?.Value, y?.Value, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode(DaxText obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
}

internal sealed class DaxTextObfuscatedValueEqualityComparer : EqualityComparer<DaxText>
{
    public static readonly DaxTextObfuscatedValueEqualityComparer Instance = new();

    public override bool Equals(DaxText? x, DaxText? y) => string.Equals(x?.ObfuscatedValue, y?.ObfuscatedValue, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode(DaxText obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ObfuscatedValue);
}
