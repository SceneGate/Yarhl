# Locate types

After [loading external .NET assemblies](./load-assembly.md) containing
implementation of _formats_ and _converters_, the application can get a list of
them via `ConverterLocator`.

> [!NOTE]  
> This is only needed if the application does not know in advance the converter
> to use. It can present the list to the user so it can choose. Or it can get
> the converter names from a configuration file and later find the actual type
> via reflection. For instance for generic Tinke-like applications.

## TypeLocator

The `TypeLocator` provides features to find types that implement or inherit a
given base type. It searches in the **loaded assemblies** of an
`AssemblyLoadContext` instance. The default _singleton_ instance is accesible
via `TypeLocator.Default` and it uses `AssemblyLoadContext.Default`. Normally
you don't need to create your own instance.

> [!NOTE]  
> .NET loads assemblies lazily, when a code to run needs them. If you need a
> deterministic search consider loading every assembly from the application
> path. See
> [Load from executing directory](./load-assembly.md#load-from-executing-directory)
> for more information.

To find a list of types that inherit a given base class or implements an
interface use the method
[`FindImplementationsOf(Type)`](<xref:Yarhl.Plugins.TypeLocator.FindImplementationsOf(System.Type)>).
It searches for final types, that is: **classes that are public and not
abstract**. It returns information for each of these types in the _record_
[`TypeImplementationInfo`](xref:Yarhl.Plugins.TypeImplementationInfo)

For instance to find every _format_ in the loaded asssemblies use:

[!code-csharp[FindFormats](../../../src/Yarhl.Examples/Plugins/LocateTypesExamples.cs?name=FindFormats)]

The case of a _generic base type_ is special as types may implemented it
multiple. For instance a _class_ may implement `IConverter<Po, BinaryFormat>`
**and** `IConverter<BinaryFormat, Po>`. Using the _generic type definition_
(`typeof(IConverter<,>)`) to find types will throw an exception. Use this method
if you are searching for a specific implementation, like
`typeof(IConverter<Po, BinaryFormat>)`

Use the method
[`FindImplementationsOfGeneric(Type)`](<xref:Yarhl.Plugins.TypeLocator.FindImplementationsOfGeneric(System.Type)>)
to get a list of types implementing the **generic base type definition** with
any type arguments. For instance in the previous example calling
`FindImplementationsOfGeneric(typeof(IConverter<,>))` will return two results
for that class. One for `IConverter<Po, BinaryFormat>` and a second for
`IConverter<BinaryFormat, Po>`. The return type is the _record_
[`GenericTypeImplementationInfo`](xref:Yarhl.Plugins.GenericTypeImplementationInfo)

[!code-csharp[FindConverters](../../../src/Yarhl.Examples/Plugins/LocateTypesExamples.cs?name=FindConverters)]

## ConverterLocator

The [`ConverterLocator`](xref:Yarhl.Plugins.FileFormat.ConverterLocator) class
provides a cache of formats and converters found in the loaded assemblies.
During initialization (first use) it will use `TypeLocator` to find every format
and converter types. The `Default` singleton instance use `TypeLocator.Default`.
You can pass a custom `TypeLocator` via its public constructor.

The properties
[`Converters`](xref:Yarhl.Plugins.FileFormat.ConverterLocator.Converters) and
[`Formats`](xref:Yarhl.Plugins.FileFormat.ConverterLocator.Formats) provides a
list of the types found, so there is no need to re-scan the assemblies each
time.

> [!NOTE]  
> If a new assembly is loaded in the `AssemblyLoadContext`, the
> `ConverterLocator` will need to performn a re-scan to find the new types. Make
> sure to call
> [`ConverterLocator.ScanAssemblies()`](xref:Yarhl.Plugins.FileFormat.ConverterLocator.ScanAssemblies)
> after loading new assemblies.
