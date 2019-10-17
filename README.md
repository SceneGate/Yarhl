# Yarhl: Yet Another ROM Hacking Library [![GPL v3 License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

![Yarhl Logo](https://raw.githubusercontent.com/SceneGate/Yarhl/master/docs/images/logo.png)

**Yarhl** is a library for *ROM Hacking* and fan-translation projects.
It provides a virtual file system, file format and format conversion features
and plugin support. It's built in C# / .NET and works in Windows, Linux and
Mac OS X.

| NuGet | [![Nuget](https://img.shields.io/nuget/v/Yarhl.svg)](https://www.nuget.org/packages/Yarhl) [![NuGet Alpha](https://img.shields.io/github/v/tag/SceneGate/Yarhl?color=yellow&include_prereleases&label=nuget)](https://github.com/SceneGate/Yarhl/packages) |
| ----- | ------ |
| **Build & Test** | [![Build Status](https://dev.azure.com/SceneGate/Yarhl/_apis/build/status/SceneGate.Yarhl?branchName=master)](https://dev.azure.com/SceneGate/Yarhl/_build/latest?definitionId=1&branchName=master) ![Azure DevOps tests](https://img.shields.io/azure-devops/tests/SceneGate/Yarhl/1?compact_message) ![Azure DevOps coverage](https://img.shields.io/azure-devops/coverage/SceneGate/Yarhl/1) |
| **Quality report** | [![CII Best Practices](https://bestpractices.coreinfrastructure.org/projects/2919/badge)](https://bestpractices.coreinfrastructure.org/projects/2919) [![Total alerts](https://img.shields.io/lgtm/alerts/g/SceneGate/Yarhl.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/SceneGate/Yarhl/alerts/) |

## Documentation

Check our on-line API overview: [Mastering Yarhl](https://scenegate.github.io/Yarhl/articles/Mastering-Yarhl.html)
and the API documentation [here](https://scenegate.github.io/Yarhl/api/Yarhl.html).

## Install

Stable releases are available from nuget.org:

* [Yarhl](https://www.nuget.org/packages/Yarhl)
* [Yarhl.Media](https://www.nuget.org/packages/Yarhl.Media)

Alpha releases can be found in this
[GitHub package repository](https://github.com/SceneGate/Yarhl/packages).

## Build

Since the unit tests are valited against .NET Core and .NET Framework or Mono
both runtime must be installed on the machine.

### Linux & Mac OS X

Requirements:
[*Mono*](http://www.mono-project.com/docs/getting-started/install/linux/) and
[*.NET Core*](https://dotnet.microsoft.com/download).

1. Clone the repository.

2. Run `./build.sh`

### Windows

Requirements: *.NET Framework* and
[*.NET Core*](https://dotnet.microsoft.com/download).

1. Clone the repository. You can use the
   [GitHub client](https://windows.github.com/)
   or the [command-line](https://git-scm.com/downloads).

2. Run `.\build.ps1`

You can also validate a Linux build using Docker with:
`docker build .`
