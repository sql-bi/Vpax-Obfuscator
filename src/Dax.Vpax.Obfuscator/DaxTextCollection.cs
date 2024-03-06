using System.Collections;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Obfuscator.Comparers;

namespace Dax.Vpax.Obfuscator;

internal sealed class DaxTextCollection: ICollection<DaxText>
{
    private readonly Dictionary<DaxText, DaxText> _plaintexts = new(DaxTextValueEqualityComparer.Instance);
    private readonly Dictionary<DaxText, DaxText> _obfuscated = new(DaxTextObfuscatedValueEqualityComparer.Instance);

    public DaxTextCollection(ObfuscationDictionary? dictionary = null)
    {
        if (dictionary != null)
        {
            IsIncrementalObfuscation = true;
            foreach (var text in dictionary.Texts)
                Add(new DaxText(text));
        }
    }

    public bool IsIncrementalObfuscation { get; }

    public int Count => _plaintexts.Count;

    public bool IsReadOnly => false;

    public void Add(DaxText item)
    {
        if (item.ObfuscatedValue == null) throw new InvalidOperationException("The obfuscated value is null.");

        _plaintexts.Add(item, item);
        _obfuscated.Add(item, item);
    }

    public void Clear() => throw new NotSupportedException();

    public bool Contains(DaxText item) => _obfuscated.ContainsKey(item);

    public void CopyTo(DaxText[] array, int arrayIndex) => throw new NotSupportedException();

    public IEnumerator<DaxText> GetEnumerator() => _plaintexts.Values.GetEnumerator();

    public bool TryGet(DaxText text, out DaxText? obfuscatedText) => _plaintexts.TryGetValue(text, out obfuscatedText);

    public bool Remove(DaxText item) => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
