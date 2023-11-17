namespace Yarhl.UnitTests.FileFormat;

using System;
using System.Reflection;
using NUnit.Framework;
using Yarhl.FileFormat;

[TestFixture]
public partial class ConvertFormatTests
{
    [Test]
    public void ConvertWithType()
    {
        using var format = new StringFormat("3");
        Assert.That(
            (ConvertFormat.With(typeof(StringFormat2IntFormat), format) as IntFormat).Value,
            Is.EqualTo(3));
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
