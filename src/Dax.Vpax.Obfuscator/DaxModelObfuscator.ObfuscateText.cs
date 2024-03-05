using Dax.Vpax.Obfuscator.Extensions;

namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelObfuscator
{
    internal DaxText ObfuscateText(DaxText text)
    {
        if (Texts.TryGet(text, out var obfuscatedText))
            return obfuscatedText ?? throw new InvalidOperationException($"Obfuscated text is null. {text.Value}");

        obfuscatedText = _obfuscator.Obfuscate(text);

        var retryLimit = DaxTextObfuscator.RetryLimitBeforeExtension + 100; // retry with the same length otherwise extend the obfuscated string length up to 100 characters
        var retryCount = 0;
        while (retryCount < retryLimit && IsRetryNeeded())
            obfuscatedText = _obfuscator.Obfuscate(obfuscatedText, ++retryCount);

        if (retryCount >= retryLimit)
            throw new InvalidOperationException($"Failed to obfuscate text. {obfuscatedText.Value} | {obfuscatedText.ObfuscatedValue}");

        Texts.Add(obfuscatedText); // << throws in case of unresolved collision (duplicate value/obfuscated value)
        return obfuscatedText;

        bool IsRetryNeeded() => obfuscatedText.ObfuscatedValue.IsDaxKeyword() || Texts.Contains(obfuscatedText);
    }
}
