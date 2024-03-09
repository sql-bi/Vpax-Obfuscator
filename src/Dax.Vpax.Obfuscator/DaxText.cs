using System.Diagnostics;
using System.Globalization;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator;

[DebuggerDisplay("{Value}")]
internal sealed class DaxText
{
    public DaxText(ObfuscationText text)
        : this(text.Value, text.Obfuscated)
    { }

    public DaxText(string value, string obfuscatedValue)
        : this(value)
    {
        ObfuscatedValue = obfuscatedValue;
    }

    [DebuggerStepThrough]
    public DaxText(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value == string.Empty) throw new ArgumentException("Value cannot be empty.", nameof(value));

        Value = value;
    }

    public string Value { get; }

    private string? _obfuscatedValue;
    public string ObfuscatedValue
    {
        get => _obfuscatedValue;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value == string.Empty) throw new ArgumentException("Value cannot be empty.", nameof(value));
            if (value.Length < Value.Length) throw new InvalidOperationException($"{nameof(ObfuscatedValue)} cannot be shorter than the {nameof(Value)}.");

            _obfuscatedValue = value;
        }
    }

    public ObfuscationText ToObfuscationText() => new(Value, ObfuscatedValue);
    public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0} | {1}", Value, ObfuscatedValue);
}
