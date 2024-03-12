using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelDeobfuscator
{
    internal string DeobfuscateText(string obfuscatedValue, ObfuscatorRule rule = ObfuscatorRule.None)
    {
        if (obfuscatedValue.IsEmptyOrWhiteSpace() || rule.ShouldPreserve(obfuscatedValue))
            return obfuscatedValue;

        return _dictionary.GetValue(obfuscatedValue);
    }
}
