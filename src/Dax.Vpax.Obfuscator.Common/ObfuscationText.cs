using Newtonsoft.Json;

namespace Dax.Vpax.Obfuscator.Common;

public sealed class ObfuscationText
{
    [JsonConstructor]
    public ObfuscationText(string value, string obfuscated)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Obfuscated = obfuscated ?? throw new ArgumentNullException(nameof(obfuscated));
    }

    public string Value { get; }
    public string Obfuscated { get; }
}
