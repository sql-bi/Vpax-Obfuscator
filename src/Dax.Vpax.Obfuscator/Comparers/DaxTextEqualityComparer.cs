﻿namespace Dax.Vpax.Obfuscator.Comparers;

internal sealed class DaxTextValueEqualityComparer : EqualityComparer<DaxText>
{
    public static readonly DaxTextValueEqualityComparer Instance = new();

    public override bool Equals(DaxText? x, DaxText? y) => string.Equals(
        x?.Value ?? throw new ArgumentNullException(nameof(x)),
        y?.Value ?? throw new ArgumentNullException(nameof(y)),
        StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode(DaxText obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Value);
}

internal sealed class DaxTextObfuscatedValueEqualityComparer : EqualityComparer<DaxText>
{
    public static readonly DaxTextObfuscatedValueEqualityComparer Instance = new();

    public override bool Equals(DaxText? x, DaxText? y) => string.Equals(
        x?.ObfuscatedValue ?? throw new ArgumentNullException(nameof(x)),
        y?.ObfuscatedValue ?? throw new ArgumentNullException(nameof(y)),
        StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode(DaxText obj) => StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ObfuscatedValue);
}
