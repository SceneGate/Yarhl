# Yarhl, A format ResearcH Library ![SceneGate awesome](https://img.shields.io/badge/SceneGate-awesome%20%F0%9F%95%B6-blue?logo=csharp)

![Yarhl logo](./images/logo-large.png)

<!-- markdownlint-disable MD033 -->
<p align="center">
  <a href="https://www.nuget.org/packages?q=Yarhl">
    <img
      alt="Stable version"
      src="https://img.shields.io/nuget/v/Yarhl?label=Stable"
    />
  </a>
  &nbsp;
  <a href="https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview">
    <img
      alt="GitHub commits since latest release (by SemVer)"
      src="https://img.shields.io/github/commits-since/SceneGate/Yarhl/latest?sort=semver"
    />
  </a>
  &nbsp;
  <a href="https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release">
    <img
      alt="Build and release"
      src="https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release/badge.svg?branch=develop"
    />
  </a>
  &nbsp;
  <a href="https://bestpractices.coreinfrastructure.org/projects/2919">
    <img
      alt="CII Best Practices"
      src="https://bestpractices.coreinfrastructure.org/projects/2919/badge"
    />
  </a>
  &nbsp;
  <a href="https://choosealicense.com/licenses/mit/">
    <img
      alt="MIT License"
      src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat"
    />
  </a>
  &nbsp;
</p>

_Yarhl_ is a set of libraries that helps to **implement and convert file
formats** It empowers you with...

- ♻️ ... APIs to easily **convert** between custom formats.
- 📚 ... **guidelines** to implement and test custom format converters.
- 🔢 ... advance **binary and text** reading / writing, encoding and
  serialization.
- 📃 ... **standard formats** implementation like **PO** for translations.
- 📂 ... virtual **file system** to unpack and pack containers efficiently.

## Usage

The project has the following .NET libraries (NuGet packages via nuget.org). The
libraries only support the latest .NET LTS version: **.NET 6.0**.

- [![Yarhl](https://img.shields.io/nuget/v/Yarhl?label=Yarhl&logo=nuget)](https://www.nuget.org/packages/Yarhl)
  - `Yarhl.FileFormat`: format conversion APIs.
  - `Yarhl.FileSystem`: virtual file system.
  - `Yarhl.IO`: streams, binary and text reading / writing.
- [![Yarhl.Media.Text](https://img.shields.io/nuget/v/Yarhl.Media.Text?label=Yarhl.Media.Text&logo=nuget)](https://www.nuget.org/packages/Yarhl.Media.Text)
  - `Yarhl.Media.Text`: translation formats and converters (Po), table replacer.
  - `Yarhl.Media.Text.Encoding`: _euc-jp_ and token-escaped encodings.
- [![Yarhl.Plugins](https://img.shields.io/nuget/v/Yarhl.Plugins?label=Yarhl.Plugins&logo=nuget)](https://www.nuget.org/packages/Yarhl.Plugins)
  - `Yarhl.Plugins`: discover formats and converters from .NET assemblies.

> [!NOTE]  
> _Are you planning to try a preview version?_ Check-out the
> [GitHub project readme](https://github.com/SceneGate/Yarhl#install) for
> details how to get setup the NuGet preview feed.

## Quick demo

You can use _Yarhl_ to create applications that converts file formats. For
instance, let's extract the text from a game into a translatable file format
like [PO](https://www.gnu.org/software/gettext/manual/html_node/PO-Files.html).
We can use the following libraries for this task:

- [Yarhl.Media.Text](./articles/media-text/po-format.md): support for PO format.
- [Ekona](https://github.com/SceneGate/Ekona/): support of NDS game file system.
- [LayTea](https://github.com/pleonex/LayTea): support for formats from
  _Professor Layton_ games.

The flow of format conversions would be:

```mermaid
flowchart TB
  subgraph S1 [1. Access game files]
    S1_A("File from disk\n(Binary format)") -->|Binary2NitroRom| S1_B
    S1_B(Container)
  end
  subgraph S2 [2. Unpack game file with the text]
    S2_A("Navigate to file\n`data/ll_common.darc`\n(Binary format)") --> |BinaryDarc2Container| S2_B
    S2_B(Container) --> S2_C
    S2_C("Navigate to file 2\n(Binary format)") -->|DencDecompression| S2_D
    S2_D("Decompressed binary")
  end
  subgraph S3 [3. Convert to PO format]
    S3_A("Decompressed binary") --> |Binary2MessageCollection| S3_B
    S3_B("Game text format") --> |MessageCollection2Po| S3_C
    S3_C("PO format")
  end
  subgraph S4 [4. Save PO format to disk]
    S4_A("PO format") --> |Po2Binary| S4_B
    S4_B("Binary format") -->|"Stream.WriteTo(output)"| S4_C
    S4_C(("Done!"))
  end

  S1 --> S2
  S2 --> S3
  S3 --> S4
```

[!code-csharp[Demo1](./../src/Yarhl.Examples/Introduction.cs?name=Demo1)]

At this point, we can also interact with any format. For instance, let's change
a translation entry.

[!code-csharp[Demo2](./../src/Yarhl.Examples/Introduction.cs?name=Demo2)]

## Showcase

Some cool projects built with _Yarhl_:

- [**Ekona**](https://scenegate.github.io/Ekona/): support Nintendo DS file
  formats.
- [**Lemon**](https://scenegate.github.io/Lemon/): support Nintendo 3DS file
  formats.
- [**LayTea**](https://www.pleonex.dev/LayTea/): modding tools for _Professor
  Layton_ games.
- [**Attack of Friday Monsters tools**](https://github.com/pleonex/AttackFridayMonsters):
  modding tools for _Attack of the Friday Monsters_ game.
- [**Metatron**](https://github.com/TraduSquare/Metatron): translation framework
  for _Shin Megami Tensei_ saga games.

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/).
