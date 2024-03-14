using System.IO.Packaging;
using Dax.Metadata;
using Dax.Vpax.Obfuscator.Common;
using Dax.Vpax.Tools;

namespace Dax.Vpax.Obfuscator;

public sealed class VpaxObfuscator : IVpaxObfuscator
{
    public static string Version { get; } = ThisAssembly.AssemblyInformationalVersion;
    public ObfuscationOptions Options { get; } = new();

    public ObfuscationDictionary Obfuscate(Stream stream) => ObfuscateImpl(Options, stream);
    public ObfuscationDictionary Obfuscate(Model model) => ObfuscateImpl(Options, model);
    public ObfuscationDictionary Obfuscate(Stream stream, ObfuscationDictionary dictionary) => ObfuscateImpl(Options, stream, dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
    public ObfuscationDictionary Obfuscate(Model model, ObfuscationDictionary dictionary) => ObfuscateImpl(Options, model, dictionary ?? throw new ArgumentNullException(nameof(dictionary)));
    public void Deobfuscate(Stream stream, ObfuscationDictionary dictionary) => DeobfuscateImpl(stream, dictionary);
    public void Deobfuscate(Model model, ObfuscationDictionary dictionary) => DeobfuscateImpl(model, dictionary);

    private static ObfuscationDictionary ObfuscateImpl(ObfuscationOptions options, Stream stream, ObfuscationDictionary? dictionary = null)
    {
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        var model = GetModel(stream);
        var result = ObfuscateImpl(options, model, dictionary);

        ZeroOutPackage(stream); // Zero out the package to remove all contents before writing the obfuscated DaxModel.json
        VpaxTools.ExportVpax(stream, model, viewVpa: null, database: null);
        return result;
    }

    private static ObfuscationDictionary ObfuscateImpl(ObfuscationOptions options, Model model, ObfuscationDictionary? dictionary = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        var obfuscator = dictionary == null
            ? new DaxModelObfuscator(options, model)
            : new DaxModelObfuscator(options, model, dictionary);

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
