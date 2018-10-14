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

        [Export(typeof(IExistsInterface))]
        public class ExistsClass : IExistsInterface
        {
        }

        [Export(typeof(IGenericExport<int>))]
        public class GenericExport : IGenericExport<int>
        {
        }
    }
}
