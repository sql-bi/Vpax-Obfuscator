using Dax.Vpax.Obfuscator.Tests.TestUtils;
using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxTextObfuscatorTests
{
    private readonly DaxTextObfuscator _obfuscator = new DaxTextObfuscator();

    [Theory]
    [InlineData("abc")]
    [InlineData("Measure1")]
    [InlineData("Sales Amount")]
    public void Obfuscate_SameValueUsingDifferentDaxTextObfuscatorInstances_ReturnsDifferentObfuscatedValues(string value)
    {
        var instance1 = new DaxTextObfuscator();
        {
            // Add a delay to ensure that Random is seeded with a different value for each instance
            // On most Windows systems, Random objects created within 15 milliseconds of one another are likely to have identical seed values.
            // https://learn.microsoft.com/en-us/dotnet/fundamentals/runtime-libraries/system-random#instantiate-the-random-number-generator
            Thread.Sleep(100);
        }
        var instance2 = new DaxTextObfuscator();

        var text1 = instance1.Obfuscate(new DaxText(value));
        var text2 = instance2.Obfuscate(new DaxText(value));

        Assert.NotEqual(text1.ObfuscatedValue, text2.ObfuscatedValue);
    }

    [Fact]
    public void Obfuscate_EscapedQuotationMarkInStringLiteral_ReturnsUnobfuscatedQuotationMark()
    {
        var value = "\"\"\"\""; // e.g. VAR __quotationMarkChar = """"
        var text = _obfuscator.Obfuscate(new DaxText(value));
        Assert.Equal(value, text.ObfuscatedValue);
    }

    [Theory]
    [InlineData(DaxTextObfuscator.CharSet)]
    [InlineData("Sales Amount")]
    public void Obfuscate_ReturnsObfuscatedString(string value)
    {
        var text = new DaxTextObfuscator().Obfuscate(new DaxText(value));

        AssertThat.IsObfuscated(text);
    }

    [Theory]
    [InlineData("-- This is a singleline comment")]
    [InlineData("-- This-is-a-singleline-comment --")]
    public void Obfuscate_SingleLineComment_PreservesLeadingCommentChars(string value)
    {
        var text = _obfuscator.Obfuscate(new DaxText(value));

        Assert.StartsWith("--", text.ObfuscatedValue);
        AssertThat.IsObfuscated(text);
    }

    [Theory]
    [InlineData("/* This is a multi line comment */")]
    [InlineData("/* This-is-a-multi-line-comment */")]
    public void Obfuscate_MultiLineComment_PreservesLeadingAndTrailingCommentChars(string value)
    {
        var text = _obfuscator.Obfuscate(new DaxText(value));

        Assert.StartsWith("/*", text.ObfuscatedValue);
        Assert.EndsWith("*/", text.ObfuscatedValue);
        AssertThat.IsObfuscated(text);
    }

    [Fact]
    public void Obfuscate_MultiLineComment_PreservesCRAndLFChars()
    {
        var value = "/* This is a multiline comment containing CR \r and LF \n characters \r\n */";
        var text = _obfuscator.Obfuscate(new DaxText(value));

        Assert.Equal('\r', text.ObfuscatedValue[45]);
        Assert.Equal('\n', text.ObfuscatedValue[54]);
        Assert.Equal('\r', text.ObfuscatedValue[67]);
        Assert.Equal('\n', text.ObfuscatedValue[68]);
    }
}
