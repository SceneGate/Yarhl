namespace Yarhl.UnitTests.FileFormat;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Yarhl.FileFormat;

public partial class ConvertFormatTests
{
    [Test]
    public void ConvertWithGeneric()
    {
        using var format = new StringFormatTest("3");
        Assert.That(
            ConvertFormat.With<FormatTestDuplicatedConverter1>(format),
            Is.EqualTo(3));
    }

    [Test]
    public void ConvertWithGenericInitializerInterface()
    {
        using var format = new StringFormatTest("C0");
        Assert.That(
            (ConvertFormat.With<ConverterWithInitializerInterface, NumberStyles>(
                NumberStyles.HexNumber,
                format) as IntFormatTest)?.Value,
            Is.EqualTo(192));
    }

    [Test]
    public void ConvertWithInstance()
    {
        using var format = new StringFormatTest("3");
        var converter = new StringFormatTest2IntFormatTestConverter();
        Assert.That(
            (ConvertFormat.With(converter, format) as IntFormatTest)?.Value,
            Is.EqualTo(3));
    }

    [Test]
    public void ConvertWithInstanceThrowsIfConverterIsNull()
    {
        using var format = new StringFormatTest("3");
        StringFormatTest2IntFormatTestConverter converter = null;
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertWithGenericAndInstanceThrowsExceptionIfNoImplementIConverter()
    {
        using var format = new StringFormatTest("3");
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
        StringFormatTest format = null;

        Assert.That(
            () => ConvertFormat.With<StringFormatTest2IntFormatTestConverter>(format),
            Throws.ArgumentNullException);
        Assert.That(
            () => ConvertFormat.With<ConverterWithInitializerInterface, NumberStyles>(
                NumberStyles.Integer,
                format),
            Throws.ArgumentNullException);

        var converter = new StringFormatTest2IntFormatTestConverter();
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertWithGenericOrInstanceThrowsExceptionIfInvalidConverter()
    {
        using var format = new StringFormatTest("3");
        string msg = "Converter cannot convert the type: Yarhl.UnitTests.FileFormat.StringFormatTest";

        Assert.That(
            () => ConvertFormat.With<SingleOuterConverterExample>(format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
        Assert.That(
            () => ConvertFormat.With<ConverterAndOtherInterface, int>(4, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));

        var converter = new SingleOuterConverterExample();
        Assert.That(
            () => ConvertFormat.With(converter, format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }
}
