# Mastering Yarhl

![Yarhl Logo](../images/logo.png)

**Yarhl** - *Yet Another ROM Hacking Library* - is a library for _ROM Hacking_ and fan-translation projects. It provides a virtual file system, file format, and format conversion features and plugin support.

But what it really has to offer? Why should you use it? And how? This tutorial will teach you how to use Yarhl and how to take advantage of the 100% of it.

Remember that if you have any question you can [use our gitter chat](https://gitter.im/SceneGate/Yarhl), but first make sure you've read the whole docs.

## Your first steps: Reading and Writing

Oh, hi! I'm Master Yarhl (or M.Y.). Nice to meet you. I will be your guide! Erm... y-you can... picture me like this:

![Master Yarhl](../images/mister.png)

Let's get started! The first module I'm teaching you is [`Yarhl.IO`](xref:Yarhl.IO) (IO stands for _Input/Output_), which is similar to .NET standard `System.IO` but with specific features to work with binary files.

This module is divided into binary and text files. Easy peasy!
Let's go deeper into these classes!

### DataStream

[`DataStream`](xref:Yarhl.IO.DataStream) wraps any kind of .NET `Stream`.

#### Reuse of Stream

It allows to reuse a parent Stream to have substreams to reduce the number of resources to use. For instance, to unpack a file you would just need to create `DataStream` instances from the same parent `DataStream` having different offsets and lengths.

Disposing the last instance of a `DataStream` that has a reference to a `Stream` will dispose its parent `Stream` too.

#### Comparison

The `DataStream` class provides the [`Compare`](xref:Yarhl.IO.DataStream.Compare(Yarhl.IO.DataStream)) method to check if two streams are identical.

#### Push and pop positions

Similar to the terminal commands `pushd` and `popd`, our `DataStream` provides methos to temporarily moving into a position to perform an operation and then restore the position. This is very useful when you need to read or write a few fields into another section of the file. It works with an stack so you can push several positions.

- [`PushCurrentPosition`](xref:Yarhl.IO.DataStream.PushCurrentPosition): save the current position.
- [`PushToPosition`](xref:Yarhl.IO.DataStream.PushToPosition*): save the current position and move.
- [`PopPosition`](xref:Yarhl.IO.DataStream.PopPosition): restore the last saved position.
- [`RunInPosition`](xref:Yarhl.IO.DataStream.RunInPosition(System.Action,System.Int64,Yarhl.IO.SeekMode)): push, run the lambda expression and pop again.

#### Read and Write

We have also the typical read and write methods for arrays of bytes. And don't forget about the [`WriteTo`](xref:Yarhl.IO.DataStream.WriteTo(System.String)) methods that allows to write a full `DataStream` into another `DataStream` or into a file in your HD. Very useful mate!

### DataReader and DataWriter

[`DataReader`](xref:Yarhl.IO.DataReader) is the equivalent of the .NET `BinaryReader` and [`DataWriter`](xref:Yarhl.IO.DataWriter) of `BinaryWriter. Apart from the typical read and write methods, they provide the following very useful features.

#### Endianness

By properties or constructor you can specify if the endianness of the stream if little or endian. This will affect to all the read and write operations.

#### Strings

By using the different overloads of `ReadString` and `Write` you can read and write strings with different encodings, fixed sizes, null terminated or not or in the class _size + content_ style. I recommend you to take a look into them, they cover all the cases you will need to work with files.

#### Padding

Are you tired of writing logic to skip or write padding bytes? Well, we too!
If you are reading a file and you want to skip padding bytes, you can call [`ReadPadding`](xref:Yarhl.IO.DataReader.ReadPadding(System.Int32,System.Boolean)) and if you need to write padding bytes, then [`WritePadding`](xref:Yarhl.IO.DataWriter.WritePadding(System.Byte,System.Int32,System.Boolean)) will be your friend.

### TextReader and TextWriter

So far, `DataReader` and `DataWriter` have been very useful when you are dealing with a file that contains some integer fields for size or offset, arrays of bytes and maybe null-terminated strings. But, what about if you need to work with a file that only contains text and you are interested in reading line by line? In that case, you need [`TextReader`](xref:Yarhl.IO.TextReader) and [`TextWriter`](xref:Yarhl.IO.TextWriter).

#### New lines

By default, `TextWriter` uses always (Windows too) the new line `\n`. It doesn't use `\r\n`. The reason is that most file formats uses `\n` and in some games having the `\r` may crash. It's sometimes difficult to notice that. If you want to use any other new line string (you can even use `<br/>`), you just need to change the [`NewLine`](xref:Yarhl.IO.TextWriter.NewLine) property.

In the case of the `TextReader` the behavior is different. The default value for the [`NewLine`](xref:Yarhl.IO.TextReader.NewLine) property depends on the OS (Windows: `\r\n`, Unix: `\n`). In addition, we provided with an automatic mehcanism enabled by default: [`AutoNewLine`](xref:Yarhl.IO.TextReader.AutoNewLine*). If it's enabled, you don't need to know the line ending in advance because we will stop at `\n` and remove the last `\r` if present. This is also useful if a file mix both line endings. And remember, by setting the `NewLine` property `AutoNewLine` is disabled.

#### Encoding

The encoding can only by specified in the constructor. We believe that it doesn't have sense to change the encoding once you start using the reader because a text file must not mix encodings.

#### Peeking

Do you need to read a line without actually moving the position of the stream. Maybe you want to check if the line contains a token but you are not sure and don't want to keep the current position all the time. Well, in that case you have the `Peek*` methods.

#### Preambles / BOM

Some encodings may have a specific [BOM](https://en.wikipedia.org/wiki/Byte_order_mark) (_Byte Order Mark_) (or _preamble_ in the .NET world). These are some bytes at the beginning of the stream that confirms the encoding of the file. For instance, when using UTF-16, the file will begin with the bytes `0xFEFF`. It also specifies if the encoding is little-ending or big-endian (needed for UTF-16).

Our `TextReader` will skip the BOM (_if it's present_) at the beginning of the file. In the case of the `TextWriter`, the behavior is defined by the property [`AutoPreamble`](xref:Yarhl.IO.TextWriter.AutoPreamble) which is set to `false` by default (again, some games may see it as unexpected bytes). When enabled, the first write call will also write the BOM. You can also write it manually by calling [`WritePreamble()`](xref:Yarhl.IO.TextWriter.WritePreamble) (but remember, only if you are at the beginning of the stream).

### Examples

#### Reading / writing a binary file

```csharp
public void LoadFile(string path)
{
    using (var stream = new DataStream(path, FileOpenMode.Read)) {
        var reader = new DataReader(stream) {
            DefaultEncoding = new EscapeOutRangeEnconding("ascii"),
            Endianness = EndiannessMode.BigEndian,
        };

        string id = reader.ReadString(4);
        int offset = reader.ReadInt32();
        reader.ReadPadding(32);
        double myDouble = reader.ReadDouble();

        string name;
        stream.RunInPosition(
            () => name = reader.ReadString(),
            offset);
    }
}

public void SaveFile(string path)
{
    using (var stream = new DataStream(path, FileOpenMode.Read)) {
        var writer = new DataWriter(stream);

        writer.Write("TEX0", false);
        writer.Write(0xCAFE);
        writer.Write(0x00);
        writer.WritePadding(0xFF, 32);
        writer.Write("My long text of 80 bytes", 80);

        stream.PushToPosition(0x08);
        writer.Write(0x65402);
        stream.PopPosition();
    }
}
```

#### Reading / writing a text file

```csharp
public void LoadFile(DataStream stream)
{
    var reader = new TextReader(stream, Encoding.Unicode);

    string firstLine = reader.ReadLine();
    char[] someChars = reader.Read(4);
    string beforeToken = reader.ReadToToken("#");

    if (reader.Peek() == ':')
        reader.ReadLine();
    string restFile = reader.ReadToEnd();
}

public void SaveFile(DataStream stream)
{
    var writer = new TextWriter(stream) {
        AutoPreamble = true,
    };

    writer.WriteLine("Hello world!");
    writer.WriteLine("Count is {0}", 42);
    writer.Write("No new line");
}
```

## Small introduction to Format

Every game is composed by files, which has a specific format, for example .NCLR is a Palette file, or .aar is a package file. Yarhl helps you to code objects as you were actually coding a game file.

Continuing with the Palette example, you'd create a new class named NCLR with everything you need to store. Or if you have different Palette types with common content, you could create a Palette format and then the children.

Let's go for a quick example!

![Hex view of example file](../images/hex_example.png)

This file follows the following specification:

Size | Name
---- | -----
4    | Magic ID
2    |Number of sentences
2    | Size of the file
\*   | Sentences

So we can create the class Example like this:

```csharp
public class Example : Format
{
    public uint MagicID { get; set; }
    public ushort NumberSentences { get; set; }
    public ushort FileSize { get; set; }
    public IList<String> Sentences { get; private set; }

    public Example()
    {
        Sentences = new List<String>();
    }
}
```

Easy! But for now I can't teach you how to transform this example binary file we'just saw into this clean Object, I need to explain other things first.

### BinaryFormat

This is the only Format which Yarhl has in its core, why? Because as the name says, it is the most basic Format you'll ever need: a Binary file.

Remember what we did at the first chapter? Opening up a file in order to read... What if I told you, we did it wrong?
Well, actually it's not wrong, but it's not complete! What? What's that nonsense? Okay, okay, phew! Let's go.

As you can see [in the documentation](https://scenegate.github.io/Yarhl/api/Yarhl.FileFormat.BinaryFormat.html) BinaryFormat is just a wrapped DataStream with all the functions of a Format (which we'll see soon), and for now that's the only thing you need to know.

I know... I talk too much... Let's continue!

## Entering the virtual world: Nodes

This is the main feature of Yarhl and the most important one, no doubt, 10/10 Yarhl users would say so<sup>1</sup>. Yarhl has a virtual file system to handle your files while mantaining your computer intact, you can now delete your "tests" folder and clean your desktop after-ages.

As always let's take a look [at the docs](https://scenegate.github.io/Yarhl/api/Yarhl.FileSystem.Node.html#properties) a Node is a virtual file with three properties: Format, IsContainer and Stream.

Also it inhertis from NavigableNode, which has interesting properties as Name, Path (of the virtual file system, not your computers'), Parent and Children, whose behaviour you'll get later at 2.1.

As we said before, every file in a game has a format, that's why our virtual files (Nodes) must have a Format. IsContainer tells you if it has children (let's ignore this for now) and a DataStream (just if the format is BinaryFormat, null otherwise. It is just a shortcut to access the Stream property of the BinaryFormat).

Let's clarify it out with an example:

```csharp
new Node("NodeName", new BinaryFormat(filePath));
```

Heh... nothing special right? What about this?

```csharp
NodeFactory.FromFile(filePath);
```

Yeeeah! That's the face I was looking for! You can create a virtual file that quick!

And there's more! What about creating multiple nodes from multiple files?

<sup>1</sup> <small>None of Yarhl users wants to talk with me anymore, so maybe this is not 100% accurate.</small>

### NodeContainerFormat

Remember that IsContainer property? Well, this is its implementation:

```csharp
public bool IsContainer {
    get { return Format is NodeContainerFormat; }
}
```

*NodeContainerFormat* starts a tree of Nodes starting from its Root (the only property it has). With this you create a virtual directory, with its virtual files.

It's more clear with a picture:

![A directory named mastering with tho files inside](../images/node_example.png)

"mastering" would be our root Node, while example.example and example2.example would be our tho child Nodes.

How can we create that? Do I have more magic for you?

```csharp
NodeFactory.FromDirectory(dirPath);
```

N is the "mastering" virtual folder! That easy!

Yarhl is way more interesting now, right!? Well, then the next chapter will blow your mind!

## Converting Formats, Transforming Nodes

Finally! Now everything begin to fall into place, you'll see.

Coding a tool is just an automation tool to transform files to different formats right? Converting .dat to .txt (text), .Ncgr to .png (images), .pak to multiple files (unpacking)...

Well, that's how Yarhl works, Yarhl is all about converting formats, let's see an example:

```csharp
new BinaryFormat(filePath)
    .ConvertWith<Font2Binary, BinaryFormat, Font>()
    .ConvertWith<Font2Image, Font, System.Drawing.Image>()
    .Save(outputPath);
```

- We start creating a new BinaryFormat with a file path (creating a virtual file copying a physical one).
- We convert the virtual File to a Font object.
- We convert the Font object to an Image object.
- And we save it to a physical file.

"But what's Font2Binary and Font2Image, Master Yarhl?" you would say.
They are converters! A converter is a class which implements the [IConverter interface](https://scenegate.github.io/Yarhl/api/Yarhl.FileFormat.IConverter-2.html).

Here you can see the full [Font2Binary](https://github.com/pleonex/PokemonConquest/blob/master/AmbitionConquest/AmbitionConquest/Fonts/Font2Binary.cs) and [Font2Image](https://github.com/pleonex/PokemonConquest/blob/master/AmbitionConquest/AmbitionConquest/Fonts/Font2Image.cs) coded by **pleonex** (my father), but I am going to show you a most simpler converter.

Remember that example from lesson 2? We are coding the converter right now!

So, here is the hex and the Format class:

![Hex view of example file](../images/hex_example.png)

```csharp
public class Example : Format
{
    public uint MagicID { get; set; }
    public ushort NumberSentences { get; set; }
    public ushort FileSize { get; set; }
    public IList<String> Sentences { get; private set; }

    public Example()
    {
        Sentences = new List<String>;
    }
}
```

```csharp
public class Binary2Example : IConverter<BinaryFormat, Example>
{
    public Example Convert(BinaryFormat file)
    {
        var example = new Example();
        var reader = new DataReader(file.Stream);

        example.MagicID = reader.ReadUInt32();
        example.NumberSentences = reader.ReadUInt16();
        example.FileSize = reader.ReadUInt16();

        for (int i = 0; i < example.NumberSentences; i++) {
            example.Sentences.Add(reader.ReadString());
        }

        return example;
    }
}
```

So now if we do:

```csharp
new BinaryFormat(filePath)
    .ConvertWith<Binary2Example, BinaryFormat, Example>();
```

We would have an Example object with all the data we need.

Also, we can code the Converter for Example into BinaryFormat:

```csharp
public class Example2Binary :
    IConverter<Example, BinaryFormat>
{
    public BinaryFormat Convert(Example example)
    {
        var binary = new BinaryFormat();
        var writer = new DataWriter(binary.Stream);

        writer.Write(example.MagicID);
        writer.Write(example.Sentences.Count);
        writer.Write(0x00); // Placeholder size to override later

        foreach (string sentence in example.Sentences) {
            writer.Write(sentence);
        }

        binary.Stream.Position = 0x06;
        writer.Write((ushort)binary.Stream.Length);

        return binary;
    }
}
```

And that's it! I'm pretty sure you've got enough of converters, but, let's see how it's done with Nodes:

```csharp
var node = NodeFactory.FromFile(path);
node.Transform<Example>;
```

Now we have a Node with the Format we want, and it is quite easy to save into a physical file in our computer.
