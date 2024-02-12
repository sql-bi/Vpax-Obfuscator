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
}
