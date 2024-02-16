using System.CommandLine;
using Dax.Vpax.Obfuscator.Common;

namespace Dax.Vpax.Obfuscator.CLI;

internal class Program
{
    public static int Main(string[] args) => BuildCommand().Invoke(args);

    private static RootCommand BuildCommand()
    {
        var allowOverwriteOption = new Option<bool>(name: "--allow-overwrite")
        {
            Description = "Allow output files to be overwritten. If not provided, the command will fail if an output file already exists.",
            IsRequired = false,
        };
        var command = new RootCommand("VertiPaq-Analyzer obfuscator CLI");
        command.AddGlobalOption(allowOverwriteOption);
        command.AddCommand(BuildCommandObfuscate(allowOverwriteOption));
        command.AddCommand(BuildCommandDeobfuscate(allowOverwriteOption));
        return command;
    }

    private static Command BuildCommandObfuscate(Option<bool> allowOverwriteOption)
    {
        var vpaxOption = new Option<FileInfo>(name: "--vpax")
        {
            Description = "Path to the unobfuscated VPAX file.",
            IsRequired = true,
        };
        var dictionaryOption = new Option<FileInfo>(name: "--dictionary")
        {
            Description = "Path to the dictionary file to be used for incremental obfuscation. If not provided, a new dictionary will be created.",
            IsRequired = false,
        };
        var outputVpaxOption = new Option<FileInfo>(name: "--output-vpax")
        {
            Description = "Path to write the obfuscated VPAX file.",
            IsRequired = true,
        };
        var outputDictionaryOption = new Option<FileInfo>(name: "--output-dictionary")
        {
            Description = "Path to write the obfuscation dictionary file.",
            IsRequired = true,
        };

        var command = new Command("obfuscate", "Obfuscate the DaxModel.json file and delete all other contents from a VPAX file.");
        command.AddOption(vpaxOption);
        command.AddOption(dictionaryOption);
        command.AddOption(outputVpaxOption);
        command.AddOption(outputDictionaryOption);
        command.SetHandler(Obfuscate, vpaxOption, dictionaryOption, outputVpaxOption, outputDictionaryOption, allowOverwriteOption);
        return command;
    }

    private static Command BuildCommandDeobfuscate(Option<bool> allowOverwriteOption)
    {
        var vpaxOption = new Option<FileInfo>(name: "--vpax")
        {
            Description = "Path to the obfuscated VPAX file.",
            IsRequired = true,
        };
        var dictionaryOption = new Option<FileInfo>(name: "--dictionary")
        {
            Description = "Path to the dictionary file.",
            IsRequired = true,
        };
        var outputVpaxOption = new Option<FileInfo>(name: "--output-vpax")
        {
            Description = "Path to write the deobfuscated VPAX file.",
            IsRequired = true,
        };
        var command = new Command("deobfuscate", "Deobfuscate the DaxModel.json file from an obfuscated VPAX file using the provided dictionary.");
        command.AddOption(vpaxOption); 
        command.AddOption(dictionaryOption);
        command.AddOption(outputVpaxOption);
        command.SetHandler(Deobfuscate, vpaxOption, dictionaryOption, outputVpaxOption, allowOverwriteOption);
        return command;
    }

    private static void Obfuscate(FileInfo vpaxFile, FileInfo? dictionaryOption, FileInfo outputVpaxFile, FileInfo outputDictionaryFile, bool allowOverwrite)
    {
        using var stream = Read(vpaxFile);
        var obfuscator = new VpaxObfuscator();
        var outputDictionary = dictionaryOption is not null
            ? obfuscator.Obfuscate(stream, dictionary: ObfuscationDictionary.ReadFrom(dictionaryOption.FullName))
            : obfuscator.Obfuscate(stream);

        outputDictionary.WriteTo(outputDictionaryFile.FullName, overwrite: allowOverwrite, indented: true);
        Write(stream, outputVpaxFile, allowOverwrite);
    }

    private static void Deobfuscate(FileInfo vpaxFile, FileInfo dictionaryFile, FileInfo outputVpaxFile, bool allowOverwrite)
    {
        using var stream = Read(vpaxFile);
        var obfuscator = new VpaxObfuscator();

        obfuscator.Deobfuscate(stream, dictionary: ObfuscationDictionary.ReadFrom(dictionaryFile.FullName));
        Write(stream, outputVpaxFile, allowOverwrite);
    }

    private static MemoryStream Read(FileInfo file)
    {
        var buffer = File.ReadAllBytes(file.FullName);
        var stream = new MemoryStream();
        stream.Write(buffer, 0, buffer.Length);
        return stream;
    }

    private static void Write(MemoryStream stream, FileInfo file, bool allowOverwrite)
    {
        var bytes = stream.ToArray();
        var mode = allowOverwrite ? FileMode.Create : FileMode.CreateNew;
        using var fileStream = new FileStream(file.FullName, mode, FileAccess.Write, FileShare.Read);
        fileStream.Write(bytes, 0, bytes.Length);
    }
}
