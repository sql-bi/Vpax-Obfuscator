using Xunit;

namespace Dax.Vpax.Obfuscator.Common.Tests
{
    public class ObfuscationDictionaryTests
    {
        [Fact]
        public void ctor_EmptyTexts_ReturnsEmptyDictionary()
        {
            var dictionary = new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts: []);
            Assert.Empty(dictionary.Texts);
        }

        [Fact]
        public void ctor_EmptyId_Throws()
        {
#if NETFRAMEWORK
            var expectedMessage = "The dictionary identifier is not valid.\r\nParameter name: id";
#else
            var expectedMessage = "The dictionary identifier is not valid. (Parameter 'id')";
#endif
            var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.Empty.ToString("D"), texts: []));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void ctor_DuplicateTexts_Throws()
        {
#if NETFRAMEWORK
            var expectedMessage = "An item with the same key has already been added.";
#else
            var expectedMessage = "An item with the same key has already been added. Key: VALUE";
#endif
            var texts = new[]
            {
                new ObfuscationText("VALUE", "XXXXXX"),
                new ObfuscationText("VALUE", "XXXXXX"),
            };

            var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts));
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void Count_ReturnsNumberOfTexts()
        {
            var dictionary = CreateTestDictionary();
            Assert.Equal(2, dictionary.Texts.Count);
        }

        [Fact]
        public void TryGetValue_ExistingObfuscated_ReturnsValue()
        {
            var dictionary = CreateTestDictionary();
            Assert.True(dictionary.TryGetValue("XXXXXX", out var value));
            Assert.Equal("VALUE1", value);
        }

        [Fact]
        public void TryGetValue_NonExistingObfuscated_ReturnsFalse()
        {
            var dictionary = CreateTestDictionary();
            Assert.False(dictionary.TryGetValue("ZZZZZZ", out _));
        }

        [Fact]
        public void TryGetObfuscated_ExistingValue_ReturnsObfuscated()
        {
            var dictionary = CreateTestDictionary();
            Assert.True(dictionary.TryGetObfuscated("VALUE1", out var obfuscated));
            Assert.Equal("XXXXXX", obfuscated);
        }

        [Fact]
        public void TryGetObfuscated_NonExistingValue_ReturnsFalse()
        {
            var dictionary = CreateTestDictionary();
            Assert.False(dictionary.TryGetObfuscated("ZZZZZZ", out _));
        }

        [Fact]
        public void WriteTo_Stream_WritesDictionary()
        {
            var expected = CreateDictionary(new[]
            {
                new ObfuscationText("VALUE1", "XXXXXX"),
                new ObfuscationText("VALUE2", "YYYYYY"),
            });

            using var stream = new MemoryStream();
            expected.WriteTo(stream, leaveOpen: true);
            var actual = ObfuscationDictionary.ReadFrom(stream);

            Assert.Equal(expected.Texts.Count, actual.Texts.Count);
            Assert.True(expected.TryGetValue("XXXXXX", out var value1) && value1.Equals("VALUE1"));
            Assert.True(expected.TryGetValue("YYYYYY", out var value2) && value2.Equals("VALUE2"));
            Assert.True(expected.TryGetObfuscated("VALUE1", out var value3) && value3.Equals("XXXXXX"));
            Assert.True(expected.TryGetObfuscated("VALUE2", out var value4) && value4.Equals("YYYYYY"));
        }

        private ObfuscationDictionary CreateTestDictionary()
        {
            return CreateDictionary(new[]
            {
                new ObfuscationText("VALUE1", "XXXXXX"),
                new ObfuscationText("VALUE2", "YYYYYY"),
            });
        }

        private ObfuscationDictionary CreateDictionary(ObfuscationText[] texts)
        {
            return new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts);
        }
    }
}
