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
namespace Libgame.UnitTests
{
    using System.IO;
    using System.Linq;
    using FileFormat;
    using Libgame.FileFormat;
    using Mono.Addins;
    using NUnit.Framework;

    [TestFixture]
    public class PluginManagerTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            PluginManager.Shutdown();  // Clean any previous state
        }

        [TearDown]
        public void TearDown()
        {
            PluginManager.Shutdown();
        }

        [Test]
        public void InstanceInitializeAddinManager()
        {
            Assert.IsFalse(AddinManager.IsInitialized);
            Assert.IsNotNull(PluginManager.Instance);
            Assert.IsTrue(AddinManager.IsInitialized);
        }

        [Test]
        public void AddinFolderIsHiddenAndExists()
        {
            Assert.IsNotNull(PluginManager.Instance);
            DirectoryInfo dirInfo = new DirectoryInfo(".addins");
            Assert.IsTrue(dirInfo.Exists);
            Assert.IsTrue(dirInfo.Attributes.HasFlag(FileAttributes.Hidden));
        }

        [Test]
        public void ShutdownTurnOffAddinManager()
        {
            Assert.IsNotNull(PluginManager.Instance);
            Assert.IsTrue(AddinManager.IsInitialized);
            PluginManager.Shutdown();
            Assert.IsFalse(AddinManager.IsInitialized);
        }

        [Test]
        public void FindExtensionByGenericType()
        {
            var extensions = PluginManager.Instance
                .FindExtensions<Format>()
                .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(StringFormatTest), extensions);
        }

        [Test]
        public void FindSpecificExtensionByGenericTypeFails()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions<StringFormatTest>();
            Assert.IsEmpty(extensions);
        }

        [Test]
        public void FindExtension()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions(typeof(Format))
                                          .ToList();
            Assert.IsNotEmpty(extensions);
            Assert.Contains(typeof(StringFormatTest), extensions);
        }

        [Test]
        public void FindSpecificExtensionFails()
        {
            var extensions = PluginManager.Instance
                                          .FindExtensions(typeof(StringFormatTest));
            Assert.IsEmpty(extensions);
        }
    }
}