# VertiPaq-Analyzer Obfuscator

VertiPaq Analyzer Obfuscator is an integration library for .NET, enabling the obfuscation of VertiPaq Analyzer files.

### Installation

The library is available on [NuGet](https://www.nuget.org/packages/Dax.Vpax.Obfuscator). Just search for *Dax.Vpax.Obfuscator* in the **Package Manager GUI** or run the following command in the **.NET CLI**:

```shell
dotnet add package Dax.Vpax.Obfuscator
```

## Usage

The library can be used in any .NET application built with `net462`, `netcoreapp3.1`, `net5.0` or later.

**Obfuscation**

```csharp
using var vpax = File.Open(@"C:\path\to\file.vpax", FileMode.Open);
var obfuscator = new VpaxObfuscator();
var dictionary = obfuscator.Obfuscate(vpax);
dictionary.WriteTo(@"C:\path\to\dictionary.json");
```

**Deobfuscation**

```csharp
using var vpax = File.Open(@"C:\path\to\obfuscated.vpax", FileMode.Open);
var dictionary = ObfuscationDictionary.ReadFrom(@"C:\path\to\dictionary.json");
var obfuscator = new VpaxObfuscator();
obfuscator.Deobfuscate(vpax, dictionary);
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
  --vpax <vpax> (REQUIRED)                            Path to the unobfuscated VPAX file.
  --dictionary <dictionary>                           Path to the dictionary file to be used for incremental obfuscation. If not provided, a new dictionary will be created.
  --output-vpax <output-vpax> (REQUIRED)              Path to write the obfuscated VPAX file.
  --output-dictionary <output-dictionary> (REQUIRED)  Path to write the obfuscation dictionary file.
  --track-unobfuscated                                Specifies whether to include unobfuscated values in the output dictionary.
  --allow-overwrite                                   Allow output files to be overwritten. If not provided, the command will fail if an output file already exists.
  -?, -h, --help                                      Show help and usage information
```
