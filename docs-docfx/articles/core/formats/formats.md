# Formats

In Yarhl, _formats_ are _.NET classes_ that represents a model that can be
converted and/or assigned into virtual files (_nodes_).

## Implement a new format

A format is just a regular programming model, a _.NET class_ (or _record_),
usually with properties and methods. The only requirement to have a _Yarhl
format-compatible_ is to implement the empty interface
[`IFormat`](xref:Yarhl.FileFormat.IFormat).

[!code-csharp[format implementation](./../../../../src/Yarhl.Examples/Formats.cs?name=FormatImpl)]

## Converting formats

> [!NOTE]  
> Check-out the [converters](./converters.md) topic to learn more about them.

The _converters_ classes are responsible to convert one format into a new one.
To use it, create a new instance and call its
[`Convert(source)`](<xref:Yarhl.FileFormat.IConverter`2.Convert(`0)>) method.

[!code-csharp[serialize PO](./../../../../src/Yarhl.Examples/Converters.cs?name=SerializePo)]

### Fluent API

An easier way, it's to use the extension method on formats
`ConvertWith(converter)`. As it returns the new format, it allows to _chain
conversions_:

```csharp
FullImage fontImage = binaryFont
    .ConvertWith(new Binary2Font(FontKind.Debug)) // binary -> font model
    .ConvertWith(new Font2Image());               // font -> image
```

### Converting without knowing converter at compile-time

Sometimes the application doesn't know the converter type at compile-time. This
could be the case of generic tools that loads assemblies in a plugin-style and
select the converter type via configuration file or user interface.

The static class [`ConvertFormat`](xref:Yarhl.FileFormat.ConvertFormat) provides
the APIs to convert formats by passing its type object. The API uses reflection
to validate the converter type and its arguments so that it throws an exception
when:

- the type does not implement `IConverter<TSrc, TDst>`
- the converter cannot convert the type of the input.
- the parameters does not match any constructor signature.

```csharp
object inputFormat;

// UI / config file loaded from same or external assembly
Type converterType;
object[] converterArgs;

object outputFormat = ConvertFormat.With(converterType, inputFormat, converterArgs);
```

> [!IMPORTANT]  
> Note that as it uses reflection it's not as performant as other APIs. It also
> lose the ability to have type-safe code. If one of the converter change its
> interfaces or parameters between versions it may throw unexpected exceptions.

## `IFormat` interface

The converter interface does not have any requirements for the types it could
convert. You can theoretically implement `IConverter<string, int>`. However, in
order to provide some features the library expects that every format implements
the _empty_ interface [`IFormat`](xref:Yarhl.FileFormat.IFormat).

By using the `IFormat` interface it allows the APIs to:

- Provide extension methods that applies to formats only (like `ConvertWith`).
- Provide type discovery for _formats_ via _Yarhl.Plugins_.
- Prevent unboxing performance issues.

### Working with existing models

Models should implement the `IFormat` interface. If you have a model and cannot
be modified to inherit from the interface, then it's possible to create a
_format wrapper_.

For instance, let's see how to provide a format-compatible class for a
third-party sound format `ThirdPartyWave`:

[!code-csharp[format wrapper](./../../../../src/Yarhl.Examples/Formats.cs?name=FormatWrapper)]

## Cloneable formats

.NET does not provide an interface to guarantee a
[deep clone](https://learn.microsoft.com/en-us/dotnet/api/system.icloneable?view=net-7.0#remarks)
implementation.

The [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat) gives the
possibility to a format implementation to specify how it should _deep_ clone its
data into a new format. This could be a simple as copying its properties into a
new object or in the case of binary data copying all its bytes into a new
stream.

[!code-csharp[cloneable](./../../../../src/Yarhl.Examples/Formats.cs?name=CloneableFormat)]

The interface already implements `IFormat` so it's not needed to implement both.

> [!NOTE]  
> This interface is not required to be implemented by every format but some APIs
> of the library relies on it. For instance it's only possible to clone a
> [node via its constructor](<xref:Yarhl.FileSystem.Node.%23ctor(Yarhl.FileSystem.Node)>)
> if it has a format that implements
> [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat).

> [!NOTE]  
> The built-in formats from _Yarhl_ implements
> [`ICloneableFormat`](xref:Yarhl.FileFormat.ICloneableFormat).
