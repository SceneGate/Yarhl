namespace Yarhl.UnitTests.FileFormat;

using System.Globalization;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.FileFormat;

[TestFixture]
public class FormatExtensionsTests
{
    [Test]
    public void ConvertWithConvertsFormat()
    {
        using var source = new StringFormat("3");
        using var expected = new IntFormat(3);

        IntFormat actual = source.ConvertWith(new StringFormat2IntFormat());

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ConvertWithAllowsParameters()
    {
        using var source = new StringFormat("C0");
        using var expected = new IntFormat(0xC0 + 5);

        IntFormat actual = source.ConvertWith(
            new StringFormatConverterWithConstructor(NumberStyles.HexNumber, 5));

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ConvertWithAllowsChaining()
    {
        using var source = new StringFormat("C0");
        using var expected = new StringFormat("197");

        StringFormat actual = source
            .ConvertWith(
                new StringFormatConverterWithConstructor(NumberStyles.HexNumber, 5))
            .ConvertWith(new IntFormat2StringFormat());

        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void ConvertWithDoesNotDisposeFormatByDefault()
    {
        using var source = new IntFormat(3);
        using var converter = new IntFormatDisposableConverter();

        StringFormat actual = source.ConvertWith(converter);

        Assert.Multiple(() => {
            Assert.That(source.Disposed, Is.False);
            Assert.That(actual.Disposed, Is.False);
        });
    }

    [Test]
    public void ConvertWithDoesNotDisposeConverterByDefault()
    {
        using var source = new IntFormat(3);
        using var converter = new IntFormatDisposableConverter();

        _ = source.ConvertWith(converter);

        Assert.That(converter.Disposed, Is.False);
    }

    [Test]
    public void ConvertWithCanDisposeFormat()
    {
        using var source = new IntFormat(3);
        using var converter = new IntFormatDisposableConverter();

        StringFormat actual = source.ConvertWith(converter, disposeInput: true);

        Assert.Multiple(() => {
            Assert.That(source.Disposed, Is.True);
            Assert.That(actual.Disposed, Is.False);
        });
    }

    [Test]
    public void ConvertWithCanDisposeConverter()
    {
        using var source = new IntFormat(3);
        using var converter = new IntFormatDisposableConverter();

        _ = source.ConvertWith(converter, disposeConverter: true);

        Assert.That(converter.Disposed, Is.True);
    }

    [Test]
    public void ConvertWithDoesNotFailTryingToDisposeNonDisposableFormatOrConverter()
    {
        var source = new IntNonDisposableFormat(3);
        var converter = new IntNonDisposableFormatConverter();

        Assert.That(
            () => _ = source.ConvertWith(converter, disposeInput: true, disposeConverter: true),
            Throws.Nothing);
    }
}
