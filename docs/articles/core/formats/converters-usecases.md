# Advanced uses cases for converters

The [converters](./converters.md) topic covers the standard use cases _convert
one format into another_. However often you may run into more advanced
scenarios. The following sections tries to provide some architecture guidance.

## Convert multiple formats into one

The recommended design is to create a converter for the **main format and use
parameters** to pass the additional formats.

Let's try to see with a couple of examples:

### Create a font file from an image file and an JSON file

We identify the _main_ format as the JSON structure as it contains most of the
information required to setup the format. An image file goes as parameter to be
used for the glyphs of the font.

[!code-csharp[Font2Binary example](../../../../src/Yarhl.Examples/Formats/AdvancedConverters.cs?name=ManyToOneFont&highlight=5)]

> [!NOTE]  
> Instead of passing a JSON binary data, pre-convert it already into its
> structure / class. It will simplify the implementation of the converter and it
> could be it can be re-used for more cases (e.g. in the future you decide to
> change to YAML).

### Convert an indexed image with a palette into an RGB image

The _main_ format would be the indexed image as contains more information
representing the target format. A palette is required to transform the pixel
indexes into a final RGB color.

[!code-csharp[IndexedImage2RgbImage example](../../../../src/Yarhl.Examples/Formats/AdvancedConverters.cs?name=ManyToOneIndexedImage&highlight=5)]

### Additional patterns for many to one

Below other implementations that may fit some use cases. In our experience they
don't work as good as the previous mentioned _parameter_ approach.

#### Intermediary types

Create an intermediary type that groups all the required formats to convert. For
instance you could create a class `IndexedImageWithPalette` to gather the
`IndexedImage` and `Palette`. Then create a converter for
`IConverter<IndexedImageWithPalette, RgbImage>`.

This may simplify your converter but it can create more complex APIs. Now users
will need to _convert_ their two formats into this intermediary representation
before they can use the converter.

It may prevent a _fluent-like_ usage of the converters when used with the
[node](../virtual-file-system/nodes.md#format-conversion) APIs. It won't allow
to convert one _node_ passing other _node_ as parameters.

#### Using tuples as input type

This similar to the above case. It has the further limitation that it can't
evolve over the time. If you need an additional format or parameter in the
future you will breaking the API for the users making it a bit more messy.

## Convert one format into many

Depending on the use cases you may want to:

1. **Convert the format into a container type `NodeContainerFormat`** that
   contains a child per output format.
2. Create a **separate** converter for each target format.

> [!TIP]  
> Check-out the [node](../virtual-file-system/nodes.md) topic to learn more
> about containers.

### Convert an RGB image into indexed image and palette

Reverse operation from
[convert an indexed image with palette into RGB image](#convert-an-indexed-image-with-a-palette-into-an-rgb-image).
As this converter will generate a palette where the _indexed pixels_ will point
to, we will need to return it as well.

We will return a container with a child `image` and another `palette`.

[!code-csharp[RgbImage2IndexedImage example](../../../../src/Yarhl.Examples/Formats/AdvancedConverters.cs?name=OneToManyIndexedImage)]

The user of the API would be able to extract both formats later:

[!code-csharp[Using previous converter](../../../../src/Yarhl.Examples/Formats/AdvancedConverters.cs?name=OneToManyIndexedImageProgram)]

### Export a font into information and image

In this case it could be a better approach to separate the converters:

1. A `Font2BinaryInfo` converter that serializes the charset map and other
   information into JSON / YAML `Stream`.
2. A `Font2Image` converter that exports the glyphs into an image.

Each converter runs a different process to generate the output. These two output
formats are not generated at the same time (as it was the case above).

By splitting it allows users to run the one they need when they need it. It may
not be required to generate an image all the time or vice-versa.

## Convert multiple formats into many

This use case would be covered by the two previous cases: combining converting
[multiple formats into one](#convert-multiple-formats-into-one) and
[one format into many](#convert-one-format-into-many).

## Updating / Importing data in a format

Sometimes you may run a process that modifies existing data of a format
**without creating a new format**.

For instance, if there is an unknown or complex binary format like an executable
and we want to **only change its text**.

In these cases we can create a converter that **returns the same input
instance** after processing. We can pass the data to import as a **parameter**.

Let's see an example:

[!code-csharp[ExecutableTextImporter example](../../../../src/Yarhl.Examples/Formats/AdvancedConverters.cs?name=ExecutableTextImporter)]

> [!TIP]  
> It could be a good idea to create a **new `BinaryFormat` and copy the input
> before overwriting data**. In that case you would be returning a new binary
> format but with the existing content. In this way you don't modify the
> existing file on disk but create a new one in case something wrong happens and
> you want to run it again.
