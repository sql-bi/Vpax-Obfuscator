using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelObfuscator
{
    internal string ObfuscateText(DaxText text, ObfuscatorRule rule = ObfuscatorRule.None)
    {
        if (text.Value.IsEmptyOrWhiteSpace() || rule.ShouldPreserve(text.Value))
        {
            UnobfuscatedValues.Add(text.Value);
            return text.ObfuscatedValue = text.Value;
        }

        if (Texts.TryGet(text, out var obfuscatedText))
            return text.ObfuscatedValue = obfuscatedText?.ObfuscatedValue ?? throw new InvalidOperationException($"Obfuscated text is null. {text.Value}");

        _ = _obfuscator.Obfuscate(text);

        var retryLimit = DaxTextObfuscator.RetryLimitBeforeExtension + 100; // retry with the same length otherwise extend the obfuscated string length up to 100 characters
        var retryCount = 0;
        while (retryCount < retryLimit && IsRetryNeeded())
            _ = _obfuscator.Obfuscate(text, ++retryCount);

        if (retryCount >= retryLimit)
            throw new InvalidOperationException($"Failed to obfuscate text. {text.Value} | {text.ObfuscatedValue}");

        Texts.Add(text); // << throws in case of unresolved collision (duplicate value/obfuscated value)
        return text.ObfuscatedValue;

        bool IsRetryNeeded() => text.ObfuscatedValue.IsDaxKeyword() || Texts.Contains(text);
    }
}
