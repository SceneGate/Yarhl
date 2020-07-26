# Yarhl: Yet Another ROM Hacking Library [![MIT License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat)](https://choosealicense.com/licenses/mit/)

![Yarhl Logo](https://raw.githubusercontent.com/SceneGate/Yarhl/develop/docs/images/logo.png)

**Yarhl** is a library for _ROM Hacking_ and fan-translation projects. It
provides a virtual file system, file format and format conversion features and
plugin support. It's built in C# / .NET and works in Windows, Linux and Mac OS
X.

<!-- prettier-ignore -->
| NuGet | [![Nuget](https://img.shields.io/nuget/v/Yarhl.svg)](https://www.nuget.org/packages/Yarhl) |
| ----- | ------ |
| **Build & Test** | [![Build Status](https://dev.azure.com/SceneGate/Yarhl/_apis/build/status/SceneGate.Yarhl?branchName=develop)](https://dev.azure.com/SceneGate/Yarhl/_build?definitionId=1&_a=summary&repositoryFilter=1&branchFilter=38) ![Azure DevOps tests](https://img.shields.io/azure-devops/tests/SceneGate/Yarhl/1?compact_message) ![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/SceneGate/Yarhl/1) |
| **Quality report** | [![CII Best Practices](https://bestpractices.coreinfrastructure.org/projects/2919/badge)](https://bestpractices.coreinfrastructure.org/projects/2919) [![Total alerts](https://img.shields.io/lgtm/alerts/g/SceneGate/Yarhl.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/SceneGate/Yarhl/alerts/) |

## Documentation

Check our on-line API overview:
[Yarhl in a nutshell](https://scenegate.github.io/Yarhl/articles/Yarhl-nutshell.html)
and the API documentation
[here](https://scenegate.github.io/Yarhl/api/Yarhl.html).

## Install

Stable releases are available from nuget.org:

- [Yarhl](https://www.nuget.org/packages/Yarhl)
- [Yarhl.Media](https://www.nuget.org/packages/Yarhl.Media)

Alpha releases can be found in this
[Azure DevOps package repository](https://dev.azure.com/SceneGate/Yarhl/_packaging).
To use an alpha release, create a file `nuget.config` in the same directory of
your solution (.sln) file with the following content:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Yarhl-Alpha" value="https://pkgs.dev.azure.com/SceneGate/Yarhl/_packaging/preview%40Local/nuget/v3/index.json" />
  </packageSources>
</configuration>
```

## Build

Since the unit tests are validated against .NET Core and .NET Framework or Mono
both runtime must be installed on the machine. Then run:

```sh
dotnet tool restore
dotnet cake
```
