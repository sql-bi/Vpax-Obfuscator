namespace Dax.Vpax.Obfuscator;

internal sealed partial class DaxModelObfuscator
{
    internal DaxText ObfuscateText(DaxText text)
    {
        if (Texts.TryGet(text, out var obfuscatedText))
            return obfuscatedText;

        _ = _obfuscator.Obfuscate(text);

        var retryLimit = DaxTextObfuscator.RetryLimitBeforeExtension + 100; // retry with the same length otherwise extend the obfuscated string length up to 100 characters
        var retryCount = 0;
        while (retryCount < retryLimit && IsRetryNeeded())
            _ = _obfuscator.Obfuscate(text, ++retryCount);

        if (retryCount >= retryLimit)
            throw new InvalidOperationException($"Failed to obfuscate text >> {text.Value} | {text.ObfuscatedValue}");

        Texts.Add(text); // << throws in case of unresolved collision (duplicate value/obfuscated value)
        return text;

        bool IsRetryNeeded() => text.IsObfuscatedAsDaxKeyword || Texts.Contains(text);
    }
}
