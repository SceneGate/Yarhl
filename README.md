# Yarhl: Yet Another ROM Hacking Library [![GPL v3 License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

**Yarhl** is a library for *ROM Hacking* and fan-translation projects.
It provides a virtual file system, file format and format conversion features
and plugin support. It's built in C# / .NET and works in Windows, Linux and
Mac OS X.

| | Linux & Mac OS X | Windows |
| ----- | ----- | ----- |
| **Build & Test** | [![Travis Build Status](https://travis-ci.org/SceneGate/Yarhl.svg?branch=master)](https://travis-ci.org/SceneGate/Yarhl) [![Travis Build history](https://buildstats.info/travisci/chart/SceneGate/Yarhl)](https://travis-ci.org/SceneGate/Yarhl/builds) | [![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/hjgmge090s7962q6/branch/master?svg=true)](https://ci.appveyor.com/project/pleonex/libgame/branch/master) [![AppVeyor Build history](https://buildstats.info/appveyor/chart/pleonex/libgame)](https://ci.appveyor.com/project/pleonex/libgame/history) |

| Quality report | [![Sonar Gate](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=alert_status)](https://sonarcloud.io/dashboard?id=yarhl) |
| ----- | ------ |
| **Coverage** | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=coverage)](https://sonarcloud.io/dashboard?id=yarhl) |
| **Quality Details** | [![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=yarhl) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=bugs)](https://sonarcloud.io/dashboard?id=yarhl) [![Code smells](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=code_smells)](https://sonarcloud.io/dashboard?id=yarhl) [![Duplicated lines](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=yarhl) |
| **Code Stats** | [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=ncloc)](https://sonarcloud.io/dashboard?id=yarhl) |

## Documentation

Online API documentation is available in the GitHub page
[here](https://scenegate.github.io/Yarhl/).

## Build

### Linux & Mac OS X

Requirements:
[*Mono*](http://www.mono-project.com/docs/getting-started/install/linux/).

1. Clone the repository.

2. Run `./build.sh`

### Windows

Requirements:
*Visual Studio* or
[*Xamarin Studio*](http://www.monodevelop.com/download/).

1. Clone the repository. You can use the
   [GitHub client](https://windows.github.com/)
   or the [command-line](https://git-scm.com/downloads).

2. Run `.\build.ps1`
