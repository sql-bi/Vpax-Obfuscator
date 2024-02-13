using Dax.Metadata;

namespace Dax.Vpax.Obfuscator.Comparers;

internal sealed class DaxNameEqualityComparer : EqualityComparer<DaxName>
{
    public static readonly DaxNameEqualityComparer Instance = new();

    public override bool Equals(DaxName? x, DaxName? y) => string.Equals(x?.Name, x?.Name, StringComparison.OrdinalIgnoreCase);
    public override int GetHashCode(DaxName obj) => obj.Name.GetHashCode();
}
