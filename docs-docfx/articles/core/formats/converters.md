# Converters

You can convert a [formats](./formats.md) (model) into another format by using a
_converter_ class. A _Yarhl converter_ implements the interface
[`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2) and provides the
method [`TDst Convert(TSrc)`](<xref:Yarhl.FileFormat.IConverter`2.Convert(`0)>).
This method creates a new object in the target type _converting_ the data from
the input.

For instance the converter [`Po2Binary`](xref:Yarhl.Media.Text.Po2Binary)
implements `IConverter<Po, BinaryFormat>`. It allows to convert a
[`Po`](xref:Yarhl.Media.Text.Po) model format into a
[_binary_ format](xref:Yarhl.IO.BinaryFormat). This is also known as
**serialization**. You can later write this binary data into a file on disk.

In a similar way, the converter [`Binary2Po`](xref:Yarhl.Media.Text.Binary2Po)
implements `IConverter<IBinary, Po>` to convert binary data into a
[`Po`](xref:Yarhl.Media.Text.Po) model (also known as _reading_ or
_deserializing_).

We could have more conversions between formats. For instance
`IConverter<Po, Excel>` or `IConverter<Excel, Po>`. This is sometimes referred
as _exporting_ and _importing_ formats. _Converters_ simplify all these
operations by their common denominator: **converting models.**

Let's see how to _serialize_ / convert a _Po_ model into binary data to write on
disk:

[!code-csharp[serialize PO](./../../../../src/Yarhl.Examples/Converters.cs?name=SerializePo)]

## Implementing a new converter

To create a new converter, create a new class and implement the interface
[`<IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2). `TSrc` is the
type (or base type / interface) you are going to convert into a new object of
`TDst` type.

> [!NOTE]  
> It is possible to have a class implementing more than one converter at a type.
> However this can be confusing for the user. Our recommendation is that each
> class implements only one
> [`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2) interface. For
> instance, create `Po2Binary` and `Binary2Po` instead of just `Binary2Po`
> having the two implementations.

As an example, let's implement a new converter that reads binary data and
creates a [container type](../virtual-file-system/nodes.md) (like a file
system).

First we create a new class for our converter: `BinaryArchive2Container` to do
the operation _binary data_ -> _container class_ (deserializing).

```csharp
public class BinaryArchive2Container : IConverter<IBinary, NodeContainerFormat>
{
    // TODO: Implement interface.
}
```

Now let's add the required method `Convert` for the interface.

```csharp
public NodeContainerFormat Convert(IBinary source)
{
    var container = new NodeContainerFormat();
    // TODO: do something with the source data.
    return container;
}
```

Finally let's read some data to fill the container. This example binary format
contains a set of binary files inside.

```csharp
public class BinaryArchive2Container : IConverter<IBinary, NodeContainerFormat>
{
    public NodeContainerFormat Convert(IBinary source)
    {
        // Format: number of files + table with "name + offset + size", then file data.
        var reader = new DataReader(source.Stream);
        var container = new NodeContainerFormat();

        int numFiles = reader.ReadInt32();
        for (int i = 0; i < numFiles; i++)
        {
            string name = reader.ReadString(bytesCount: 0x10, encoding: Encoding.UTF8);
            uint offset = reader.ReadUInt32();
            uint size = reader.ReadUInt32();

            // Create a sub-stream for the child, a stream from a region
            // of the parent stream without making any read/write or copies.
            Node child = NodeFactory.FromSubstream(name, source.Stream, offset, size);
            container.Root.Add(child);
        }

        return container;
    }
}
```

And voilà. To use our new converter we just need to create a new instance and
pass some binary data.

```csharp
// Convert the binary file into a virtual folder (no disk writing).
var fileStream = DataStreamFactory.FromFile("myArchive.bin", FileOpenMode.Read);
using var binaryFormat = new BinaryFormat();

var binary2Container = new BinaryArchive2Container();
using var container = binary2Container.Convert(binaryFormat);

// Now we can inspect or extract the content of the container
Node child = container.Children["text.json"]
child.Stream.WriteTo(child.Name);
```

## Parameters

Frequently your converter may require additional parameters than just the input
object to do the conversion. For instance in a compressor you may need to ask
your users to provide the level of compression to do. Or you may need to know
the line ending for a text format. You may need to know if the target CPU is big
or little endian or the text encoding.

In any of these cases, you can ask the user to provide this required or optional
information in the constructor of the converter class.

> [!TIP]  
> If your converter can run with some _default_ parameters, provide a
> parameter-less constructor to simplify its usage for common use cases.

```csharp
public class RgbImage2IndexedImage : IConverter<RgbImage, IndexedImage>
{
    private readonly IColorQuantization quantization;

    // Parameter-less constructors for a default value that can be used in most cases.
    public RgbImage2IndexedImage()
    {
        quantization = new ColorQuantization();
    }

    // Allow the user to customize the converter to their needs.
    public RgbImage2IndexedImage(IColorQuantization customQuantization)
    {
        quantization = customQuantization;
    }

    public IndexedImage Converter(RgbImage source)
    {
        // Use the quantization instance to convert RGB colors into an indexed image
        // ...
    }
}
```

## `IConverter` interface

> [!IMPORTANT]  
> Normally the [`IConverter`](xref:Yarhl.FileFormat.IConverter) (no generics
> version) is for internal use only. Unless writing a new framework or generic
> tools, use always
> [`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2).

You may notice that there is also an
[`IConverter`](xref:Yarhl.FileFormat.IConverter) interface that takes no
generics. The [`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2)
_implements_ this base interface.

This is an empty interface used only internally to enforce some basic
type-safety when due to technical reason we can't know the types of the
converter, so we can't use
[`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2).

For instance
[`Node.TransformWith(IConverter converter)`](<xref:Yarhl.FileSystem.Node.TransformWith(Yarhl.FileFormat.IConverter)>)
uses the base interface to provide a simple API. Requiring the fully typed
interface would make users to specify to repeat the types:
`node.TransformWith<IBinary, NodeContainerFormat>(myConverter)` as the compiler
cannot guess these types at compile-type. By having the simple interface we can
just use `node.TransformWith(myConverter)`.

Note that when the API uses [`IConverter`](xref:Yarhl.FileFormat.IConverter) it
will run reflection run-time checks to ensure the argument is valid. It will
check that the variable or type implements
[`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2) and that the
input object is valid for this type. Although it may hit some nanoseconds of
performance, it provides better error messages.
