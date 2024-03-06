using System.Text;
using Newtonsoft.Json;

namespace Dax.Vpax.Obfuscator.Common;

public sealed class ObfuscationDictionary
{
    private readonly Dictionary<string, ObfuscationText> _values;
    private readonly Dictionary<string, ObfuscationText> _obfuscated;

    [JsonConstructor]
    public ObfuscationDictionary(string id, ObfuscationText[] texts)
    {
        if (id == null || !Guid.TryParseExact(id, "D", out var guid) || guid == Guid.Empty)
            throw new ArgumentException("The dictionary identifier is not valid.", nameof(id));

        Id = id;
        Texts = texts.OrderBy((t) => t.Value).ToArray();

        // Create dictionaries to allow for fast lookups. This also ensures uniqueness of the keys by throwing if there are duplicates.
        _values = Texts.ToDictionary((text) => text.Value, StringComparer.OrdinalIgnoreCase);
        _obfuscated = Texts.ToDictionary((text) => text.Obfuscated, StringComparer.OrdinalIgnoreCase);
    }

    public string Id { get; }
    public IReadOnlyList<ObfuscationText> Texts { get; }

    public string GetValue(string obfuscated)
    {
        if (_obfuscated.TryGetValue(obfuscated, out var text))
            return text.Value;

        throw new KeyNotFoundException($"The obfuscated value was not found in the dictionary [{obfuscated}].");
    }

    public string GetObfuscated(string value)
    {
        if (_values.TryGetValue(value, out var text))
            return text.Obfuscated;

        throw new KeyNotFoundException($"The value was not found in the dictionary [{value}].");
    }

    public string? GetPlaintextOrValue(string obfuscated)
    {
        if (_obfuscated.TryGetValue(obfuscated, out var text))
            return text.Plaintext ?? text.Value;

        throw new KeyNotFoundException($"The obfuscated value was not found in the dictionary [{obfuscated}].");
    }

    public void WriteTo(string path, bool overwrite = false, bool indented = false)
    {
        var mode = overwrite ? FileMode.Create : FileMode.CreateNew;
        using (var stream = new FileStream(path, mode))
            WriteTo(stream, indented);
    }

    public void WriteTo(Stream stream, bool indented = false, bool leaveOpen = false)
    {
        var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true);
        var serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            Formatting = indented ? Formatting.Indented : Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
        });

        using (var streamWriter = new StreamWriter(stream, encoding, bufferSize: 1024, leaveOpen))
        using (var writer = new JsonTextWriter(streamWriter))
            serializer.Serialize(writer, this);

        if (leaveOpen) stream.Position = 0L;
    }

    public static ObfuscationDictionary ReadFrom(string path)
    {
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            return ReadFrom(stream);
    }

    public static ObfuscationDictionary ReadFrom(Stream stream, bool leaveOpen = false)
    {
        using (var streamReader = new StreamReader(stream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen))
        using (var reader = new JsonTextReader(streamReader))
        {
            var serializer = JsonSerializer.Create();
            return serializer.Deserialize<ObfuscationDictionary>(reader) ?? throw new InvalidOperationException("The deserialized dictionary is null.");
        }
    }
}
