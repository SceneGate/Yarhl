namespace Yarhl.UnitTests.FileFormat;
using System;
using NUnit.Framework;
using Yarhl.FileFormat;

public partial class ConvertFormatTests
{
    [Test]
    public void ConvertToBaseWithDerivedConverter()
    {
        // The converter will generate a derived type and cast-down to base.
        BaseFormat val = null;
        Assert.DoesNotThrow(() => val = ConvertFormat.With(typeof(ConvertDerivedFormat), (ushort)3) as BaseFormat);
        Assert.IsInstanceOf<DerivedFormat>(val);
        Assert.AreEqual(3, val.X);

        Assert.DoesNotThrow(() => val = ConvertFormat.With(typeof(ConvertBaseFormat), 3) as BaseFormat);
        Assert.IsInstanceOf<BaseFormat>(val);
        Assert.AreEqual(5, val.X);
    }

    [Test]
    public void ConvertFromBaseWithDerivedConverterThrows()
    {
        // We cannot do the inverse, from base type use the derived converter
        BaseFormat val = new BaseFormat { X = 3 };
        Assert.Throws<InvalidOperationException>(
            () => ConvertFormat.With(typeof(ConvertDerivedFormat), val));
    }

    [Test]
    public void ConvertToDerivedWithDerivedConverter()
    {
        // Just to validate converter, derived with derived converter
        DerivedFormat derived = null;
        Assert.DoesNotThrow(() => derived = ConvertFormat.With(typeof(ConvertDerivedFormat), (ushort)4) as DerivedFormat);
        Assert.AreEqual(5, derived.Y);
        Assert.AreEqual(4, derived.X);

        ushort conv = 0;
        Assert.DoesNotThrow(() => conv = (ushort)ConvertFormat.With(typeof(ConvertDerivedFormat), derived));
        Assert.AreEqual(5, conv);
    }

    [Test]
    public void ConvertFromDerivedWithBaseConverter()
    {
        var format = new DerivedFormat { Y = 11, X = 10 };
        int conv = 0;

        Assert.DoesNotThrow(() => conv = (int)ConvertFormat.With(typeof(ConvertBaseFormat), format));
        Assert.AreEqual(15, conv);
    }

    [Test]
    public void ConvertFromImplementationWithInterfaceFormatConverter()
    {
        var format = new InterfaceImpl { Z = 14 };
        int conv = 0;
        Assert.DoesNotThrow(() => conv = (int)ConvertFormat.With(typeof(ConverterInterface), format));
        Assert.AreEqual(14, conv);
    }
}
