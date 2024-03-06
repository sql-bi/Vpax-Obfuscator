using System.Diagnostics;
using Newtonsoft.Json;

namespace Dax.Vpax.Obfuscator.Common;

[DebuggerDisplay("{Value}")]
public sealed class ObfuscationText
{
    [JsonConstructor]
    public ObfuscationText(string value, string obfuscated, string? plaintext = null)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Obfuscated = obfuscated ?? throw new ArgumentNullException(nameof(obfuscated));
        Plaintext = plaintext;
    }

    public string Value { get; }
    public string Obfuscated { get; }
    public string? Plaintext { get; }
}
