using Xunit;

namespace Dax.Vpax.Obfuscator.Tests;

public class DaxTextCollectionTests
{
    [Theory]
    [InlineData("A", "A")]
    [InlineData("A", "a")]
    [InlineData("a", "a")]
    public void Add_DuplicateValue_Throws(string value1, string value2)
    {
        var text1 = new DaxText(value1); text1.ObfuscatedValue = value1;
        var text2 = new DaxText(value2); text2.ObfuscatedValue = value2;

        var texts = new DaxTextCollection();
        texts.Add(text1);

        Assert.Throws<ArgumentException>(() => texts.Add(text2));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Add_DuplicateValue_Throws__DifferentLengthsTest(int count)
    {
        var charset = DaxTextObfuscator.CharSet.ToArray().Where(char.IsUpper); // exclude lowercase because DaxText equality comparer is case-insensitive
        var values = charset.Select((c) => new string(c, count)).ToList();
        var dictionary = new DaxTextCollection();

        foreach (var value in values) {
            var text = new DaxText(value); text.ObfuscatedValue = value;
            dictionary.Add(text); // Seed the result dictionary
            Assert.Throws<ArgumentException>(() => dictionary.Add(text));
        }
    }

    [Theory]
    [InlineData("A", "A")]
    [InlineData("A", "a")]
    [InlineData("a", "a")]
    public void Add_DuplicateObfuscatedValue_Throws(string obfuscatedValue1, string obfuscatedValue2)
    {
        var text1 = new DaxText("X");
        var text2 = new DaxText("Y");

        text1.ObfuscatedValue = obfuscatedValue1;
        text2.ObfuscatedValue = obfuscatedValue2;

        var texts = new DaxTextCollection();
        texts.Add(text1);

        Assert.Throws<ArgumentException>(() => texts.Add(text2));
    }
}
