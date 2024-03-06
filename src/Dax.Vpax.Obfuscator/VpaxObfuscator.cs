using System.IO.Packaging;
using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Tools;

namespace Dax.Vpax.Obfuscator;

public sealed class VpaxObfuscator : IVpaxObfuscator
{
    public ObfuscationDictionary Obfuscate(Stream stream) => ObfuscateImpl(stream, dictionary: null);
    public ObfuscationDictionary Obfuscate(Model model) => ObfuscateImpl(model, dictionary: null);
    public ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary dictionary) => ObfuscateImpl(stream, dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
    public ObfuscationDictionary Obfuscate(Model model, ObfuscationDictionary dictionary) => ObfuscateImpl(model, dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
    public void Deobfuscate(Stream stream, ObfuscationDictionary dictionary) => DeobfuscateImpl(stream, dictionary);
    public void Deobfuscate(Model model, ObfuscationDictionary dictionary) => DeobfuscateImpl(model, dictionary);

    private static ObfuscationDictionary ObfuscateImpl(Stream stream, ObfuscationDictionary? dictionary)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var model = GetModel(stream);
        var result = ObfuscateImpl(model, dictionary);

        ZeroOutPackage(stream); // Zero out the package to remove all contents before writing the obfuscated DaxModel.json
        VpaxTools.ExportVpax(stream, model, viewVpa: null, database: null);

        return result;
    }

    private static ObfuscationDictionary ObfuscateImpl(Model model, ObfuscationDictionary? dictionary)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        var obfuscator = new DaxModelObfuscator(model, dictionary);
        return obfuscator.Obfuscate();
    }

    private static void DeobfuscateImpl(Stream stream, ObfuscationDictionary dictionary)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

        var model = GetModel(stream);
        DeobfuscateImpl(model, dictionary);
        VpaxTools.ExportVpax(stream, model, viewVpa: null, database: null);
    }

    private static void DeobfuscateImpl(Model model, ObfuscationDictionary dictionary)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));
        if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

        var deobfuscator = new DaxModelDeobfuscator(model, dictionary);
        deobfuscator.Deobfuscate();
    }

    private static Model GetModel(Stream stream)
    {
        var model = VpaxTools.ImportVpax(stream, importDatabase: false).DaxModel;
        return model ?? throw new InvalidOperationException($"The VPAX package does not contain a {VpaxFormat.DAXMODEL} file.");
    }

    private static void ZeroOutPackage(Stream stream)
    {
        using var package = Package.Open(stream, FileMode.Open, FileAccess.ReadWrite);

        foreach (var part in package.GetParts().ToArray())
            package.DeletePart(part.Uri);

        package.Close();
    }
}
