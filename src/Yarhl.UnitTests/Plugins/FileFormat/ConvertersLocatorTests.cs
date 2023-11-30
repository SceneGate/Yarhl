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

using System;
using System.Linq;
using System.Runtime.Loader;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Plugins;
using Yarhl.Plugins.FileFormat;

[TestFixture]
public class ConvertersLocatorTests
{
    [Test]
    public void InstanceIsSingleton()
    {
        ConverterLocator instance1 = ConverterLocator.Default;
        ConverterLocator instance2 = ConverterLocator.Default;

        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void InstanceIsInitialized()
    {
        ConverterLocator instance = ConverterLocator.Default;

        Assert.That(instance.Formats, Is.Not.Null);
        Assert.That(instance.Converters, Is.Not.Null);
    }

    [Test]
    public void InstancePerformsAssemblyScanningOnInitialization()
    {
        // At least the formats defined in this assembly for testing should be there.
        Assert.That(ConverterLocator.Default.Formats, Is.Not.Empty);

        Assert.That(new ConverterLocator(TypeLocator.Default).Formats, Is.Not.Null);
    }

    [Test]
    public void InitializeWithCustomLoadContextProvidesIsolation()
    {
        var loadContext = new AssemblyLoadContext(nameof(InitializeWithCustomLoadContextProvidesIsolation));
        TypeLocator isolatedLocator = new TypeLocator(loadContext);
        ConverterLocator converterLocator = new ConverterLocator(isolatedLocator);

        Assert.That(converterLocator.Formats, Is.Empty);
        Assert.That(converterLocator.Converters, Is.Empty);
    }

    [Test]
    public void LocateFormatsWithTypeInfo()
    {
        TypeImplementationInfo myFormat = ConverterLocator.Default.Formats
            .FirstOrDefault(i => i.Type == typeof(DerivedSourceFormat));

        Assert.That(myFormat, Is.Not.Null);
        Assert.That(myFormat.Name, Is.EqualTo(typeof(DerivedSourceFormat).FullName));
    }

    [Test]
    public void FormatsAreNotDuplicated()
    {
        TypeImplementationInfo[] formats = ConverterLocator.Default.Formats
            .Where(f => f.Type == typeof(MySourceFormat))
            .ToArray();

        Assert.That(formats, Has.Length.EqualTo(1));
    }

    [Test]
    public void LocateFormatsFindYarhlBaseFormats()
    {
        Assert.That(
            ConverterLocator.Default.Formats.Select(f => f.Type),
            Does.Contain(typeof(BinaryFormat)));

        Assert.That(
            ConverterLocator.Default.Formats.Select(f => f.Type),
            Does.Contain(typeof(NodeContainerFormat)));
    }

    [Test]
    public void LocateConvertersWithTypeInfo()
    {
        ConverterTypeInfo result = ConverterLocator.Default.Converters
            .FirstOrDefault(i => i.Type == typeof(MyConverter));

        Assert.That(result, Is.Not.Null);
        Assert.That(result.GenericBaseType, Is.EqualTo(typeof(IConverter<MySourceFormat, MyDestFormat>)));
        Assert.That(result.Name, Is.EqualTo(typeof(MyConverter).FullName));
        Assert.That(result.SourceType, Is.EqualTo(typeof(MySourceFormat)));
        Assert.That(result.DestinationType, Is.EqualTo(typeof(MyDestFormat)));
    }

    [Test]
    public void ConvertersAreNotDuplicated()
    {
        ConverterTypeInfo[] results = ConverterLocator.Default.Converters
            .Where(f => f.Type == typeof(MyConverter))
            .ToArray();

        Assert.That(results, Has.Length.EqualTo(1));
    }

    [Test]
    public void ScanAssembliesDoesNotDuplicateFindings()
    {
        ConverterLocator.Default.ScanAssemblies();

        FormatsAreNotDuplicated();
        ConvertersAreNotDuplicated();
    }

    [Test]
    public void LocateConverterWithParameters()
    {
        ConverterTypeInfo[] results = ConverterLocator.Default.Converters
            .Where(f => f.Type == typeof(MyConverterParametrized))
            .ToArray();

        Assert.That(results.Length, Is.EqualTo(1));
    }

    [Test]
    public void LocateSingleInnerConverter()
    {
        ConverterTypeInfo converter = ConverterLocator.Default.Converters
                .FirstOrDefault(c => c.Type == typeof(SingleOuterConverter.SingleInnerConverter));

        Assert.That(converter, Is.Not.Null);
    }

    [Test]
    public void LocateSingleOuterConverter()
    {
        ConverterTypeInfo converter = ConverterLocator.Default.Converters
                .FirstOrDefault(c => c.Type == typeof(SingleOuterConverter));

        Assert.That(converter, Is.Not.Null);
    }

    [Test]
    public void LocateTwoConvertersInSameClass()
    {
        ConverterTypeInfo[] converters = ConverterLocator.Default.Converters
            .Where(c => c.Type == typeof(TwoConverters))
            .ToArray();

        Assert.That(converters.Length, Is.EqualTo(2));
        Assert.That(
            Array.Exists(
                converters,
                c => c.InterfaceImplemented == typeof(IConverter<MySourceFormat, MyDestFormat>)),
            Is.True);
        Assert.That(
            Array.Exists(
                converters,
                c => c.SourceType == typeof(MySourceFormat) && c.DestinationType == typeof(MyDestFormat)),
            Is.True);

        Assert.That(
            Array.Exists(
                converters,
                c => c.InterfaceImplemented == typeof(IConverter<MyDestFormat, MySourceFormat>)),
            Is.True);
        Assert.That(
            Array.Exists(
                converters,
                c => c.SourceType == typeof(MyDestFormat) && c.DestinationType == typeof(MySourceFormat)),
            Is.True);
    }

    [Test]
    public void LocateDerivedConverter()
    {
        ConverterTypeInfo[] converters = ConverterLocator.Default.Converters
            .Where(c => c.Type == typeof(DerivedConverter))
            .ToArray();

        Assert.That(converters.Length, Is.EqualTo(1));
    }

    [Test]
    public void LocateConvertsWithOtherInterfaces()
    {
        ConverterTypeInfo[] converters = ConverterLocator.Default.Converters
            .Where(c => c.Type == typeof(ConverterAndOtherInterface))
            .ToArray();

        Assert.That(converters.Length, Is.EqualTo(1));
    }
}
