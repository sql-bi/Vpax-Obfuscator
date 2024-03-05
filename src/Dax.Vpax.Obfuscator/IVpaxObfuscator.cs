using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator;

public interface IVpaxObfuscator
{
    /// <summary>
    /// Obfuscate the DaxModel.json file and delete all other contents from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <returns>The obfuscation dictionary generated from the obfuscation process</returns>
    ObfuscationDictionary Obfuscate(Stream stream);

    /// <summary>
    /// Obfuscate the <paramref name="model"/>.
    /// </summary>
    /// <returns>The obfuscation dictionary generated from the obfuscation process</returns>
    ObfuscationDictionary Obfuscate(Model model);

    /// <summary>
    /// Incrementally obfuscate the DaxModel.json file and delete all other contents from the <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary generated from a previous obfuscation</param>
    /// <returns>The obfuscation <paramref name="dictionary"/> updated to changes applied since a previous obfuscation.</returns>
    ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary dictionary);

    /// <summary>
    /// Incrementally obfuscate the <paramref name="model"/>.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary generated from a previous obfuscation</param>
    /// <returns>The obfuscation <paramref name="dictionary"/> updated to changes applied since a previous obfuscation.</returns>
    ObfuscationDictionary Obfuscate(Model model, ObfuscationDictionary dictionary);

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
