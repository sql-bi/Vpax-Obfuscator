using System.Diagnostics;
using System.Globalization;

namespace Dax.Vpax.Obfuscator;

[DebuggerDisplay("{Value}")]
internal sealed class DaxText 
{
    public DaxText(string value, string obfuscatedValue)
        : this(value)
    {
        if (obfuscatedValue == null) throw new ArgumentNullException(nameof(obfuscatedValue));
        if (obfuscatedValue == string.Empty) throw new ArgumentException("Obfuscated value cannot be empty.", nameof(obfuscatedValue));
        if (obfuscatedValue.Length < value.Length) throw new InvalidOperationException("Obfuscated value cannot be shorter than the original value.");

        ObfuscatedValue = obfuscatedValue;
    }

    public DaxText(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        if (value == string.Empty) throw new ArgumentException("Value cannot be empty.", nameof(value));

        Value = value;
    }

    public string Value { get; }
    public string ObfuscatedValue { get; internal set; }
    public bool IsObfuscatedAsDaxKeyword => DaxTextObfuscator.DaxKeywords.Contains(ObfuscatedValue ?? throw new InvalidOperationException("ObfuscatedValue is null"));

    public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0} | {1}", Value, ObfuscatedValue);
}
