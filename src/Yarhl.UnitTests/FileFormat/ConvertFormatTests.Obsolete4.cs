namespace Yarhl.UnitTests.FileFormat;
using System.Globalization;
using NUnit.Framework;
using Yarhl.FileFormat;

public partial class ConvertFormatTests
{
    [Test]
    public void ConvertWithGeneric()
    {
        using var format = new StringFormat("3");
        Assert.That(
            (ConvertFormat.With<StringFormat2IntFormat>(format) as IntFormat).Value,
            Is.EqualTo(3));
    }

    [Test]
    public void ConvertWithGenericInitializerInterface()
    {
        using var format = new StringFormat("C0");
        Assert.That(
            (ConvertFormat.With<StringFormatConverterWithInitializerInterface, NumberStyles>(
                NumberStyles.HexNumber,
                format) as IntFormat)?.Value,
            Is.EqualTo(192));
    }

    [Test]
    public void ConvertWithInstance()
    {
        using var format = new StringFormat("3");
        var converter = new StringFormat2IntFormat();
        Assert.That(
            (ConvertFormat.With(converter, format) as IntFormat)?.Value,
            Is.EqualTo(3));
    }

    [Test]
    public void ConvertWithInstanceThrowsIfConverterIsNull()
    {
        using var format = new StringFormat("3");
        StringFormat2IntFormat converter = null;
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertWithGenericAndInstanceThrowsExceptionIfNoImplementIConverter()
    {
        using var format = new StringFormat("3");
        string msg = "Converter doesn't implement IConverter<,>";

        Assert.That(
            () => ConvertFormat.With<ConverterWithoutGenericInterface>(format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
        Assert.That(
            () => ConvertFormat.With<ConverterWithoutGenericInterface, int>(4, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));

        var converter = new ConverterWithoutGenericInterface();
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }

    [Test]
    public void ConvertWithGenericOrInstanceThrowsIfFormatIsNull()
    {
        StringFormat format = null;

        Assert.That(
            () => ConvertFormat.With<StringFormat2IntFormat>(format),
            Throws.ArgumentNullException);
        Assert.That(
            () => ConvertFormat.With<StringFormatConverterWithInitializerInterface, NumberStyles>(
                NumberStyles.Integer,
                format),
            Throws.ArgumentNullException);

        var converter = new StringFormat2IntFormat();
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertWithGenericOrInstanceThrowsExceptionIfInvalidConverter()
    {
        using var format = new StringFormat("3");
        const string msg = "Converter cannot convert the type: Yarhl.UnitTests.FileFormat.StringFormat";

        Assert.That(
            () => ConvertFormat.With<IntFormat2StringFormat>(format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
        Assert.That(
            () => ConvertFormat.With<IntFormat2StringFormat, int>(4, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));

        var converter = new IntFormat2StringFormat();
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }
}
