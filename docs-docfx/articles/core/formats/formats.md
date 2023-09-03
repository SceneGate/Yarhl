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

By using the [`IFormat`](xref:Yarhl.FileFormat.IFormat) interface it allows the
APIs to:

- Provide extension methods that applies to formats only (like `ConvertWith`).
- Provide type discovery for _formats_ via _Yarhl.Plugins_.
- Prevent unboxing performance issues.

## Working with existing models

Models should implement the [`IFormat`](xref:Yarhl.FileFormat.IFormat)
interface. If you have a model and cannot be modified to inherit from the
interface, then it's possible to create a _format wrapper_.

For instance, let's see how to provide a format-compatible class for a
third-party sound format `ThirdPartyWave`:

[!code-csharp[format wrapper](./../../../../src/Yarhl.Examples/Formats.cs?name=FormatWrapper)]
