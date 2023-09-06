# Cloneable format

.NET does not provide an interface to guarantee a
[deep clone](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable?view=net-7.0#remarks)
implementation.

The [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat) gives the
possibility to a format implementation to specify how it should _deep_ clone its
data into a new format. This could be as simple as copying its properties into a
new object or in the case of binary data, copying all its bytes into a new
stream.

[!code-csharp[cloneable](./../../../../src/Yarhl.Examples/Formats/Formats.cs?name=CloneableFormat)]

The interface already implements `IFormat` so it's not needed to add both.

> [!IMPORTANT]  
> This interface is not required to be implemented by every format but some APIs
> of the library relies on it. For instance it's only possible to clone a
> [node via its constructor](<xref:Yarhl.FileSystem.Node.%23ctor(Yarhl.FileSystem.Node)>)
> if it has a format that implements
> [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat).

> [!TIP]  
> The built-in formats from _Yarhl_ implements
> [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat).
