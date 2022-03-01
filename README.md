# Yarhl: Yarhl, A format ResearcH Library ![awesomeness](https://img.shields.io/badge/SceneGate-awesome%20%F0%9F%95%B6-blue?logo=csharp)

_Formats implementation just for fun._

![Yarhl Logo](https://raw.githubusercontent.com/SceneGate/Yarhl/develop/docs/images/logo.png)

> **Yarhl** is a set of libraries to **implement and convert file formats**. It
> provides a virtual file system, format conversion APIs, full feature binary IO
> and plugin support to support common formats. It's built in **C# / .NET** and
> works in Windows, Linux and Mac OS X.

<!-- prettier-ignore -->
| Stable | [![Yarhl](https://img.shields.io/nuget/v/Yarhl?label=Yarhl)](https://www.nuget.org/packages/Yarhl) |
|------------------| ------ |
| **Preview** | [![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/SceneGate/Yarhl/latest?sort=semver)](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview) |
| **Build & Test** | ![Build and release](https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release/badge.svg?branch=develop) |
| **Open source!** | [![CII Best Practices](https://bestpractices.coreinfrastructure.org/projects/2919/badge)](https://bestpractices.coreinfrastructure.org/projects/2919) [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/) |

## Features

- :books: Format implementation architecture and guidelines
- :recycle: Format conversion API
- :open_file_folder: Virtual file system with format transformations
- :1234: Binary IO
  - Custom `Stream` with sub-stream supports (memory efficiency!)
  - Full feature binary and text readers and writers
  - Simple binary (de)serializer
- :page_with_curl: Text formats
  - Industry-standard translation format: PO
  - Table text replaces
  - Common encodings: euc-jp, token-escaped encoding
  - Base class for quick and simple encoding implementations

## Getting started guide

Check out the
[getting started guide](https://scenegate.github.io/Yarhl/guides/getting-started/introduction.html)
to start using the full power of _Yarhl_. Below you have a code snippet to show
how easy is to implement a new container format and convert and navigate through
it.

```csharp
// Implement a new format container from binary (file) into a virtual file system.
public class BinaryArchive2Container : IConverter<IBinary, NodeContainerFormat>
{
    public NodeContainerFormat Convert(IBinary source)
    {
        var reader = new DataReader(source.Stream);
        var container = new NodeContainerFormat();

        // Format: table with "name + offset + size", then file data.
        // The offset to the first file give us the number of entries.
        int numFiles = reader.ReadInt32() / 0x18;
        reader.Stream.Position = 0;

        for (int i = 0; i < numFiles; i++) {
            string name = reader.ReadString(0x10); // 16 bytes for name
            uint offset = reader.ReadUInt32();
            uint size = reader.ReadUInt32();

            // Create a substream for the child, a stream from a region
            // of the parent stream without making any read/write or copies.
            Node node = NodeFactory.FromSubstream(name, source.Stream, offset, size);
            container.Root.Add(node);
        }

        return container;
    }
}

// Unpack a child from the container file.
Node dataNode = NodeFactory.FromFile("file.bin", FileOpenMode.Read)
    .TransformWith<BinaryArchive2Container>() // Binary -> container (virtual file system)
    .Children["data"].Children["text.json"]   // Navigate the children
    .Stream.WriteTo("text.json");             // Export to the disk
```

Feel free to ask any question in the
[project Discussion site](https://github.com/SceneGate/Yarhl/discussions) and
check the complete documentation [here](https://scenegate.github.io/Yarhl/).

## Usage

This project provides the following libraries as NuGet packages (via nuget.org).
The libraries only support the latest version of .NET and its LTS: **.NET 6.0**.

- [![Yarhl](https://img.shields.io/nuget/v/Yarhl?label=Yarhl&logo=nuget)](https://www.nuget.org/packages/Yarhl):
  core, format conversion, file system and binary reading / writing (IO).
- [![Yarhl.Media](https://img.shields.io/nuget/v/Yarhl.Media?label=Yarhl.Media&logo=nuget)](https://www.nuget.org/packages/Yarhl.Media):
  text formats (Po) and encodings.

Preview releases can be found in this
[Azure DevOps package repository](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview).
To use a preview release, create a file `nuget.config` in the same directory of
your solution file (.sln) with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="SceneGate-Preview" value="https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

Then restore / install as usual via Visual Studio, Rider or command-line.

## Contributing

The repository requires to build .NET 6.0 SDK and .NET Framework 4.8 or latest
Mono (for DocFX). If you open the project with VS Code and you did install the
[VS Code Remote Containers](https://code.visualstudio.com/docs/remote/containers)
extension, you can have an already pre-configured development environment with
Docker or Podman.

To build, test and generate artifacts run:

```sh
# Only required the first time
dotnet tool restore

# Default target is Stage-Artifacts
dotnet cake
```

To just build and test quickly, run:

```sh
dotnet cake --target=BuildTest
```

Additionally you can use _Visual Studio_ or _JetBrains Rider_ as any other .NET
project.

To contribute follow the [contributing guidelines](CONTRIBUTING.md).

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/).
