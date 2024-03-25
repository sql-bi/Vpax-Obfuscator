# VertiPaq-Analyzer Obfuscator

VertiPaq Analyzer Obfuscator is a .NET library that enables the obfuscation of VertiPaq Analyzer files.

You can read more [here](https://www.sqlbi.com/blog/marco/2024/03/15/vpax-obfuscator-a-library-to-obfuscate-vpax-files).

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/Dax.Vpax.Obfuscator). Just search for *Dax.Vpax.Obfuscator* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package Dax.Vpax.Obfuscator
```

## Usage

The library can be used in any .NET application built with `net462`, `netcoreapp3.1`, `net5.0` or later.

**Obfuscation**

```csharp
using var vpax = new MemoryStream(File.ReadAllBytes(@"C:\path\to\unobfuscated.vpax"));
var obfuscator = new VpaxObfuscator();
var dictionary = obfuscator.Obfuscate(vpax);

dictionary.WriteTo(@"C:\path\to\dictionary.dict");
File.WriteAllBytes(@"C:\path\to\obfuscated.ovpax", vpax.ToArray());
```

**Deobfuscation**

```csharp
using var vpax = new MemoryStream(File.ReadAllBytes(@"C:\path\to\obfuscated.ovpax"));
var dictionary = ObfuscationDictionary.ReadFrom(@"C:\path\to\dictionary.dict");
var obfuscator = new VpaxObfuscator();
obfuscator.Deobfuscate(vpax, dictionary);

File.WriteAllBytes(@"C:\path\to\deobfuscated.vpax", vpax.ToArray());
```

**Incremental Obfuscation**

> Incremental obfuscation keeps the same obfuscated names across different VPAX versions of the same model.

```csharp
using var vpax = new MemoryStream(File.ReadAllBytes(@"C:\path\to\unobfuscated-v2.vpax"));
var dictionaryV1 = ObfuscationDictionary.ReadFrom(@"C:\path\to\dictionary-v1.dict");
var obfuscator = new VpaxObfuscator();
var dictionary = obfuscator.Obfuscate(vpax, dictionaryV1);

dictionary.WriteTo(@"C:\path\to\dictionary-v2.dict");
File.WriteAllBytes(@"C:\path\to\obfuscated-v2.ovpax", vpax.ToArray());
```

## CLI

A command-line interface is also available for the obfuscator. The CLI is available as a standalone executable, which can be downloaded from the [releases page](https://github.com/sql-bi/Vpax-Obfuscator/releases/latest).

```cmd
C:\> vpax-obfuscator.exe [command] [options]
```

For usage and help content for any command, pass in the `-h` parameter, for example:

```cmd
C:\> vpax-obfuscator.exe obfuscate -h

Description:
  Obfuscate the DaxModel.json file and delete all other contents from a VPAX file.

Usage:
  vpax-obfuscator obfuscate [options]

Options:
  --vpax <vpax> (REQUIRED)                 Path to the unobfuscated VPAX file.
  --dictionary <dictionary>                Path to the dictionary file to be used for incremental obfuscation. If not provided, a new dictionary will be created.
  --output-vpax <output-vpax>              Path to write the obfuscated VPAX file. If not provided, the file will be written to the same folder as the '--vpax' file, using the default file extension for obfuscated VPAX files, which is '.ovpax'.
  --output-dictionary <output-dictionary>  Path to write the obfuscation dictionary file. If not provided, the file will be written to the same folder as the '--vpax' file, using the default file extension for obfuscation dictionary files, which is '.dict'.
  --track-unobfuscated                     Specifies whether to include unobfuscated values in the output dictionary.
  --allow-overwrite                        Allow output files to be overwritten. If not provided, the command will fail if an output file already exists.
  -?, -h, --help                           Show help and usage information
```
