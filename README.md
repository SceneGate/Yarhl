# Yarhl, A format ResearcH Library [![awesomeness](https://img.shields.io/badge/SceneGate-awesome%20%F0%9F%95%B6-blue?logo=csharp)](https://github.com/SceneGate)

![Yarhl Logo](./docs/images/logo-large.png)

<!-- markdownlint-disable MD033 -->
<p align="center">
  <a href="https://www.nuget.org/packages?q=Yarhl">
    <img alt="Stable version" src="https://img.shields.io/nuget/v/Yarhl?label=Stable" />
  </a>
  &nbsp;
  <a href="https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview">
    <img alt="GitHub commits since latest release (by SemVer)" src="https://img.shields.io/github/commits-since/SceneGate/Yarhl/latest?sort=semver" />
  </a>
  &nbsp;
  <a href="https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release">
    <img alt="Build and release" src="https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release/badge.svg?branch=develop" />
  </a>
  &nbsp;
  <a href="https://bestpractices.coreinfrastructure.org/projects/2919">
    <img alt="CII Best Practices" src="https://bestpractices.coreinfrastructure.org/projects/2919/badge" />
  </a>
  &nbsp;
  <a href="https://choosealicense.com/licenses/mit/">
    <img alt="MIT License" src="https://img.shields.io/badge/license-MIT-blue.svg?style=flat" />
  </a>
  &nbsp;
</p>

**Yarhl** is a set of libraries that helps to **implement and convert file
formats**. It provides a virtual file system, format conversion APIs, full
featured binary IO and plugin support to support common formats. It's built in
**C# / .NET** and works in any OS that supports the .NET runtime.

- :books: **Format implementation** architecture and guidelines
- :recycle: **Format conversion** API
- :open_file_folder: **Virtual file system** with format transformations
- :1234: **Enhanced binary IO API**
  - Custom `Stream` with **sub-stream supports** (memory and disk efficient!)
  - Full feature binary and text readers and writers
  - Simple binary (de)serializer by attributes in the model.
- :page_with_curl: Standard text formats
  - Industry-standard localization format: **GNU gettext PO**
  - Table text replacements
  - **Common encodings**: euc-jp, token-escaped encoding
  - **API for simple encoding implementations**
- 🔌**Plugin** API to load and find types in .NET assemblies.

## Get started

Check out the [documentation site](https://scenegate.github.io/Yarhl/index.html)
to start learning the power of _Yarhl_.

Feel free to ask any question in the
[project discussions](https://github.com/SceneGate/Yarhl/discussions).

## Usage

This project provides the following libraries as NuGet packages (via nuget.org).
The libraries support the latest version of .NET and its LTS.

- [![Yarhl](https://img.shields.io/nuget/v/Yarhl?label=Yarhl&logo=nuget)](https://www.nuget.org/packages/Yarhl):
  core, format conversion, file system and binary reading / writing (IO).
- [![Yarhl.Media.Text](https://img.shields.io/nuget/v/Yarhl.Media.Text?label=Yarhl.Media.Text&logo=nuget)](https://www.nuget.org/packages/Yarhl.Media.Text):
  text formats (Po) and encodings.
- [![Yarhl.Plugins](https://img.shields.io/nuget/v/Yarhl.Plugins?label=Yarhl.Plugins&logo=nuget)](https://www.nuget.org/packages/Yarhl.Plugins):
  discover formats and converters from .NET assemblies.

**Preview releases** can be found in this
[Azure DevOps package repository](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview).
To use a preview release, create a file `nuget.config` in the same directory of
your solution file (.sln) with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear/>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="SceneGate-Preview" value="https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="nuget.org">
      <package pattern="*" />
    </packageSource>
    <packageSource key="SceneGate-Preview">
      <package pattern="Yarhl*" />
    </packageSource>
  </packageSourceMapping>
</configuration>
```

Then restore / install as usual via Visual Studio, Rider or command-line. You
may need to restart Visual Studio for the changes to apply.

## Showcase

Some cool projects built with _Yarhl_:

- [**Texim**](https://github.com/SceneGate/Texim): experimental API for image
  file formats.
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

## Contributing

The repository requires to build .NET 8.0 SDK.

To build, test and generate artifacts run:

```sh
# Build and run tests
dotnet run --project build/orchestrator

# (Optional) Create bundles (nuget, zips, docs)
dotnet run --project build/orchestrator -- --target=Bundle
```

Additionally you can use _Visual Studio_ or _JetBrains Rider_ as any other .NET
project.

To contribute follow the [contributing guidelines](CONTRIBUTING.md).

### How to release

Create a new _GitHub release_ with a tag name `v{Version}` (e.g. `v2.4`) and
that's it! This triggers a pipeline that builds and deploy the project.

## License

The software is licensed under the terms of the
[MIT license](https://choosealicense.com/licenses/mit/).
