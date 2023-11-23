// Copyright (c) 2023 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.Examples.Formats;

using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

#pragma warning disable SA1649 // File name match type name

#region ManyToOneFont
public class Font2Binary : IConverter<Font, BinaryFormat>
{
    private readonly RgbImage fontImage;

    public Font2Binary(RgbImage fontImage)
    {
        this.fontImage = fontImage;
    }

    public BinaryFormat Convert(Font source)
    {
        var binaryFont = new BinaryFormat();

        // TODO: Use source and fontImage to serialize into binary.
        return binaryFont;
    }
}
#endregion

#region ManyToOneIndexedImage
public class IndexedImage2RgbImage : IConverter<IndexedImage, RgbImage>
{
    private readonly Palette palette;

    public IndexedImage2RgbImage(Palette palette)
    {
        this.palette = palette;
    }

    public RgbImage Convert(IndexedImage source)
    {
        var fullImage = new RgbImage();

        // TODO: Convert each pixel into RGB using the provided palette.
        return fullImage;
    }
}
#endregion

#region OneToManyIndexedImage
public class RgbImage2IndexedImage : IConverter<RgbImage, NodeContainerFormat>
{
    public NodeContainerFormat Convert(RgbImage source)
    {
        // TODO: Run a quantization algorithm that generates a palette and indexed pixels
        IndexedImage indexedImage = null;
        Palette palette = null;

        var container = new NodeContainerFormat();

        var indexedImageNode = new Node("image", indexedImage);
        container.Root.Add(indexedImageNode);

        var paletteNode = new Node("palette", palette);
        container.Root.Add(paletteNode);

        return container;
    }
}
#endregion

#region FontImporter
public class FontGlyphsImporter : IConverter<Font, Font>
{
    private readonly RgbImage glyphs;

    public FontGlyphsImporter(RgbImage glyphs)
    {
        this.glyphs = glyphs;
    }

    public Font Convert(Font source)
    {
        // TODO: Update Font instance with the glyph images
        return source;
    }
}
#endregion

#region ExecutableTextImporter
public record TextBlockInfo(uint Position, string Text);

public class ExecutableTextImporter : IConverter<IBinary, IBinary>
{
    private readonly IEnumerable<TextBlockInfo> textInfos;

    public ExecutableTextImporter(IEnumerable<TextBlockInfo> textInfos)
    {
        this.textInfos = textInfos ?? throw new ArgumentNullException(nameof(textInfos));
    }

    public IBinary Convert(IBinary source)
    {
        var writer = new DataWriter(source.Stream);

        foreach (var info in textInfos) {
            writer.Stream.Position = info.Position;

            // you should check it doesn't overwrite more data than it can
            writer.Write(info.Text);
        }

        return source;
    }
}
#endregion

public static class Program
{
    public static void OneToManyIndexedImage()
    {
        #region OneToManyIndexedImageProgram
        using Node imageNode = NodeFactory.FromFile("image.png", FileOpenMode.Read)
            .TransformWith<BinaryPng2RgbImage>()
            .TransformWith<RgbImage2IndexedImage>();

        var indexedImage = imageNode.Children["image"].GetFormatAs<IndexedImage>();
        var palette = imageNode.Children["palette"].GetFormatAs<Palette>();
        #endregion
    }
}

public class Font : IFormat
{
}

public class RgbImage : IFormat
{
}

public class IndexedImage : IFormat
{
}

public class Palette : IFormat
{
}

public class BinaryPng2RgbImage : IConverter<IBinary, RgbImage>
{
    public RgbImage Convert(IBinary source) => throw new NotImplementedException();
}
