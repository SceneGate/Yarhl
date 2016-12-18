//
// NodeFactoryTests.cs
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
namespace Libgame.UnitTests.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using Mono.Addins;
    using NUnit.Framework;
    using Libgame.FileFormat;
    using Libgame.FileSystem;

    [TestFixture]
    public class NodeFactoryTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(".addins");
                AddinManager.Registry.Update();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (AddinManager.IsInitialized)
                AddinManager.Shutdown();
        }

        [Test]
        public void CreateContainerWithName()
        {
            Node container = NodeFactory.CreateContainer("MyTest");
            Assert.AreEqual("MyTest", container.Name);
            Assert.IsInstanceOf<NodeContainerFormat>(container.Format);
            Assert.IsTrue(container.IsContainer);
        }

        [Test]
        public void CreateFromFile()
        {
            string tempFile = Path.GetTempFileName();
            Node tempNode = NodeFactory.FromFile(tempFile);

            Assert.AreEqual(Path.GetFileName(tempFile), tempNode.Name);
            Assert.IsFalse(tempNode.IsContainer);
            Assert.IsInstanceOf<BinaryFormat>(tempNode.Format);
            Assert.IsEmpty(tempNode.Children);

            tempNode.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void CreateFromFileDoesNotExist()
        {
            Assert.Throws<FileNotFoundException>(() =>
                NodeFactory.FromFile("ThisPathDoesNotExist"));
        }

        [Test]
        public void CreateFromNullFile()
        {
            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromFile(null));
        }

        [Test]
        public void CreateFromFileAndNullName()
        {
            string tempFile = Path.GetTempFileName();
            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromFile(tempFile, null));
            File.Delete(tempFile);
        }

        [Test]
        public void CreateFromFileAndName()
        {
            string tempFile = Path.GetTempFileName();
            Node tempNode = NodeFactory.FromFile(tempFile, "MyTempNode");

            Assert.AreEqual("MyTempNode", tempNode.Name);
            Assert.IsFalse(tempNode.IsContainer);
            Assert.IsInstanceOf<BinaryFormat>(tempNode.Format);
            Assert.IsEmpty(tempNode.Children);

            tempNode.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void CreateFromDirectoryWithFiles()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile1).Dispose();
            string tempFile2 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile2).Dispose();
            string tempFile3 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile3).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir);
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.AreEqual(3, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile2)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile3)));

            node.Children[0].Dispose();
            node.Children[1].Dispose();
            node.Children[2].Dispose();
            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryAndNameAndEmpty()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            Node node = NodeFactory.FromDirectory(tempDir, "MyTempNode");
            Assert.AreEqual("MyTempNode", node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.IsEmpty(node.Children);

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryDoesNotExist()
        {
            Assert.Throws<DirectoryNotFoundException>(() =>
                NodeFactory.FromDirectory("ThisPathDoesNotExist"));
        }

        [Test]
        public void CreateFromNullDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(null));
        }

        [Test]
        public void CreateFromDirectoryAndNullName()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.FromDirectory(tempDir, null));
            
            Directory.Delete(tempDir, true);
        }
    }
}
