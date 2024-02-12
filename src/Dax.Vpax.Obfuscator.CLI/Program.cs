using Dax.Vpax.Obfuscator.Common;
using System.CommandLine;

namespace Dax.Vpax.Obfuscator.CLI;

internal class Program
{
    public static int Main(string[] args)
    {
        var command = BuildCommand();
        return command.Invoke(args);
    }

    private static RootCommand BuildCommand()
    {
        var fileOption = new Option<FileInfo>("--file")
        {
            Description = "The full path to the VPAX file to obfuscate.",
            IsRequired = true,
        };
        fileOption.AddAlias("-f");

        var dictionaryOption = new Option<FileInfo>("--dictionary")
        {
            Description = "The full path to the JSON dictionary file to use for obfuscation.",
            IsRequired = false,
        };
        dictionaryOption.AddAlias("-d");

        var rootCommand = new RootCommand("VertiPaq-Analyzer obfuscator.");
        rootCommand.AddOption(fileOption);
        rootCommand.AddOption(dictionaryOption);
        rootCommand.SetHandler(Obfuscate, fileOption, dictionaryOption);

        return rootCommand;
    }

    private static void Obfuscate(FileInfo vpaxFile, FileInfo dictionaryFile)
    {
        var buffer = File.ReadAllBytes(vpaxFile.FullName);
        using var stream = new MemoryStream();
        stream.Write(buffer, 0, buffer.Length);

        var obfuscator = new VpaxObfuscator();
        var dictionary = dictionaryFile != null
            ? obfuscator.Obfuscate(stream, ObfuscationDictionary.ReadFrom(dictionaryFile.FullName))
            : obfuscator.Obfuscate(stream);

        var path = Path.Combine(vpaxFile.DirectoryName!, Path.ChangeExtension(vpaxFile.Name, ".dictionary.json"));
        dictionary.WriteTo(path, overwrite: false, indented: true);

        File.WriteAllBytes(vpaxFile.FullName, stream.ToArray());
    }
}
