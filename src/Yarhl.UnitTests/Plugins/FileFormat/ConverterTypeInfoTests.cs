// Copyright (c) 2019 SceneGate

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
namespace Yarhl.UnitTests.Plugins.FileFormat;

using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.Plugins;
using Yarhl.Plugins.FileFormat;

[TestFixture]
public class ConverterTypeInfoTests
{
    private readonly ConverterTypeInfo converterInfo = new(
        typeof(MyConverter).FullName!,
        typeof(MyConverter),
        typeof(IConverter<MySourceFormat, MyDestFormat>),
        new[] { typeof(MySourceFormat), typeof(MyDestFormat) });

    [Test]
    public void SourceAndDestinationInfoFromGenericTypes()
    {
        Assert.That(converterInfo.SourceType, Is.EqualTo(typeof(MySourceFormat)));
        Assert.That(converterInfo.DestinationType, Is.EqualTo(typeof(MyDestFormat)));
    }

    [Test]
    public void CreateFromGenericInfo()
    {
        var genericInfo = new GenericTypeImplementationInfo(
            typeof(MyConverter).FullName!,
            typeof(MyConverter),
            typeof(IConverter<MySourceFormat, MyDestFormat>),
            new[] { typeof(MySourceFormat), typeof(MyDestFormat) });

        var info = new ConverterTypeInfo(genericInfo);
        Assert.Multiple(() => {
            Assert.That(info.Name, Is.EqualTo(genericInfo.Name));
            Assert.That(info.Type, Is.EqualTo(genericInfo.Type));
            Assert.That(info.GenericBaseType, Is.EqualTo(genericInfo.GenericBaseType));
            Assert.That(info.GenericTypeParameters, Is.EquivalentTo(genericInfo.GenericTypeParameters));

            Assert.That(info.SourceType, Is.EqualTo(typeof(MySourceFormat)));
            Assert.That(info.DestinationType, Is.EqualTo(typeof(MyDestFormat)));
        });

        var copy = info with { };
        Assert.That(copy, Is.EqualTo(info));
    }

    [Test]
    public void CreateFromGenericInfoThrowsIfMoreThanTwoGenericTypes()
    {
        var generic3Info = new GenericTypeImplementationInfo(
            typeof(MyConverter).FullName!,
            typeof(MyConverter),
            typeof(IConverter<MySourceFormat, MyDestFormat>),
            new[] { typeof(MySourceFormat), typeof(MyDestFormat), typeof(string) });

        var generic1Info = new GenericTypeImplementationInfo(
            typeof(MyConverter).FullName!,
            typeof(MyConverter),
            typeof(IConverter<MySourceFormat, MyDestFormat>),
            new[] { typeof(MySourceFormat) });

        Assert.That(() => new ConverterTypeInfo(generic3Info), Throws.ArgumentException);
        Assert.That(() => new ConverterTypeInfo(generic1Info), Throws.ArgumentException);
    }

    [Test]
    public void CanConvertSourceThrowsExceptionIfNullArgument()
    {
        Assert.That(
            () => converterInfo.CanConvert(null),
            Throws.ArgumentNullException);
    }

    [Test]
    public void CanConvertReturnsTrueForExactType()
    {
        Assert.That(converterInfo.CanConvert(typeof(MySourceFormat)), Is.True);
    }

    [Test]
    public void CanConvertReturnsTrueForDerivedTypes()
    {
        Assert.That(converterInfo.CanConvert(typeof(DerivedSourceFormat)), Is.True);
    }

    [Test]
    public void CanConvertReturnsFalseForBaseTypes()
    {
        Assert.That(converterInfo.CanConvert(typeof(IFormat)), Is.False);
    }

    [Test]
    public void CanConvertReturnsFalseForDifferentTypes()
    {
        Assert.That(converterInfo.CanConvert(typeof(MyDestFormat)), Is.False);
    }

    [Test]
    public void CanConvertReturnsForExactSourceAndDest()
    {
        Assert.That(
            converterInfo.CanConvert(typeof(MySourceFormat), typeof(MyDestFormat)),
            Is.True);
        Assert.That(
            converterInfo.CanConvert(typeof(MyDestFormat), typeof(MySourceFormat)),
            Is.False);
    }

    [Test]
    public void CanConvertReturnsForSourceAndDestDerived()
    {
        // Source is a derived format
        Assert.That(
            converterInfo.CanConvert(typeof(DerivedSourceFormat), typeof(MyDestFormat)),
            Is.True);

        // Destination is a base type
        Assert.That(
            converterInfo.CanConvert(typeof(MySourceFormat), typeof(IFormat)),
            Is.True);

        // Cannot convert base type
        Assert.That(
            converterInfo.CanConvert(typeof(IFormat), typeof(MyDestFormat)),
            Is.False);

        // Cannot convert to derived type
        Assert.That(
            converterInfo.CanConvert(typeof(MySourceFormat), typeof(DerivedDestFormat)),
            Is.False);
    }

    [Test]
    public void CanConvertSourceDestThrowsExceptionIfNullArgument()
    {
        Assert.That(
            () => converterInfo.CanConvert(null, typeof(int)),
            Throws.ArgumentNullException);
        Assert.That(
            () => converterInfo.CanConvert(typeof(int), null),
            Throws.ArgumentNullException);
    }
}
