using Xunit;

namespace Dax.Vpax.Obfuscator.Tests.TestUtils;

internal static class AssertThat
{
    public static void IsObfuscated(DaxText text) => IsObfuscated(text.Value, text.ObfuscatedValue);

    public static void IsObfuscated(string plaintext, string obfuscated)
    {
        Assert.Equal(plaintext.Length, obfuscated.Length);
        Assert.NotEqual(plaintext, obfuscated);

        Assert.False(char.IsNumber(obfuscated[0]));
        Assert.DoesNotContain(" ", obfuscated);
        Assert.DoesNotContain(plaintext, obfuscated, StringComparison.OrdinalIgnoreCase);
    }
}
