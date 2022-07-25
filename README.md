# Yarhl: Yet Another ROM Hacking Library [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/)

![Yarhl Logo](https://raw.githubusercontent.com/SceneGate/Yarhl/develop/docs/images/logo.png)

**Yarhl** is a library for _ROM Hacking_ and fan-translation projects. It
provides a virtual file system, file format and format conversion features and
plugin support. It's built in C# / .NET and works in Windows, Linux and Mac OS
X.

<!-- prettier-ignore -->
| NuGet              | [![Yarhl](https://img.shields.io/nuget/v/Yarhl?label=Yarhl)](https://www.nuget.org/packages/Yarhl) [![Yarhl.Media.Text](https://img.shields.io/nuget/v/Yarhl.Media.Text?label=Yarhl.Media.Text)](https://www.nuget.org/packages/Yarhl.Media.Text) |
| ------------------ | ------ |
| **Build & Test**   | ![Build and release](https://github.com/SceneGate/Yarhl/workflows/Build%20and%20release/badge.svg?branch=develop) |
| **Quality report** | [![CII Best Practices](https://bestpractices.coreinfrastructure.org/projects/2919/badge)](https://bestpractices.coreinfrastructure.org/projects/2919) |

## Documentation

Feel free to ask any question in the
[project Discussion site!](https://github.com/SceneGate/Yarhl/discussions).

Check our on-line API overview:
[Yarhl in a nutshell](https://scenegate.github.io/Yarhl/guides/Yarhl-nutshell.html)
and the complete API documentation
[here](https://scenegate.github.io/Yarhl/api/Yarhl.html).

## Install

Stable releases are available from nuget.org:

- [Yarhl](https://www.nuget.org/packages/Yarhl)
- [Yarhl.Media.Text](https://www.nuget.org/packages/Yarhl.Media.Text)

The libraries only support the latest version of .NET and its LTS (.NET 6).

Preview releases can be found in this
[Azure DevOps package repository](https://dev.azure.com/SceneGate/SceneGate/_packaging?_a=feed&feed=SceneGate-Preview).
To use a preview release, create a file `nuget.config` in the same directory of
your solution (.sln) file with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="SceneGate-Preview" value="https://pkgs.dev.azure.com/SceneGate/SceneGate/_packaging/SceneGate-Preview/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

## Build

The project requires to build .NET 6.0 SDK and .NET Framework 4.8 or latest
Mono. If you open the project with VS Code and you did install the
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
