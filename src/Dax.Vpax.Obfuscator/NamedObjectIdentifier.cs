using Dax.Metadata;

namespace Dax.Vpax.Obfuscator;

internal class NamedObjectIdentifier : IEquatable<NamedObjectIdentifier>
{
    public NamedObjectIdentifier(string name, NamedObjectType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

        if (type == NamedObjectType.Null)
            throw new ArgumentException("Type cannot be None.", nameof(type));

        Name = name;
        Type = type;
    }

    public string Name { get; }
    public NamedObjectType Type { get; }

    public bool Equals(NamedObjectIdentifier? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name)
            && Type == other.Type;
    }

    public override bool Equals(object? obj) => Equals(obj as NamedObjectIdentifier);

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            hash = hash * 23 + Type.GetHashCode();
            return hash;
        }
    }

    public override string ToString() => $"[{Type}] {Name}";
}

internal class NamedObjectIdentifierCollection
{
    private readonly Dictionary<NamedObjectIdentifier, object> _identifiers = [];

    public NamedObjectIdentifierCollection()
    { }

    public void Map(Function function)
    {
        var identifier = new NamedObjectIdentifier(function.FunctionName.Name, NamedObjectType.Function);
        _identifiers.Add(identifier, function);
    }

    public bool IsKnownUserDefinedFunction(string name)
    {
        var identifier = new NamedObjectIdentifier(name, NamedObjectType.Function);
        return _identifiers.ContainsKey(identifier);
    }
}
