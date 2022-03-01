# _Yarhl, A format ResearcH Library_

**Yarhl** is a set of libraries to **implement and convert file formats**. It
provides a virtual file system, format conversion APIs, full feature binary IO
and plugin support to support common formats. It's built in **C# / .NET** and
works in Windows, Linux and Mac OS X.

## Usage

The provides the following .NET libraries as NuGet packages (via nuget.org). The
libraries only support the latest version of .NET and its LTS: **.NET 6.0**.

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

[!code-csharp[Demo1](../../../src/Yarhl.Examples/Introduction.csx?name=Demo1)]

At this point, we can also interact with any format. For instance, let's change
a translation entry.

[!code-csharp[Demo2](../../../src/Yarhl.Examples/Introduction.csx?name=Demo2)]
