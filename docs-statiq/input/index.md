---
Title: Format research library
---

<!-- markdownlint-disable MD033 -->
<img alt="Yarhl logo" src="./assets/logo-large.png" style="width: 100%;" />

<p align="center">
  <a href="https://github.com/SceneGate">
    <img
      alt="SceneGate awesome"
      src="https://img.shields.io/badge/SceneGate-awesome%20%F0%9F%95%B6-blue?logo=csharp"
    />
  </a>
  &nbsp;
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
  - `Yarhl`: plugin manager to find formats and converters.
  - `Yarhl.FileFormat`: format conversion APIs.
  - `Yarhl.FileSystem`: virtual file system.
  - `Yarhl.IO`: streams, binary and text reading / writing.
- [![Yarhl.Media](https://img.shields.io/nuget/v/Yarhl.Media?label=Yarhl.Media&logo=nuget)](https://www.nuget.org/packages/Yarhl.Media)
  - `Yarhl.Media.Text`: translation formats and converters (Po), table replacer.
  - `Yarhl.Media.Text.Encoding`: _euc-jp_ and token-escaped encodings.

## Quick demo

You can use _Yarhl_ to create applications to convert and work with file formats
already supported by its plugins. For instance, let's extract the text from a
_NDS_ game using two _Yarhl_ libraries:

- [Ekona](https://github.com/SceneGate/Ekona/): support of NDS game file system.
- [LayTea](https://github.com/pleonex/LayTea): support for formats from
  _Professor Layton_ games.

<!-- [!code-csharp[Demo1](../../../src/Yarhl.Examples/Introduction.cs?name=Demo1)] -->

At this point, we can also interact with any format. For instance, let's change
a translation entry.

<!-- [!code-csharp[Demo2](../../../src/Yarhl.Examples/Introduction.cs?name=Demo2)] -->

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