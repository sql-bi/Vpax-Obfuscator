using System.Diagnostics;
using System.Globalization;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator;

[DebuggerDisplay("{Value}")]
internal sealed class DaxText
{
    public DaxText(ObfuscationText text)
        : this(text.Value, text.Obfuscated)
    {
        if (text.Plaintext != null)
            PlaintextValue = text.Plaintext;
    }

    public DaxText(string value, string obfuscatedValue)
        : this(value)
    {
        ObfuscatedValue = obfuscatedValue;
    }

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
            if (_obfuscatedValue != null) throw new InvalidOperationException($"{nameof(ObfuscatedValue)} cannot be changed once set.");

            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value == string.Empty) throw new ArgumentException("Value cannot be empty.", nameof(value));
            if (value.Length < Value.Length) throw new InvalidOperationException($"{nameof(ObfuscatedValue)} cannot be shorter than the original {nameof(Value)}.");

            _obfuscatedValue = value;
        }
    }

    private string? _plaintextValue;
    public string? PlaintextValue
    {
        get => _plaintextValue;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value == string.Empty) throw new ArgumentException("Value cannot be empty.", nameof(value));
            if (_plaintextValue != null) throw new InvalidOperationException($"{nameof(PlaintextValue)} cannot be changed once set.");

            _plaintextValue = value;
        }
    }

    public ObfuscationText ToObfuscationText() => new(Value, ObfuscatedValue, PlaintextValue);
    public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0} | {1}", Value, ObfuscatedValue);
}
