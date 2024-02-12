using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator;

public interface IVpaxObfuscator
{
    /// <summary>
    /// Obfuscate the DaxModel.json file and delete all other contents from the provided VPAX stream.
    /// </summary>
    /// <param name="stream">The VPAX stream</param>
    /// <returns>The obfuscation dictionary</returns>
    ObfuscationDictionary Obfuscate(Stream stream);

    /// <summary>
    /// Obfuscate the DaxModel.json file and delete all other contents from the provided VPAX stream.
    /// </summary>
    /// <remarks>
    /// This method updates the obfuscation dictionary to changes applied since a previous obfuscation.
    /// </remarks>
    /// <param name="stream">The VPAX stream</param>
    /// <param name="dictionary">The obfuscation dictionary generated from a previous obfuscation</param>
    /// <returns>The obfuscation dictionary updated to changes applied since a previous obfuscation.</returns>
    ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary dictionary);
}
