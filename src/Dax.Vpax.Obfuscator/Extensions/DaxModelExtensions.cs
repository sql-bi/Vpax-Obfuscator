using System.Diagnostics;
using Dax.Metadata;

namespace Dax.Vpax.Obfuscator.Extensions;

internal static class DaxModelExtensions
{
    public static bool IsObfuscated(this Model model)
    { 
        return model.ObfuscatorDictionaryId != null
            || model.ObfuscatorLib != null
            || model.ObfuscatorLibVersion != null;
    }

    public static bool IsObfuscatedWithDictionaryId(this Model model, string dictionaryId)
    {
        Debug.Assert(IsObfuscated(model));
        return string.Equals(model.ObfuscatorDictionaryId, dictionaryId, StringComparison.OrdinalIgnoreCase);
    }
}
