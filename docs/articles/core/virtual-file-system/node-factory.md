# Node factory

The [`Node`](xref:Yarhl.FileSystem.Node) constructor requires at least a
**non-null name**. It's not required to provide an _extension_ in the name. Its
format can be initially `null`, you can [change it later](nodes.md#format).

```csharp
IFormat binary = ...;
using var node = new Node("myNode", binary);

using var nodeNoFormat = new Node("myNode2");
```

> [!NOTE]  
> There is another overload of the `Node` constructor with a `Node` parameter.
> It's covered in the [node clone](nodes.md#cloning-a-node) topic.

To cover common use cases, the
[`NodeFactory`](xref:Yarhl.FileSystem.NodeFactory) provides APIs to create nodes
with a format quickly.

## Create node with binary data

Similar to the [`DataStreamFactory`](xref:Yarhl.IO.DataStreamFactory), we can
create a node with binary data from different sources. In all these cases the
node will have the format [`BinaryFormat`](xref:Yarhl.IO.BinaryFormat).

- New memory buffer:
  [`FromMemory(name)`](<xref:Yarhl.FileSystem.NodeFactory.FromMemory(System.String)>)
- Byte array:
  [`FromArray(name, data)`](<xref:Yarhl.FileSystem.NodeFactory.FromArray(System.String,System.Byte[])>)
- Segment of a byte array:
  [`FromArray(name, data, offset, length)`](<xref:Yarhl.FileSystem.NodeFactory.FromArray(System.String,System.Byte[],System.Int32,System.Int32)>)
- .NET `Stream`:
  [`FromStream(name, stream)`](<xref:Yarhl.FileSystem.NodeFactory.FromStream(System.String,System.IO.Stream)>)
- Segment of a .NET `Stream`:
  [`FromSubStream(name, stream, offset, lengt)`](<xref:Yarhl.FileSystem.NodeFactory.FromSubstream(System.String,System.IO.Stream,System.Int64,System.Int64)>)

<!-- TODO: example -->

## Create nodes from files

The `FromFile` overloads allows to create a new node to access the data from a
file on disk. The node will have the format
[`BinaryFormat`](xref:Yarhl.IO.BinaryFormat).

The
[`FromFile(path, mode)`](<xref:Yarhl.FileSystem.NodeFactory.FromFile(System.String,Yarhl.IO.FileOpenMode)>)
creates the node with the name from file. This includes the file extension as
well.

The overload
[`FromFile(path, name, mode)`](<xref:Yarhl.FileSystem.NodeFactory.FromFile(System.String,System.String,Yarhl.IO.FileOpenMode)>)
allows to set a specific name for the node that differs from the name of the
file on disk.

If the path points to a symbolic link from _Windows_ or _Unix_, it will resolve
to the target.

> [!TIP]  
> The new node will have a [tag](nodes.md#tags) named `FileInfo` containing an
> instance of .NET
> [`FileInfo`](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileinfo)
> for the given path. If the path was pointing to a symbolic link, the file info
> will contain information of the link, not the actual target.

<!-- TODO: example -->

## Create a container node

The
[`CreateContainer(name)`](<xref:Yarhl.FileSystem.NodeFactory.CreateContainer(System.String)>)
method allows to create a node with the given node for _container_ usage. It
will have the format `NodeContainerFormat`.

## Create nodes from directories

The factory contains a set of APIs to create a node hierarchy that replicates
the files and folders from a given path on disk.

The first method is
[`FromDirectory(path, filter, mode)`](<xref:Yarhl.FileSystem.NodeFactory.FromDirectory(System.String,System.String,Yarhl.IO.FileOpenMode)>).
By default the `filter` is `*`. It creates a new node with children **for each
file** on the given path. **It does not iterate recursively and it will ignore
folders on the path**. It opens the files with the provided `mode`. Optionally
it's possible to pass a `string` with a simple _filter_ or search pattern. More
information about the filter from the .NET API
[`Directory.GetFiles`](<https://learn.microsoft.com/en-us/dotnet/api/system.io.directory.getfiles#system-io-directory-getfiles(system-string-system-string)>).
The node will have the name of the root directory from the given path.

<!-- TODO: example -->

The method
[`FromDirectory(path, filter, name, iterateDirectories, mode)`](<xref:Yarhl.FileSystem.NodeFactory.FromDirectory(System.String,System.String,System.String,System.Boolean,Yarhl.IO.FileOpenMode)>)
behaves similar but allows to specify the name of the node. It also has a new
argument to specify if it should **iterate recursively through any directory**
and create the full hierarchy.

<!-- TODO: example -->

To provide advanced filtering capabilities, there are two more methods that
behaves similar to above. In these cases instead of a `string` to filter, you
can specify a function (lambda or method) that takes as an argument a file path
and returns a boolean to accept or not the file.

<!-- TODO: example -->

## Create node hierarchy

TODO
