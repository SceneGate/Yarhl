# Yarhl: Yet Another ROM Hacking Library [![GPL v3 License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

![Yarhl Logo](https://raw.githubusercontent.com/SceneGate/Yarhl/master/docs/images/logo.png)

**Yarhl** is a library for *ROM Hacking* and fan-translation projects.
It provides a virtual file system, file format and format conversion features
and plugin support. It's built in C# / .NET and works in Windows, Linux and
Mac OS X.

| NuGet | [![Nuget](https://img.shields.io/nuget/v/Yarhl.svg)](https://www.nuget.org/packages/Yarhl) [![Nuget prerelease](https://img.shields.io/nuget/vpre/Yarhl.svg)](https://www.nuget.org/packages/Yarhl) |
| ----- | ------ |
| **Build & Test** | [![Build Status](https://dev.azure.com/SceneGate/Yarhl/_apis/build/status/SceneGate.Yarhl?branchName=master)](https://dev.azure.com/SceneGate/Yarhl/_build/latest?definitionId=1&branchName=master) |
| **Quality report** | [![Sonar Gate](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=alert_status)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) [![Total alerts](https://img.shields.io/lgtm/alerts/g/SceneGate/Yarhl.svg?logo=lgtm&logoWidth=18)](https://lgtm.com/projects/g/SceneGate/Yarhl/alerts/) |
| **Coverage** | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=coverage)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) |
| **Project Best Practices** | [![CII Best Practices](https://bestpractices.coreinfrastructure.org/projects/2919/badge)](https://bestpractices.coreinfrastructure.org/projects/2919) |
| **Quality Details** | [![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=bugs)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) [![Code smells](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=code_smells)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) [![Duplicated lines](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) |
| **Code Stats** | [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=SceneGate_Yarhl&metric=ncloc)](https://sonarcloud.io/dashboard?id=SceneGate_Yarhl) |

## Documentation

Check our on-line API overview: [Mastering Yarhl](https://scenegate.github.io/Yarhl/articles/Mastering-Yarhl.html)
and the API documentation [here](https://scenegate.github.io/Yarhl/).

## Install

Yarhl is available on NuGet:

* [Yarhl](https://www.nuget.org/packages/Yarhl)
* [Yarhl.Media](https://www.nuget.org/packages/Yarhl.Media)

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
