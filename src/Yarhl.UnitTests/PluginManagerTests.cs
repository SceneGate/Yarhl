//
// PluginManagerTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Yarhl.UnitTests
{
    using System;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture, SingleThreaded]
    public class PluginManagerTests
    {
        public interface IExistsInterface
        {
        }

        public interface IGenericExport<T>
        {
        }

        [Test]
        public void InstanceInitializePluginManager()
        {
            Assert.IsNotNull(PluginManager.Instance);
        }

        [Test]
        public void FindExtensionByGenericType()
        {
            var extensions = PluginManager.Instance
                .FindExtensions<IExistsInterface>()
                .ToList();
            Assert.IsInstanceOf(typeof(IExistsInterface), extensions.Single());
        }

        [Test]
        public void FindSpecificExtensionByGenericTypeReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindExtensions<ExistsClass>();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindExtensionByType()
        {
            var extensions = PluginManager.Instance
                .FindExtensions(typeof(IExistsInterface));
            Assert.IsInstanceOf(typeof(IExistsInterface), extensions.Single());
        }

        [Test]
        public void FindExtensionByTypeNotRegisteredReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindExtensions(typeof(ExistsClass));
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindExtensionWithNullTypeThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                PluginManager.Instance.FindExtensions(null));
        }

        [Test]
        public void FindGenericExtensions()
        {
            var extensions = PluginManager.Instance
                .FindExtensions(typeof(IGenericExport<int>));
            Assert.IsInstanceOf(typeof(IGenericExport<int>), extensions.Single());
        }

        [Test]
        public void FindGenericExtensionsNotRegisteredReturnsEmpty()
        {
            var extensions = PluginManager.Instance
                .FindExtensions(typeof(IGenericExport<double>));
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindLazyExtensionByGeneric()
        {
            var extensions = PluginManager.Instance
                .FindLazyExtensions<ConstructorWithException>();
            Assert.That(extensions.Count(), Is.EqualTo(1));
            Assert.That(() => extensions.Single().CreateExport(), Throws.Exception);
        }

        [Test]
        public void FindLazyExtensionByType()
        {
            var extensions = PluginManager.Instance
                .FindLazyExtensions(typeof(ConstructorWithException));
            Assert.That(extensions.Count(), Is.EqualTo(1));
            Assert.That(
                extensions.Single().GetType(),
                Is.EqualTo(typeof(ExportFactory<ConstructorWithException>)));
            Assert.That(
                () => ((ExportFactory<ConstructorWithException>)extensions.Single()).CreateExport().Value,
                Throws.Exception);
        }

        [Test]
        public void FindLazyExtensionByTypeWithNullThrowsException()
        {
            Assert.That(
                () => PluginManager.Instance.FindLazyExtensions(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void FindLazyExtensionWithMetadata()
        {
            var formats = PluginManager.Instance
                .FindLazyExtensions<Format, FormatMetadata>()
                .Select(f => f.Metadata.Type);
            Assert.That(formats, Does.Contain(typeof(PluginFormat)));
        }

        [Test]
        public void FindLazyExtesionWithMetadataIsUnique()
        {
            var formats = PluginManager.Instance
                .FindLazyExtensions<Format, FormatMetadata>()
                .Select(f => f.Metadata.Type);
            Assert.That(formats, Is.Unique);
        }

        [Test]
        public void GetFormatsReturnsListWithMetadata()
        {
            var formats = PluginManager.Instance.GetFormats()
                .Select(f => f.Metadata.Type);
            Assert.That(formats, Does.Contain(typeof(PluginFormat)));
        }

        [Test]
        public void GetConvertersWithMetadataReturnsListWithMetadata()
        {
            var formats = PluginManager.Instance.GetConverters()
                .Select(f => f.Metadata.Type);
            Assert.That(formats, Does.Contain(typeof(PluginConverter)));
            
            var conv = (PluginConverter)PluginManager.Instance.GetConverters()
                    .Where(f => f.Metadata.Type == typeof(PluginConverter))
                    .Single()
                    .CreateExport().Value;
            Assert.That(conv.Convert(new PluginFormat()), Is.EqualTo(0));
        }

        [Export(typeof(IExistsInterface))]
        public class ExistsClass : IExistsInterface
        {
        }

        [Export(typeof(IGenericExport<int>))]
        public class GenericExport : IGenericExport<int>
        {
        }

        [Export]
        public class ConstructorWithException
        {
            public ConstructorWithException()
            {
                throw new Exception();
            }
        }

        public class PluginFormat : Format
        {
            public int Value => 0;
        }

        public class PluginConverter : IConverter<PluginFormat, int>
        {
            public int Convert(PluginFormat src)
            {
                return src.Value;
            }
        }
    }
}
