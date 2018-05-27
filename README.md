# Yarhl - Yet Another ROM Hacking Library

**Yarhl** is a library for *ROM Hacking* and translation projects. It provides a virtual file system, file format and format conversion facilities and plugin support. It's built in C# / .NET and works in Windows, Linux and Mac OS.

| Build & Test | [![Travis Build Status](https://travis-ci.org/SceneGate/Yarhl.svg?branch=master)](https://travis-ci.org/SceneGate/Yarhl) [![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/hjgmge090s7962q6/branch/master?svg=true)](https://ci.appveyor.com/project/pleonex/libgame/branch/master) |
| ----- | ------ |
| **Coverage** | [![Coveralls](https://coveralls.io/repos/github/SceneGate/Yarhl/badge.svg?branch=master)](https://coveralls.io/github/SceneGate/Yarhl?branch=master) [![Sonar Coverage](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=coverage)](https://sonarcloud.io/dashboard?id=yarhl) |
| **Quality Reports** | [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=alert_status)](https://sonarcloud.io/dashboard?id=yarhl) |
| **Quality Details** | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=bugs)](https://sonarcloud.io/dashboard?id=yarhl) [![Code smells](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=code_smells)](https://sonarcloud.io/dashboard?id=yarhl) [![Duplicated lines](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=yarhl) [![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=yarhl) |
| **Code Stats** | [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=yarhl&metric=ncloc)](https://sonarcloud.io/dashboard?id=yarhl) |
| **License** | [![License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html) |

## Compilation

### Linux

1.  Download the [repository](https://github.com/SceneGate/Yarhl/archive/master.zip) or clone it with git: `git clone https://github.com/SceneGate/Yarhl --recursive`
2.  Install [Mono](http://www.mono-project.com/docs/getting-started/install/linux/).
3.  Run `automation/build.sh`

### Windows

1.  Clone the repository with the [GitHub client](https://windows.github.com/) or download the [zip](https://github.com/SceneGate/Yarhl/archive/master.zip).
2.  Download and install *Xamarin Studio* from [here](http://www.monodevelop.com/download/) and open the solution. It should work with *Visual Studio* and [*SharpDevelop*](http://www.icsharpcode.net/OpenSource/SD/Download/) too.
3.  Compile Mono.Addins dependency project: *mono-addins/Mono.Addins/Mono.Addins.csproj*
4.  Compile Yarhl
