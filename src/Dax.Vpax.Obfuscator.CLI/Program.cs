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
            Description = "Path to write the obfuscated VPAX file. If not provided, the file will be written to the same folder as the '--vpax' file, using the default file extension for obfuscated VPAX files, which is '.ovpax'.",
            IsRequired = false,
        };
        var outputDictionaryOption = new Option<FileInfo>(name: "--output-dictionary")
        {
            Description = "Path to write the obfuscation dictionary file. If not provided, the file will be written to the same folder as the '--vpax' file, using the default file extension for obfuscation dictionary files, which is '.dict'.",
            IsRequired = false,
        };
        var trackUnobfuscatedOption = new Option<bool?>(name: "--track-unobfuscated")
        {
            Description = "Specifies whether to include unobfuscated values in the output dictionary.",
            IsRequired = false,
        };

        var command = new Command("obfuscate", "Obfuscate the DaxModel.json file and delete all other contents from a VPAX file.");
        command.AddOption(vpaxOption);
        command.AddOption(dictionaryOption);
        command.AddOption(outputVpaxOption);
        command.AddOption(outputDictionaryOption);
        command.AddOption(trackUnobfuscatedOption);
        command.SetHandler(Obfuscate, vpaxOption, dictionaryOption, outputVpaxOption, outputDictionaryOption, allowOverwriteOption, trackUnobfuscatedOption);
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
            Description = "Path to write the deobfuscated VPAX file. If not provided, the file will be written to the same folder as the '--vpax' file, using the default file extension for unobfuscated VPAX files, which is '.vpax'.",
            IsRequired = false,
        };
        var command = new Command("deobfuscate", "Deobfuscate the DaxModel.json file from an obfuscated VPAX file using the provided dictionary.");
        command.AddOption(vpaxOption); 
        command.AddOption(dictionaryOption);
        command.AddOption(outputVpaxOption);
        command.SetHandler(Deobfuscate, vpaxOption, dictionaryOption, outputVpaxOption, allowOverwriteOption);
        return command;
    }

    private static void Obfuscate(FileInfo vpaxFile, FileInfo? dictionaryOption, FileInfo? outputVpaxFile, FileInfo? outputDictionaryFile, bool allowOverwrite, bool? trackUnobfuscated)
    {
        using var stream = Read(vpaxFile);
        var obfuscator = new VpaxObfuscator();
        obfuscator.Options.TrackUnobfuscated = trackUnobfuscated ?? obfuscator.Options.TrackUnobfuscated;

        var outputDictionary = dictionaryOption is not null
            ? obfuscator.Obfuscate(stream, dictionary: ObfuscationDictionary.ReadFrom(dictionaryOption.FullName))
            : obfuscator.Obfuscate(stream);

        if (obfuscator.Options.TrackUnobfuscated)
            NotifyUnobfuscated(outputDictionary);
        
        var dictPath = outputDictionaryFile?.FullName ?? Path.ChangeExtension(vpaxFile.FullName, ".dict");
        var ovpaxPath = outputVpaxFile?.FullName ?? Path.ChangeExtension(vpaxFile.FullName, ".ovpax");

        outputDictionary.WriteTo(dictPath, overwrite: allowOverwrite, indented: true);
        Write(stream, ovpaxPath, allowOverwrite);
    }

    private static void Deobfuscate(FileInfo vpaxFile, FileInfo dictionaryFile, FileInfo outputVpaxFile, bool allowOverwrite)
    {
        using var stream = Read(vpaxFile);
        var obfuscator = new VpaxObfuscator();
        var ovpaxPath = outputVpaxFile?.FullName ?? Path.ChangeExtension(vpaxFile.FullName, ".vpax");

        obfuscator.Deobfuscate(stream, dictionary: ObfuscationDictionary.ReadFrom(dictionaryFile.FullName));
        Write(stream, ovpaxPath, allowOverwrite);
    }

    private static MemoryStream Read(FileInfo file)
    {
        var buffer = File.ReadAllBytes(file.FullName);
        var stream = new MemoryStream();
        stream.Write(buffer, 0, buffer.Length);
        return stream;
    }

    private static void Write(MemoryStream stream, string path, bool allowOverwrite)
    {
        var bytes = stream.ToArray();
        var mode = allowOverwrite ? FileMode.Create : FileMode.CreateNew;
        using var fileStream = new FileStream(path, mode, FileAccess.Write, FileShare.Read);
        fileStream.Write(bytes, 0, bytes.Length);
    }

    private static void NotifyUnobfuscated(ObfuscationDictionary dictionary)
    {
        _ = dictionary.UnobfuscatedValues ?? throw new InvalidOperationException($"{nameof(dictionary.UnobfuscatedValues)} is null.");
        if (dictionary.UnobfuscatedValues.Count == 0) return;

        var message = $"Obfuscation dictionary contains unobfuscated values. [{dictionary.UnobfuscatedValues.Count}]";

        // TODO: Add support for warning messages in ci/cd environments
        //var isGitHubAction = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";
        //var isAzurePipeline = Environment.GetEnvironmentVariable("TF_BUILD") == "True";
        //var isAppVeyor = Environment.GetEnvironmentVariable("APPVEYOR") == "True";

        Console.WriteLine($"\u001b[93m{message}\u001b[0m");
    }
}
