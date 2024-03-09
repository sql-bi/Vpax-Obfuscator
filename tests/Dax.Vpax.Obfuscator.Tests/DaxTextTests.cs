using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxTextTests
{
    [Fact]
    public void ctor_NullValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DaxText(value: null!));
    }

    [Fact]
    public void ctor_NullObfuscatedValue_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new DaxText(value: "abc", obfuscatedValue: null!));
    }

    [Fact]
    public void ctor_EmptyValue_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() => new DaxText(value: string.Empty, obfuscatedValue: "abc"));
        Assert.StartsWith("Value cannot be empty.", exception.Message);
    }

    [Fact]
    public void ctor_EmptyObfuscatedValue_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() => new DaxText(value: "abc", obfuscatedValue: string.Empty));
        Assert.StartsWith("Value cannot be empty.", exception.Message);
    }

    [Fact]
    public void ctor_ObfuscatedValueShorterThanValue_Throws()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => new DaxText(value: "abc", obfuscatedValue: "ab"));
        Assert.Equal($"{nameof(DaxText.ObfuscatedValue)} cannot be shorter than the {nameof(DaxText.Value)}.", exception.Message);
    }
}
