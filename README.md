# Yarhl - Yet Another ROM Hacking Library

[![Build Status](https://travis-ci.org/SceneGate/yarhl.svg?branch=master)](https://travis-ci.org/SceneGate/Yarhl)
[![Build status](https://ci.appveyor.com/api/projects/status/hjgmge090s7962q6/branch/master?svg=true)](https://ci.appveyor.com/project/pleonex/libgame/branch/master)
[![License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

**Yarhl** is a library for *ROM Hacking* and translation projects. It provides a virtual file system, file format and format conversion facilities and plugin support. It's built in C# and works in all the operating systems.


## Compilation
### Linux
1. Download the [repository](https://github.com/SceneGate/yarhl/archive/master.zip) or clone it with git: `git clone https://github.com/SceneGate/Yarhl --recursive`
2. Install [Mono](http://www.mono-project.com/docs/getting-started/install/linux/).
3. Resolve any dependency with [NuGet](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe): `mono nuget.exe restore`
4. Compile the Mono.Addins dependencies project at: *mono-addins/Mono.Addins/Mono.Addins.csproj*
5. Compile with *MonoDevelop* or *msbuild*.

### Windows
1. Clone the repository with the [GitHub client](https://windows.github.com/) or download the [zip](https://github.com/SceneGate/yarhl/archive/master.zip).
2. Download and install *Xamarin Studio* from [here](http://www.monodevelop.com/download/) and open the solution. It should work with *Visual Studio* and [*SharpDevelop*](http://www.icsharpcode.net/OpenSource/SD/Download/) too.
3. Compile the Mono.Addins dependencies project at: *mono-addins/Mono.Addins/Mono.Addins.csproj*
4. Compile yarhl!
