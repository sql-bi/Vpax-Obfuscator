using Xunit;

namespace Dax.Vpax.Obfuscator.Common.Tests;

public class ObfuscationDictionaryTests
{
    [Fact]
    public void ctor_EmptyTexts_ReturnsEmptyDictionary()
    {
        var dictionary = new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), "0.0.0-test", texts: [], unobfuscatedValues: []);
        Assert.Empty(dictionary.Texts);
        Assert.Empty(dictionary.UnobfuscatedValues);
    }

    [Fact]
    public void ctor_EmptyId_Throws()
    {
        var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.Empty.ToString("D"), "0.0.0-test", texts: [], unobfuscatedValues: []));
        Assert.StartsWith("The dictionary identifier is not valid.", exception.Message);
    }

    [Fact]
    public void ctor_DuplicateTexts_Throws()
    {
        var texts = new[]
        {
            new ObfuscationText("VALUE", "XXXXXX"),
            new ObfuscationText("VALUE", "XXXXXX"),
        };

        var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), "0.0.0-test", texts, unobfuscatedValues: []));
        Assert.StartsWith("An item with the same key has already been added.", exception.Message);
    }

    [Fact]
    public void Count_ReturnsNumberOfTexts()
    {
        var dictionary = CreateTestDictionary();
        Assert.Equal(2, dictionary.Texts.Count);
    }

    [Fact]
    public void GetValue_ExistingObfuscated_ReturnsValue()
    {
        var dictionary = CreateTestDictionary();
        var value = dictionary.GetValue("XXXXXX");
        Assert.Equal("VALUE1", value);
    }

    [Fact]
    public void GetValue_NonExistingObfuscated_ThrowsKeyNotFoundException()
    {
        var dictionary = CreateTestDictionary();
        Assert.Throws<KeyNotFoundException>(() => dictionary.GetValue("ZZZZZZ"));
    }

    [Fact]
    public void GetObfuscated_ExistingValue_ReturnsObfuscated()
    {
        var dictionary = CreateTestDictionary();
        var obfuscated = dictionary.GetObfuscated("VALUE1");
        Assert.Equal("XXXXXX", obfuscated);
    }

    [Fact]
    public void GetObfuscated_NonExistingValue_ThrowsKeyNotFoundException()
    {
        var dictionary = CreateTestDictionary();
        Assert.Throws<KeyNotFoundException>(() => dictionary.GetObfuscated("ZZZZZZ"));
    }

    [Fact]
    public void WriteTo_Path_WritesDictionary()
    {
        var dictionary = CreateDictionary(
        [
            new ObfuscationText("VALUE1", "XXXXXX"),
        ]);

        var path = Path.GetTempFileName();
        try
        {
            dictionary.WriteTo(path, overwrite: true);
            var actual = ObfuscationDictionary.ReadFrom(path);

            Assert.Equal(dictionary.Texts.Count, actual.Texts.Count);

            var value = dictionary.GetValue("XXXXXX");
            var obfuscated = dictionary.GetObfuscated("VALUE1");

            Assert.Equal("VALUE1", value);
            Assert.Equal("XXXXXX", obfuscated);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void WriteTo_Stream_WritesDictionary()
    {
        var dictionary = CreateDictionary(
        [
            new ObfuscationText("VALUE1", "XXXXXX"),
        ]);

        using var stream = new MemoryStream();
        dictionary.WriteTo(stream, leaveOpen: true);
        var actual = ObfuscationDictionary.ReadFrom(stream);

        Assert.Equal(dictionary.Texts.Count, actual.Texts.Count);

        var value = dictionary.GetValue("XXXXXX");
        var obfuscated = dictionary.GetObfuscated("VALUE1");

        Assert.Equal("VALUE1", value);
        Assert.Equal("XXXXXX", obfuscated);
    }

    private static ObfuscationDictionary CreateTestDictionary()
    {
        return CreateDictionary(
        [
            new ObfuscationText("VALUE1", "XXXXXX"),
            new ObfuscationText("VALUE2", "YYYYYY"),
        ]);
    }

    private static ObfuscationDictionary CreateDictionary(ObfuscationText[] texts)
    {
        return new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), "0.0.0-test", texts, unobfuscatedValues: []);
    }
}
