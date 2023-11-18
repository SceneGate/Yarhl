# Node

In Yarhl, a **node** is an entity that has a **name** and a
[**format**](../formats/formats.md). The concept is similar to a file or folder
on a computer hard-drive, but in this case is virtual, it's not a physical file
in the disk. Its format can be some bytes in a `Stream` like a disk file, but it
could also be any class that implements the
[`IFormat`](xref:Yarhl.FileFormat.IFormat) interface.

![node with name, format and path properties](images/node-simple.drawio.png)

If the _format_ of the node is `NodeContainerFormat`, then we say that the node
is a **container**. This is the equivalent of a folder in our file system. These
nodes may have one or more node **children**.

By having nodes with children we can create a hierarchy similar to the file
system of our drives. We could navigate identify and navigate by their **path**.

![previous node with three children nodes each with different set of properties](images/node-children.drawio.png)

Nodes is a core feature of Yarhl. It allows to represent with a hierarchy a set
of formats. Running [converters](../formats/converters.md) we can transform the
content of the nodes, for instance by reading, writing, unpacking, etc.

Combining nodes, [converters](../formats/converters.md) and the
[_sub-stream_ concept of `DataStream`](../binary/datastream.md#sub-streams), we
can represent a complex file system even when all the file data points to a
single `Stream` from a disk file.

Let's see it with an example from the
[Ekona](https://scenegate.github.io/Ekona/index.html) library that provides
implementation for _Nintendo DS_ formats.

[!code-csharp[overview](./../../../../src/Yarhl.Examples/FileSystem/NodeExamples.cs?name=Overview)]

## Children

A node may have children if it has a container type. Children are stored as
references in a collection and can be accessed via the property `Children`.

It's possible to iterate `Children` with a `foreach` or a regular `for` and get
the number of children with `node.Children.Count`.

To access to a child use its index, `Children[3]`, or its name,
`Children["image.png"]`. You can chain this operation to navigate the hierarchy:
`node.Children[1].Children["map1.scr"]`.

[!code-csharp[children](./../../../../src/Yarhl.Examples/FileSystem/NodeExamples.cs?name=AccessChildren)]

> [!TIP]  
> You can find more ways to iterate or navigate nodes across a hierarchy in the
> [`Navigator`](xref:Yarhl.FileSystem.Navigator) class.

### Add or remove

It's possible to add or remove children from its parent node. Use the method
`Add` to add one or more nodes as its children.

Use the `Remove` method to remove a child from its parent by instance reference
or node name. The method will **not throw an exception** if the node to remove
is not found. The method will return `false` in those cases.

The method `RemoveChildren` removes all the children. Additionally its parameter
allow to dispose them.

[!code-csharp[add remove](./../../../../src/Yarhl.Examples/FileSystem/NodeExamples.cs?name=AddRemove)]

> [!NOTE]  
> A removed child **is not disposed**. Consider freeing its formats, especially
> if they have binary type or are containers. `RemoveChildren` does offer the
> possibility to dispose the children.

> [!IMPORTANT]  
> A node cannot have two children with the same name. Adding a node with the
> same name will replace the node.

> [!IMPORTANT]  
> You should not add or remove while iterating the `Children` property.

## Format

A node can have any [format](../formats/formats.md) type that implements the
interface `IFormat`. It can also have a `null` format.

The `Format` property gets access to the format by returning an instance of type
`IFormat`. For convenience, there is also the method `GetFormatAs<T>()` that
tries to cast the node format to the desired type. It will **return `null`** if
the casting is not possible.

The property `IsContainer` returns `true` when the node have a type that allows
having children. This would be `NodeContainerFormat` or `null`.

The property `Stream` is a shortcut to `GetFormatAs<IBinary>().Stream` and it
will also return `null` if the format is not an implementation of `IBinary`.

### Changing format

The node can change its format via the method `ChangeFormat(format)`.

If the current format is a container, first it will remove any children from
this node. If the future format is a container, it will move the children from
the format to the node.

Additionally there is an optional argument to indicate if the method should
dispose the current format before changing. By default is `true`, meaning it
will call the method `Dispose` from the current format if it implements
`IDisposable`.

## Format conversion

Apart from the changing the format API, a _node_ also provides methods to
_transform_ the format by using a [converter](../formats/converters.md).

If the converter does not need any
[parameter](../formats/converters.md#parameters) and it has a public
parameterless constructor (default case) you can use the short API
[`TransformWith<Converter>()`](xref:Yarhl.FileSystem.Node.TransformWith``1)

If it takes parameters or the instance needs to be created in a different way
(e.g. factory), pass the converter object via
[`TransformWith(converter)`](<xref:Yarhl.FileSystem.Node.TransformWith``2(Yarhl.FileFormat.IConverter{``0,``1})>).

[!code-csharp[transform](./../../../../src/Yarhl.Examples/FileSystem/NodeExamples.cs?name=Transform)]

The `TransformWith()` method are a shortcut method to run a converter with the
current node's format and then call `ChangeFormat()` with the result. Note the
considerations of [changing format](#changing-format) like what it would happen
for container formats.

The method returns the same instance of the node. This allows a syntax
fluent-like for chaining conversions.

[!code-csharp[transform chaining](./../../../../src/Yarhl.Examples/FileSystem/NodeExamples.cs?name=TransformChain)]

## Tags

Nodes can store additional metadata via the generic dictionary `Tags`. Each tag
has a `string` as a key and it can have any type as value. Use it to store
metadata of the node, outside of its regular format.

Converters may use the `Tags` to provide additional information about the nodes.
For instance, the _Ekona_ library adds to every node the tag
`scenegate.ekona.id` with the internal ID of the file in the game file.

> [!NOTE]  
> **Avoid depending on _tags_ in a converter.**  
> It could be that the node has a given tag just after running a converter. But
> if it was created from a file on disk it may not have it again.

## Cloning a node

The constructor
[`Node(node)`](<xref:Yarhl.FileSystem.Node.%23ctor(Yarhl.FileSystem.Node)>)
allows to do a _deep_ clone of a node. This includes name, format and tags.

If the format of the source node is not null, **it must implement
[`ICloneableFormat`](../formats/cloneable-format.md)**. As children are also
_deep_ cloned, all of them must have a format that implements
`ICloneableFormat`.

As `NodeContainerFormat` implements `ICloneableFormat`, the children of the node
will also be deep cloned.

## Dispose

`Node` implements `IDisposable`. The `Dispose` method will remove and dispose
all of its children, recursively. It will also dispose its format if it
implements `IDisposable`.

> [!IMPORTANT]  
> A node cannot be used anymore after calling `Dispose`. This also affects to
> all its children (recursively).
