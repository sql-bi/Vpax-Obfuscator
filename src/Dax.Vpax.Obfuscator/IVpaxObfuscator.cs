using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator;

public interface IVpaxObfuscator
{
    /// <summary>
    /// Obfuscate the DaxModel.json file and delete all other contents from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary generated from a previous obfuscation</param>
    /// <returns>The obfuscation dictionary generated from the obfuscation process</returns>
    ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary? dictionary = null);

    /// <summary>
    /// Obfuscate the <paramref name="model"/>.
    /// </summary>
    /// <param name="model">The model to obfuscate</param>
    /// <param name="dictionary">The obfuscation dictionary generated from a previous obfuscation</param>
    /// <returns>The obfuscation dictionary generated from the obfuscation process</returns>
    ObfuscationDictionary Obfuscate(Model model, ObfuscationDictionary? dictionary = null);

    /// <summary>
    /// Deobfuscate the DaxModel.json file using the provided obfuscation dictionary.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary</param>
    void Deobfuscate(Stream stream, ObfuscationDictionary dictionary);

    /// <summary>
    /// Deobfuscate the <paramref name="model"/> using the provided obfuscation dictionary.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary</param>
    void Deobfuscate(Model model, ObfuscationDictionary dictionary);
}
