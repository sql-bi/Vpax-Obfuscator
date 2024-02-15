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
            var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.Empty.ToString("D"), texts: []));
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

            var exception = Assert.Throws<ArgumentException>(() => new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts));
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
        public void TryGetValue_ExistingObfuscated_ReturnsTrueAndValue()
        {
            var dictionary = CreateTestDictionary();
            var result = dictionary.TryGetValue("XXXXXX", out var value);
            Assert.True(result);
            Assert.Equal("VALUE1", value);
        }

        [Fact]
        public void TryGetValue_NonExistingObfuscated_ReturnsFalseAndNullValue()
        {
            var dictionary = CreateTestDictionary();
            var result = dictionary.TryGetValue("ZZZZZZ", out var value);
            Assert.False(result);
            Assert.Null(value);
        }

        [Fact]
        public void TryGetObfuscated_ExistingValue_ReturnsTrueAndObfuscated()
        {
            var dictionary = CreateTestDictionary();
            var result = dictionary.TryGetObfuscated("VALUE1", out var obfuscated);
            Assert.True(result);
            Assert.Equal("XXXXXX", obfuscated);
        }

        [Fact]
        public void TryGetObfuscated_NonExistingValue_ReturnsFalseAndNullObfuscated()
        {
            var dictionary = CreateTestDictionary();
            var result = dictionary.TryGetObfuscated("ZZZZZZ", out var obfuscated);
            Assert.False(result);
            Assert.Null(obfuscated);
        }

        [Fact]
        public void WriteTo_Path_WritesDictionaryToFile()
        {
            var dictionary = CreateDictionary(
            [
                new ObfuscationText("VALUE1", "XXXXXX"),
                new ObfuscationText("VALUE2", "YYYYYY"),
            ]);

            var path = Path.GetTempFileName();
            try
            {
                dictionary.WriteTo(path, overwrite: true);
                var actual = ObfuscationDictionary.ReadFrom(path);

                Assert.Equal(dictionary.Texts.Count, actual.Texts.Count);
                Assert.True(dictionary.TryGetValue("XXXXXX", out var value1) && value1.Equals("VALUE1"));
                Assert.True(dictionary.TryGetValue("YYYYYY", out var value2) && value2.Equals("VALUE2"));
                Assert.True(dictionary.TryGetObfuscated("VALUE1", out var value3) && value3.Equals("XXXXXX"));
                Assert.True(dictionary.TryGetObfuscated("VALUE2", out var value4) && value4.Equals("YYYYYY"));
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
                new ObfuscationText("VALUE2", "YYYYYY"),
            ]);

            using var stream = new MemoryStream();
            dictionary.WriteTo(stream, leaveOpen: true);
            var actual = ObfuscationDictionary.ReadFrom(stream);

            Assert.Equal(dictionary.Texts.Count, actual.Texts.Count);
            Assert.True(dictionary.TryGetValue("XXXXXX", out var value1) && value1.Equals("VALUE1"));
            Assert.True(dictionary.TryGetValue("YYYYYY", out var value2) && value2.Equals("VALUE2"));
            Assert.True(dictionary.TryGetObfuscated("VALUE1", out var value3) && value3.Equals("XXXXXX"));
            Assert.True(dictionary.TryGetObfuscated("VALUE2", out var value4) && value4.Equals("YYYYYY"));
        }

        private ObfuscationDictionary CreateTestDictionary()
        {
            return CreateDictionary(
            [
                new ObfuscationText("VALUE1", "XXXXXX"),
                new ObfuscationText("VALUE2", "YYYYYY"),
            ]);
        }

        private ObfuscationDictionary CreateDictionary(ObfuscationText[] texts)
        {
            return new ObfuscationDictionary(id: Guid.NewGuid().ToString("D"), texts);
        }
    }
}
