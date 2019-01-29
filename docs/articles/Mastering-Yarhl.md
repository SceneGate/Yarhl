# Mastering YARHL

![Yarhl Logo](https://i.imgur.com/sOzbhu4.png)

**Yarhl** - *Yet Another ROM Hacking Library* - is a library for _ROM Hacking_ and fan-translation projects. It provides a virtual file system, file format, and format conversion features and plugin support.

But what it really has to offer? Why should you use it? And how? This tutorial series will teach you how to use Yarhl and how to take advantage of the 100% of it.

Remember that if you have any question you can [use our gitter chat](https://gitter.im/SceneGate/Yarhl), but first make sure you've read the whole docs.


## Index

1. Your first project: Reading and Writing
2. Small introduction to Format
    2.1 BinaryFormat
3. Entering the virtual system: Nodes
    3.1. NodeContainerFormat
4. Converting Formats, Transform Nodes

## Your first Project: Reading and Writing

Oh, hi! I'm Master Yarhl (or M.Y.), nice to meet you, I will be your guide! Erm... y-you can.. picture me like this:

![Master Yarhl](https://i.imgur.com/w4TMqLi.png)

<small>If you want... or can...</small>

Lets' get started! The first module I'm teaching you is `Yarhl.IO` (Input Output), which is very similar to C# `System.IO`: `MemoryStream`, `BinaryWriter`, just with more functionality to work with binary and text files.

### Main Classes

This part is divided by binary file management classes and text file classes.

We have DataStream, DataReader and DataWriter for binary; TextReader and TextWriter for text. Easy peasy!

[Here](https://scenegate.github.io/Yarhl/api/Yarhl.IO.html) you can see every class with its properties, but let's see some of the most interesting ones:

- **DataStream:** Position, EndOfStream (bool if position is at the end) and Length of the stream.
- **DataReader** and **DataWriter**: DefaultEncoding, [Endianness](https://scenegate.github.io/Yarhl/api/Yarhl.IO.EndiannessMode.html) and Stream accessor.

### Main methods

#### DataStream

[Here](https://scenegate.github.io/Yarhl/api/Yarhl.IO.DataStream.html#methods) you can see all the methods, but as before we'll review the most interesting ones:

- **Compare (DataStream):** compare the content of the stream with another one.
- **PushCurrentPosition():** saves the current position.
- **PopPosition():** moves to the last position saved.
- Readers and Writers for buffers.
- **WriteTo:** save the stream into a physical file in your computer (giving the path) or into another DataStream. Very useful mate!

#### DataReader and DataWriter

We have a bunch of methods to Read and Write different type of data. You have the whole list [here](https://scenegate.github.io/Yarhl/api/Yarhl.IO.DataReader.html#methods) and [here](https://scenegate.github.io/Yarhl/api/Yarhl.IO.DataWriter.html#methods).

### Examples

#### Reading a File

```csharp
public void LoadFile(string path)
{
    using (var stream = new DataStream(path, FileOpenMode.Read)) {
        var reader = new DataReader(stream) {
            DefaultEncoding = new EscapeOutRangeEnconding("ascii"),
        };

        // Read!
    }
}
```

#### Writing a File

```csharp
public void SaveFile(string path)
{
    using (var stream = new DataStream(path, FileOpenMode.Read)) {
        var writer = new  DataWriter(stream);

        // Write into new file!
    }
}
```

## Small introduction to Format

Every game is composed by files, which has a specific format, for example .NCLR is a Palette file, or .aar is a package file. Yarhl helps you to code objects as you were actually coding a game file.

Continuing with the Palette example, you'd create a new class named NCLR with everything you need to store. Or if you have different Palette types with common content, you could create a Palette format and then the children.

Let's go for a quick example!

![Hex view of example file](https://i.imgur.com/KK5CsJH.png)

This file follows the following specification:

Size | Name
---- | -----
4    | Magic ID
2    |Number of sentences
2    | Size of the file
*    | Sentences

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

![A directory named mastering with tho files inside](https://i.imgur.com/80Qik3v.png)

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

![Hex view of example file](https://i.imgur.com/KK5CsJH.png)

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
