namespace Yarhl.UnitTests.FileFormat;

using System;
using System.Globalization;
using System.Reflection;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.FileFormat;

[TestFixture]
public partial class ConvertFormatTests
{
    [Test]
    public void ConvertWithType()
    {
        using var source = new StringFormat("3");
        using var expected = new IntFormat(3);

        object actual = ConvertFormat.With(typeof(StringFormat2IntFormat), source);

        _ = actual.Should()
            .BeOfType<IntFormat>().And
            .BeEquivalentTo(expected);
    }

    [Test]
    public void ConvertWithTypeUsingCustomConstructor()
    {
        using var source = new StringFormat("C0");
        using var expected = new IntFormat(192 - 5);

        // Parse as hexadecimal (0xC0 == 192) with delta "-5"
        object actual = ConvertFormat.With(
            typeof(StringFormatConverterWithConstructor),
            source,
            NumberStyles.HexNumber,
            -5);

        _ = actual.Should()
            .BeOfType<IntFormat>().And
            .BeEquivalentTo(expected);
    }

    [Test]
    public void ConvertWithTypeUsingSeveralConstructors()
    {
        using var source1 = new StringFormat("C0");
        using var expected1 = new IntFormat(192);

        using var source2 = new StringFormat("(192)");
        using var expected2 = new IntFormat(-192 + 5);

        // as hexadecimal with delta 0
        object actual1 = ConvertFormat.With(
            typeof(StringFormatConverterWithSeveralConstructors),
            source1,
            NumberStyles.HexNumber);

        // With parenthesis (means negative) and delta 5
        object actual2 = ConvertFormat.With(
            typeof(StringFormatConverterWithSeveralConstructors),
            source2,
            5);

        _ = actual1.Should()
            .BeOfType<IntFormat>().And
            .BeEquivalentTo(expected1);

        _ = actual2.Should()
            .BeOfType<IntFormat>().And
            .BeEquivalentTo(expected2);
    }

    [Test]
    public void ConvertWithTypeMissingParametersThrows()
    {
        using var format = new StringFormat("C0");

        // No parameterless constructor
        Assert.That(
            () => ConvertFormat.With(
                typeof(StringFormatConverterWithConstructor),
                format),
            Throws.Exception);

        // Constructor should have two arguments -- passing one
        Assert.That(
            () => ConvertFormat.With(
                typeof(StringFormatConverterWithConstructor),
                format,
                NumberStyles.HexNumber),
            Throws.Exception);

        // Invalid types
        Assert.That(
            () => ConvertFormat.With(
                typeof(StringFormatConverterWithConstructor),
                format,
                5,
                5),
            Throws.Exception);
    }

    [Test]
    public void ConvertWithTypeUsingPrivateConverterThrows()
    {
        Assert.That(
            () => ConvertFormat.With(typeof(PrivateConverter), 5L),
            Throws.Exception);
    }

    [Test]
    public void ConvertWithTypeThrowsIfTypeIsNull()
    {
        using var format = new StringFormat("3");
        Type type = null;
        Assert.That(
            () => ConvertFormat.With(type, format),
            Throws.ArgumentNullException);
    }

    [Test]
    public void ConvertWithTypeCouldWorkWithNullFormats()
    {
        // source and output are null
        Assert.That(ConvertFormat.With(typeof(NullConverter), null), Is.Null);
    }

    [Test]
    public void ConvertWithTypeThrowsIfConstructorFails()
    {
        using var test = new StringFormat { Value = "3" };
        _ = Assert.Throws<TargetInvocationException>(() =>
            ConvertFormat.With(typeof(ConverterWithConstructorException), test));

        // Just for coverage
        var converter = new ConverterWithConstructorException("2");
        Assert.That(
            converter.Convert(new StringFormat("3")),
            Is.EqualTo(3));
    }

    [Test]
    public void ConvertWithTypeThrowsIfNoImplementIConverter()
    {
        using var format = new StringFormat("3");
        Assert.That(
            () => ConvertFormat.With(typeof(DateTime), format),
            Throws.InvalidOperationException.With.Message.EqualTo(
                "Converter doesn't implement IConverter<,>"));
    }

    [Test]
    public void ConvertWithTypeThrowsExceptionIfInvalidConverter()
    {
        using var format = new StringFormat("3");
        const string msg = "Converter cannot convert the type: Yarhl.UnitTests.FileFormat.StringFormat";

        Assert.That(
            () => ConvertFormat.With(typeof(IntFormat2StringFormat), format),
            Throws.InvalidOperationException.With.Message.EqualTo(msg));
    }

    private sealed class PrivateConverter : IConverter<long, byte>
    {
        public byte Convert(long source)
        {
            return (byte)source;
        }
    }
}
