using Xunit;

namespace Dax.Vpax.Obfuscator.Tests.TestUtils;

internal static class AssertThat
{
    public static void IsObfuscated(DaxText text) => IsObfuscated(text.Value, text.ObfuscatedValue);
    public static void IsNotObfuscated(DaxText text) => IsNotObfuscated(text.Value, text.ObfuscatedValue);

    public static void IsObfuscated(string expected, string actual)
    {
        Assert.True(expected.Length <= actual.Length);
        Assert.NotEqual(expected, actual);

        Assert.False(char.IsNumber(actual[0]));
        Assert.DoesNotContain(" ", actual);
        Assert.DoesNotContain(expected, actual, StringComparison.OrdinalIgnoreCase);
    }

    public static void IsNotObfuscated(string expected, string actual)
    {
        Assert.NotNull(expected);
        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }
}
