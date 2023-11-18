namespace Yarhl.UnitTests.FileSystem;

using System;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.UnitTests.FileFormat;

[TestFixture]
public partial class NodeTests
{
    [Test]
    public void ConstructorSetName()
    {
        using var node = new Node("mytest");
        Assert.AreEqual("mytest", node.Name);
    }

    [Test]
    public void ConstructorSetNameAndFormat()
    {
        var dummyFormat = new StringFormat(string.Empty);
        using var node = new Node("mytest", dummyFormat);
        Assert.AreEqual("mytest", node.Name);
        Assert.AreSame(dummyFormat, node.Format);
    }

    [Test]
    public void ConstructorWithoutFormatNullProperty()
    {
        using var node = new Node("mytest");
        Assert.IsNull(node.Format);
    }

    [Test]
    public void ConstructorAllowsNullFormat()
    {
        using var node = new Node("mytest", null);
        Assert.That(node.Format, Is.Null);
    }

    [Test]
    public void GetStreamWhenBinaryFormatReturnsStream()
    {
        var format = new BinaryFormat();
        using var node = new Node("myteset", format);
        Assert.AreEqual(format.Stream, node.Stream);
        node.Dispose();
    }

    [Test]
    public void GetStreamIfFormatIsNotBinaryFormatReturnsNull()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);
        Assert.IsNull(node.Stream);
    }

    [Test]
    public void GetStreamWithoutFormatReturnsNull()
    {
        using var node = new Node("mytest");
        Assert.IsNull(node.Stream);
    }

    [Test]
    public void IsContainerIfFormatNodeContainer()
    {
        var format = new NodeContainerFormat();
        using var node = new Node("NodeTest", format);
        Assert.IsTrue(node.IsContainer);
    }

    [Test]
    public void IsNotContainerForDifferentFormat()
    {
        var format = new StringFormat("3");
        using var node = new Node("NodeTest", format);
        Assert.IsFalse(node.IsContainer);
    }

    [Test]
    public void IsNotContainerIfNoFormat()
    {
        using var node = new Node("mytest");
        Assert.That(node.IsContainer, Is.False);
    }

    [Test]
    public void IsContainerIfFormatDerivedFromNodeContainer()
    {
        var format = new DerivedContainerFormat();
        using var node = new Node("MyTest", format);
        Assert.IsTrue(node.IsContainer);
    }

    [Test]
    public void GetFormatAs()
    {
        var format = new StringFormat("3");
        using var node = new Node("NodeTest", format);
        Assert.AreSame(node.Format, node.GetFormatAs<StringFormat>());
    }

    [Test]
    public void GetFormatAsReturnNullIfNoCastingPossible()
    {
        var format = new StringFormat("3");
        using var node = new Node("NodeTest", format);
        Assert.IsNull(node.GetFormatAs<IntFormat>());
    }

    [Test]
    public void GetFormatAsAfterDisposeThrowsException()
    {
        using var format = new StringFormat("3");
        var node = new Node("NodeTest", format);
        node.Dispose();
        Assert.That(
            node.GetFormatAs<StringFormat>,
            Throws.TypeOf<ObjectDisposedException>());
    }

    [Test]
    public void GetFormatReturnNullIfNoFormat()
    {
        using var node = new Node("test");
        Assert.That(node.GetFormatAs<StringFormat>(), Is.Null);
    }

    [Test]
    public void ChangeFormat()
    {
        var dummyFormat1 = new StringFormat("3");
        var dummyFormat2 = new IntFormat(4);
        using var node = new Node("mytest", dummyFormat1);
        node.ChangeFormat(dummyFormat2);
        Assert.AreNotSame(node.Format, dummyFormat1);
        Assert.AreSame(node.Format, dummyFormat2);
    }

    [Test]
    public void ChangeFormatWithoutPreviousFormat()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest");
        node.ChangeFormat(dummyFormat);
        Assert.AreSame(node.Format, dummyFormat);
    }

    [Test]
    public void ChangeFormatDisposePreviousFormat()
    {
        var dummyFormat1 = new StringFormat("3");
        var dummyFormat2 = new IntFormat(4);
        using var node = new Node("mytest", dummyFormat1);
        node.ChangeFormat(dummyFormat2);
        Assert.IsTrue(dummyFormat1.Disposed);
        Assert.IsFalse(dummyFormat2.Disposed);
    }

    [Test]
    public void ChangeFormatDoesNothingForSameFormat()
    {
        var dummyFormat1 = new StringFormat("3");
        StringFormat dummyFormat2 = dummyFormat1;
        using var node = new Node("mytest", dummyFormat1);
        node.ChangeFormat(dummyFormat2);
        Assert.AreEqual(dummyFormat2, node.Format);
        Assert.IsFalse(dummyFormat1.Disposed);
        Assert.IsFalse(dummyFormat2.Disposed);
    }

    [Test]
    public void ChangeFormatWithoutDisposingPreviousFormat()
    {
        var dummyFormat1 = new StringFormat("3");
        var dummyFormat2 = new IntFormat(4);
        using var node = new Node("mytest", dummyFormat1);
        node.ChangeFormat(dummyFormat2, false);
        Assert.IsFalse(dummyFormat1.Disposed);
        Assert.IsFalse(dummyFormat2.Disposed);
    }

    [Test]
    public void ChangeFormatToNull()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);
        node.ChangeFormat(null);
        Assert.IsNull(node.Format);
    }

    [Test]
    public void SetContainerFormatAddChildren()
    {
        using var node = new Node("MyTest");
        var format = new NodeContainerFormat();
        format.Root.Add(new Node("Child"));
        node.ChangeFormat(format);

        Assert.AreEqual(1, node.Children.Count);
        Assert.AreSame(format.Root.Children[0], node.Children[0]);
        Assert.AreEqual("/MyTest/Child", node.Children[0].Path);
        Assert.AreSame(node, node.Children[0].Parent);
    }

    [Test]
    public void SetContainerFormatTransferHandlingToNode()
    {
        using var node = new Node("MyTest");
        var format = new NodeContainerFormat();
        format.Root.Add(new Node("Child"));
        node.ChangeFormat(format);

        // It must be the same parent
        Assert.AreSame(node, format.Root);
        format.Dispose();
        Assert.IsFalse(node.Disposed);
        Assert.IsNotEmpty(node.Children);
        Assert.IsFalse(node.Children[0].Disposed);
    }

    [Test]
    public void SetFromContainerToDifferentCleanChildren()
    {
        using var node = new Node("mytest");
        var format = new NodeContainerFormat();
        format.Root.Add(new Node("Child"));
        node.ChangeFormat(format);
        Assert.IsNotEmpty(node.Children);

        var newFormat = new StringFormat("3");
        node.ChangeFormat(newFormat);
        Assert.IsEmpty(node.Children);
    }

    [Test]
    public void SetFormatThrowExceptionIfDisposed()
    {
        using var node = new Node("MyTest");
        node.Dispose();
        var format = new StringFormat("3");
        _ = Assert.Throws<ObjectDisposedException>(() => node.ChangeFormat(format));
    }

    [Test]
    public void TransformWithGeneric()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        _ = node.TransformWith<StringFormat2IntFormat>();
        Assert.IsInstanceOf<IntFormat>(node.Format);
        Assert.AreNotSame(dummyFormat, node.Format);
        Assert.AreEqual(3, node.GetFormatAs<IntFormat>().Value);
    }

    [Test]
    public void TransformWithType()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        _ = node.TransformWith(typeof(StringFormat2IntFormat));
        Assert.IsInstanceOf<IntFormat>(node.Format);
        Assert.AreNotSame(dummyFormat, node.Format);
        Assert.AreEqual(3, node.GetFormatAs<IntFormat>().Value);
    }

    [Test]
    public void TransformWithTypeAndParameters()
    {
        // Converter with single non-parameterless constructor
        using var node1 = new Node("node", new StringFormat("C0"));
        using var expected1 = new IntFormat(0xC0 - 5);

        _ = node1.TransformWith(
            typeof(StringFormatConverterWithConstructor),
            NumberStyles.HexNumber,
            -5);
        _ = node1.Format.Should().BeOfType<IntFormat>().And.BeEquivalentTo(expected1);

        // Convert with multiple constructors
        // .. constructor 1: as hexadecimal no delta
        using var node2 = new Node("node", new StringFormat("C0"));
        using var expected2 = new IntFormat(0xC0);

        _ = node2.TransformWith(
            typeof(StringFormatConverterWithSeveralConstructors),
            NumberStyles.HexNumber);
        _ = node2.Format.Should().BeOfType<IntFormat>().And.BeEquivalentTo(expected2);

        // .. constructor 2: with parenthesis (negative) and delta 5
        using var node3 = new Node("node", new StringFormat("(192)"));
        using var expected3 = new IntFormat(-192 + 5);

        _ = node3.TransformWith(
            typeof(StringFormatConverterWithSeveralConstructors),
            5);
        _ = node3.Format.Should().BeOfType<IntFormat>().And.BeEquivalentTo(expected3);
    }

    [Test]
    public void TransformWithTypeAndMissingParametersThrows()
    {
        using var node = new Node("node", new StringFormat("C0"));

        // No parameterless constructor
        Assert.That(
            () => node.TransformWith(typeof(StringFormatConverterWithConstructor)),
            Throws.Exception);

        // Constructor should have two arguments - passing one
        Assert.That(
            () => node.TransformWith(typeof(StringFormatConverterWithConstructor), NumberStyles.HexNumber),
            Throws.Exception);

        // Invalid types
        Assert.That(
            () => node.TransformWith(typeof(StringFormatConverterWithConstructor), 5, 5),
            Throws.Exception);
    }

    [Test]
    public void TransformWithTypeThrowsIfNullType()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        Type myType = null;
        Assert.That(
            () => node.TransformWith(myType),
            Throws.ArgumentNullException);
    }

    [Test]
    public void TransformWithInstance()
    {
        var converter = new StringFormat2IntFormat();
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        _ = node.TransformWith(converter);
        Assert.IsInstanceOf<IntFormat>(node.Format);
        Assert.AreNotSame(dummyFormat, node.Format);
        Assert.AreEqual(3, node.GetFormatAs<IntFormat>().Value);
    }

    [Test]
    public void TransformWithInstanceThrowsIfConverterNull()
    {
        StringFormat2IntFormat converter = null;
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);
        Assert.That(
            () => node.TransformWith(converter),
            Throws.ArgumentNullException);
    }

    [Test]
    public void TransformWithMethodsReturnsNode()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        Assert.Multiple(() => {
            Assert.That(node.TransformWith<StringFormat2IntFormat>(), Is.SameAs(node));
            Assert.That(node.TransformWith(typeof(IntFormat2StringFormat)), Is.SameAs(node));
            Assert.That(node.TransformWith(new StringFormat2IntFormat()), Is.SameAs(node));
        });
    }

    [Test]
    public void TransformWithMethodsPrivateTypeThrowsException()
    {
        using var node = new Node("mytest", new NodeContainerFormat());

        Assert.Multiple(() => {
            Assert.That(() => node.TransformWith<PrivateConverter>(), Throws.Exception);
            Assert.That(() => node.TransformWith(typeof(PrivateConverter)), Throws.Exception);
            Assert.That(() => node.TransformWith(new PrivateConverter()), Throws.Exception);
        });
    }

    [Test]
    public void TransformWithMethodsThrowsIfConverterDoesNotReturnIFormat()
    {
        var dummy = new StringFormat("3");
        using var node = new Node("mytest", dummy);

        Assert.Multiple(() => {
            Assert.That(
                () => node.TransformWith<StringFormat2NoFormat>(),
                Throws.InvalidOperationException);

            Assert.That(
                () => node.TransformWith(typeof(StringFormat2NoFormat)),
                Throws.InvalidOperationException);

            Assert.That(
                () => node.TransformWith(new StringFormat2NoFormat()),
                Throws.InvalidOperationException);
        });
    }

    [Test]
    public void TransformWithMethodsDoesNotThrowIfReturnsNull()
    {
        var dummy = new NullSource();
        using var node = new Node("mytest", dummy);

        Assert.Multiple(() => {
            Assert.That(
                () => node.TransformWith<NullConverter>(),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);

            node.ChangeFormat(dummy);
            Assert.That(
                () => node.TransformWith(typeof(NullConverter)),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);

            node.ChangeFormat(dummy);
            Assert.That(
                () => node.TransformWith(new NullConverter()),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);
        });
    }

    [Test]
    public void TransformWithMethodsThrowsIfNoConverterImplementation()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        const string msg = "Converter doesn't implement IConverter<,>";
        Assert.Multiple(() => {
            Assert.That(
                () => node.TransformWith<ConverterWithoutGenericInterface>(),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            Assert.That(
                () => node.TransformWith(typeof(ConverterWithoutGenericInterface)),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            Assert.That(
                () => node.TransformWith(new ConverterWithoutGenericInterface()),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
        });
    }

    [Test]
    public void TransformWithMethodsThrowsExceptionIfUsingNonCompatibleConverter()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        Assert.Multiple(() => {
            Assert.That(
                () => node.TransformWith<IntFormat2StringFormat>(),
                Throws.InstanceOf<InvalidOperationException>());
            Assert.That(
                () => node.TransformWith(typeof(IntFormat2StringFormat)),
                Throws.InstanceOf<InvalidOperationException>());
            Assert.That(
                () => node.TransformWith(new IntFormat2StringFormat()),
                Throws.InstanceOf<InvalidOperationException>());
        });
    }

    [Test]
    public void TransformWithMethodsDisposePreviousFormat()
    {
        var dummyFormat = new StringFormat("3");
        using var node = new Node("mytest", dummyFormat);

        Assert.Multiple(() => {
            _ = node.TransformWith<StringFormat2IntFormat>();
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.GetFormatAs<IntFormat>().Disposed);

            dummyFormat = new StringFormat("3");
            node.ChangeFormat(dummyFormat);
            _ = node.TransformWith(typeof(StringFormat2IntFormat));
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.GetFormatAs<IntFormat>().Disposed);

            dummyFormat = new StringFormat("3");
            node.ChangeFormat(dummyFormat);
            _ = node.TransformWith(new StringFormat2IntFormat());
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.GetFormatAs<IntFormat>().Disposed);
        });
    }

    [Test]
    public void TransformWithMethodsAfterNodeDisposeThrowsException()
    {
        using var dummyFormat = new StringFormat("3");
        var node = new Node("mytest", dummyFormat);
        node.Dispose();

        Assert.That(
            () => node.TransformWith<StringFormat2IntFormat>(),
            Throws.TypeOf<ObjectDisposedException>());
        Assert.That(
            () => node.TransformWith(typeof(StringFormat2IntFormat)),
            Throws.TypeOf<ObjectDisposedException>());

        var converter = new StringFormat2IntFormat();
        Assert.That(
            () => node.TransformWith(converter),
            Throws.TypeOf<ObjectDisposedException>());
    }

    [Test]
    public void TransformWithMethodsWithoutFormatThrowException()
    {
        const string msg = "Cannot transform a node without format";
        using var node = new Node("mytest");

        Assert.That(
            () => node.TransformWith<StringFormat2IntFormat>(),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
        Assert.That(
            () => node.TransformWith(typeof(StringFormat2IntFormat)),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));

        var converter = new StringFormat2IntFormat();
        Assert.That(
            () => node.TransformWith(converter),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }

    [Test]
    public void DisposeDoesDisposeFormat()
    {
        using var dummyFormat = new StringFormat("3");
        var node = new Node("mytest", dummyFormat);
        Assert.IsFalse(node.Disposed);
        Assert.IsFalse(dummyFormat.Disposed);
        node.Dispose();
        Assert.IsTrue(node.Disposed);
        Assert.IsTrue(dummyFormat.Disposed);
    }

    [Test]
    public void DisposeTwiceDoesNotThrow()
    {
        using var node = new Node("mytest", new StringFormat("3"));
        node.Dispose();
        Assert.DoesNotThrow(node.Dispose);
    }

    [Test]
    public void CloneNullNodeThrowsException()
    {
        _ = Assert.Throws<ArgumentNullException>(() => _ = new Node((Node)null));
    }

    [Test]
    public void NotCloneableFormatThrowsException()
    {
        using var node = new Node("test", new StringFormat("3"));
        _ = Assert.Throws<InvalidOperationException>(() => _ = new Node(node));
    }

    [Test]
    public void CloneNodeHasTags()
    {
        using Node node = NodeFactory.FromMemory("test");
        node.Tags["TestTag"] = 23;

        using var clone = new Node(node);

        Assert.AreNotSame(node, clone);
        Assert.AreEqual(1, clone.Tags.Count);
        Assert.AreEqual(23, clone.Tags["TestTag"]);
    }

    [Test]
    public void CloneNullFormatNode()
    {
        using var node = new Node("test");
        using var clone = new Node(node);

        Assert.AreNotSame(node, clone);
        Assert.IsNull(clone.Format);
    }

    private sealed class DerivedContainerFormat : NodeContainerFormat
    {
    }

    private sealed class PrivateConverter : IConverter<NodeContainerFormat, DerivedContainerFormat>
    {
        public DerivedContainerFormat Convert(NodeContainerFormat source)
        {
            return new DerivedContainerFormat();
        }
    }
}
