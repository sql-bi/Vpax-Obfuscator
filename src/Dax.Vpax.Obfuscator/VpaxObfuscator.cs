using System.IO.Packaging;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Tools;

namespace Dax.Vpax.Obfuscator;

public sealed class VpaxObfuscator : IVpaxObfuscator
{
    /// <inheritdoc/>
    public ObfuscationDictionary Obfuscate(Stream stream)
        => ObfuscateImpl(stream, dictionary: null);

    /// <inheritdoc/>
    public ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary dictionary)
        => ObfuscateImpl(stream, dictionary ?? throw new ArgumentNullException(nameof(dictionary)));

    private ObfuscationDictionary ObfuscateImpl(Stream stream, ObfuscationDictionary? dictionary)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var model = VpaxTools.ImportVpax(stream, importDatabase: false).DaxModel;
        if (model == null) throw new InvalidOperationException($"The VPAX package does not contain a {VpaxFormat.DAXMODEL} file.");

        // Zero out the package to remove all contents before writing the obfuscated DaxModel.json
        ZeroOutPackage(stream);

        var obfuscator = new DaxModelObfuscator(model, dictionary);
        obfuscator.Obfuscate();
        var texts = obfuscator.Texts.Select((t) => new ObfuscationText(t.Value, t.ObfuscatedValue)).ToArray();
        var result = new ObfuscationDictionary(id: model.ObfuscatorDictionaryId, texts);

        VpaxTools.ExportVpax(stream, model, viewVpa: null, database: null);
        return result;
    }

    private void DeobfuscateImpl(Stream stream, ObfuscationDictionary dictionary)
    {
        throw new NotImplementedException();

        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

        var model = VpaxTools.ImportVpax(stream, importDatabase: false).DaxModel;
        if (model == null) throw new InvalidOperationException($"The VPAX package does not contain a {VpaxFormat.DAXMODEL} file.");

        //var deobfuscator = new DaxModelDeobfuscator(model);
        //obfuscator.Obfuscate();

        VpaxTools.ExportVpax(stream, model, viewVpa: null, database: null);
    }

    private static void ZeroOutPackage(Stream stream)
    {
        using var package = Package.Open(stream, FileMode.Open, FileAccess.ReadWrite);

        foreach (var part in package.GetParts().ToArray())
            package.DeletePart(part.Uri);

        package.Close();
    }
}
