# Converters

You can convert a model (formats) into another with _converter_ classes. A
_Yarhl converter_ implements the interface
[`IConverter<TSrc, TDst>`](xref:Yarhl.FileFormat.IConverter`2) and provides the
method [`TDst Convert(TSrc)`](<xref:Yarhl.FileFormat.IConverter`2.Convert(`0)>).

For instance the converter [`Po2Binary`](xref:Yarhl.Media.Text.Po2Binary)
implements `IConverter<Po, BinaryFormat>` allowing you to convert a
[`Po`](xref:Yarhl.Media.Text.Po) model format into a
[_binary_ format](xref:Yarhl.IO.BinaryFormat). This is also known as
**serialization**.

You can use it by creating a new instance and calling its `Convert(Po)` method:

[!code-csharp[serialize PO](./../../../../src/Yarhl.Examples/Converters.cs?name=SerializePo)]

## Implementing a new converter

TODO

```csharp
// Implement a new format container from binary (file) into a virtual file system.
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

// Convert the binary file into a virtual folder (no disk writing).
using Node root = NodeFactory.FromFile("file.bin", FileOpenMode.Read);
root.TransformWith<BinaryArchive2Container>(); // Binary -> node format

// Extract a child into disk.
Node child = root.Children["text.json"]   // Navigate the children
child.Stream.WriteTo("output/text.json"); // Export to the disk (creates missing dirs)
```

## `IConverter` interface

TODO

## Converters with parameters

TODO

## Converting many formats into one

TODO

## Converting one format into many

TODO

## Updating / Importing data in a format

TODO
