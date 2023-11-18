namespace Yarhl.UnitTests.FileSystem;

using System;
using System.Globalization;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.UnitTests.FileFormat;

public partial class NodeTests
{
    [Test]
    public void TransformWithInit()
    {
        var dummyFormat = new StringFormat("C0");
        using var node = new Node("mytest", dummyFormat);

        _ = node.TransformWith<StringFormatConverterWithInitializerInterface, NumberStyles>(NumberStyles.HexNumber);
        Assert.IsInstanceOf<IntFormat>(node.Format);
        Assert.AreNotSame(dummyFormat, node.Format);
        Assert.AreEqual(192, node.GetFormatAs<IntFormat>().Value);
    }

    [Test]
    public void TransformWithInitReturnsNode()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        Assert.That(
            node.TransformWith<StringFormatConverterWithInitializerInterface, NumberStyles>(NumberStyles.HexNumber),
            Is.SameAs(node));
    }

    [Test]
    public void TransformWithInitDoesNotThrowIfReturnsNull()
    {
        var dummy = new NullSource();
        using var node = new Node("mytest", dummy);

        Assert.That(
            () => node.TransformWith<NullConverter, object>(null),
            Throws.Nothing);
        Assert.That(node.Format, Is.Null);
    }

    [Test]
    public void TransformWithInitializerInterfaceThrowsIfNoConverterImplementation()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        const string msg = "Converter doesn't implement IConverter<,>";
        Assert.That(
            () => node.TransformWith<ConverterWithoutGenericInterface, int>(0),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }

    [Test]
    public void TransformWithInitializerInterfaceThrowsIfConverterDoesNotReturnIFormat()
    {
        var dummy = new StringFormat("3");
        using var node = new Node("mytest", dummy);

        Assert.That(
            () => node.TransformWith<StringFormat2NoFormat, int>(2),
            Throws.InvalidOperationException);
    }

    [Test]
    public void TransformWithInitializerInterfaceAfterDisposeThrowsException()
    {
        using var dummyFormat = new StringFormat("3");
        var node = new Node("mytest", dummyFormat);
        node.Dispose();

        Assert.That(
            () => node.TransformWith<StringFormatConverterWithInitializerInterface, NumberStyles>(NumberStyles.Number),
            Throws.TypeOf<ObjectDisposedException>());
    }

    [Test]
    public void TransformWithInitializerInterfaceWithoutFormatThrowException()
    {
        const string msg = "Cannot transform a node without format";
        using var node = new Node("mytest");

        Assert.That(
            () => node.TransformWith<StringFormatConverterWithInitializerInterface, NumberStyles>(NumberStyles.Integer),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }
}
