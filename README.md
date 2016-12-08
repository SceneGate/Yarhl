# LibGame

[![Build Status](https://travis-ci.org/pleonex/libgame.svg?branch=master)](https://travis-ci.org/pleonex/libgame)
[![Build Status](https://ci.appveyor.com/api/projects/status/hjgmge090s7962q6/branch/master?svg=true)](https://ci.appveyor.com/project/pleonex/libgame/branch/converters)
[![License](https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat)](http://www.gnu.org/copyleft/gpl.html)

<br>
<p align="center"><b>To hack your games could not be easier.</b></p>

**libgame** is a library for *ROM Hacking* projects. The goal is to simplify the implementation and conversion of common file formats.


## Compilation
It has been developed and tested with *mono 4.2.4* in *Fedora 24*.

### Linux
You need to install *git* using your package manager (ie *apt-get*, *yum*, *pacman*...) and the last stable mono version from [here](http://www.mono-project.com/docs/getting-started/install/linux/).
``` shell
# Clone the repository
git clone https://github.com/pleonex/libgame --recursive

# Compile dependencies
xbuild mono-addins/Mono.Addins/Mono.Addins.csproj
```

Now, you can either open the solution with *MonoDevelop* or compile from the terminal:
``` shell
# Restore NuGet packages
wget http://nuget.org/nuget.exe
mono nuget.exe libgame.sln

# Compile
xbuild libgame.sln

# [Optional] Run test
# Install nunit-console from your package manager
nunit-console libgame.UnitTests/bin/Debug/libgame.UnitTests.dll
```

### Windows
1. Clone the repository with the [GitHub client](https://windows.github.com/) or download the [zip](https://github.com/pleonex/libgame/archive/converters.zip).
2. Download and install *Xamarin Studio* from [here](http://www.monodevelop.com/download/) and open the solution. It should work with *Visual Studio* and [*SharpDevelop*](http://www.icsharpcode.net/OpenSource/SD/Download/) too.
3. Compile the Mono.Addins dependencies project at: *mono-addins/Mono.Addins/Mono.Addins.csproj*
4. Compile libgame!
