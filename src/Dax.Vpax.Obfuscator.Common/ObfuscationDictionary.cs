﻿using System.Diagnostics.CodeAnalysis;
using System.Text;
using Newtonsoft.Json;

namespace Dax.Vpax.Obfuscator.Common;

public sealed class ObfuscationDictionary
{
    private readonly Dictionary<string, ObfuscationText> _values;
    private readonly Dictionary<string, ObfuscationText> _obfuscated;

    [JsonConstructor]
    public ObfuscationDictionary(string id, string version, IEnumerable<ObfuscationText> texts, IEnumerable<string> unobfuscatedValues)
    {
        if (id == null || !Guid.TryParseExact(id, "D", out var guid) || guid == Guid.Empty) throw new ArgumentException("The dictionary identifier is not valid.", nameof(id));
        if (version == null) throw new ArgumentNullException(nameof(version));
        if (texts == null) throw new ArgumentNullException(nameof(texts));

        Id = id;
        Version = version;
        Texts = texts.OrderBy((t) => t.Value).ToArray();
        UnobfuscatedValues = unobfuscatedValues?.ToArray() ?? [];

        // Create dictionaries to enable fast lookups and ensure key uniqueness. An error will be thrown if duplicate keys are detected.
        _values = Texts.ToDictionary((text) => text.Value, StringComparer.OrdinalIgnoreCase);
        _obfuscated = Texts.ToDictionary((text) => text.Obfuscated, StringComparer.OrdinalIgnoreCase);
    }

    public string Id { get; }
    public string Version { get; }
    public IReadOnlyList<ObfuscationText> Texts { get; }
    public IReadOnlyList<string> UnobfuscatedValues { get; }

    public string GetValue(string obfuscated)
    {
        if (_obfuscated.TryGetValue(obfuscated, out var text))
            return text.Value;

        throw new KeyNotFoundException($"The obfuscated value was not found in the dictionary [{obfuscated}].");
    }

    public bool TryGetValue(string obfuscated, [NotNullWhen(true)] out string? value)
    {
        if (_obfuscated.TryGetValue(obfuscated, out var text))
        {
            value = text.Value;
            return true;
        }

        value = null;
        return false;
    }

    public string GetObfuscated(string value)
    {
        if (_values.TryGetValue(value, out var text))
            return text.Obfuscated;

        throw new KeyNotFoundException($"The value was not found in the dictionary [{value}].");
    }

    public bool TryGetObfuscated(string value, [NotNullWhen(true)] out string? obfuscated)
    {
        if (_values.TryGetValue(value, out var text))
        {
            obfuscated = text.Obfuscated;
            return true;
        }

        obfuscated = null;
        return false;
    }

    public void WriteTo(string path, bool overwrite = false, bool indented = true)
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
