using Newtonsoft.Json;

namespace Dax.Vpax.Obfuscator.Common;

public sealed class ObfuscationText
{
    [JsonConstructor]
    internal ObfuscationText(string value, string obfuscated)
    {
        Value = value;
        Obfuscated = obfuscated;
    }

    public string Value { get; }
    public string Obfuscated { get; }
}
