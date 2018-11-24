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
namespace Yarhl.UnitTests.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;

    [TestFixture]
    public class NodeFactoryTests
    {
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
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Node tempNode = null;

            Assert.IsFalse(File.Exists(tempFile));
            Assert.DoesNotThrow(() => tempNode = NodeFactory.FromFile(tempFile));
            Assert.IsTrue(File.Exists(tempFile));

            tempNode.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void CreateFromNullFile()
        {
            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromFile(null));
        }

        [Test]
        public void CreateFromFileAndNullNameThrowsException()
        {
            string tempFile = Path.GetTempFileName();
            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromFile(tempFile, null));
            File.Delete(tempFile);
        }

        [Test]
        public void CreateFromFileDisposeFormatsIfFails()
        {
            string tempFile = Path.GetTempFileName();
            int numStreams = Yarhl.IO.DataStream.ActiveStreams;
            Assert.Throws<ArgumentNullException>(
                () => NodeFactory.FromFile(tempFile, null));
            Assert.That(Yarhl.IO.DataStream.ActiveStreams, Is.EqualTo(numStreams));
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

            node.Dispose();

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithFinalSlash()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile1).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir + Path.DirectorySeparatorChar);
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.AreEqual(1, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryNull()
        {
            Assert.That(
                () => NodeFactory.FromDirectory(null),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromDirectory(string.Empty),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromDirectoryWithFilesAndFolders()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile).Dispose();
            string tempFolder = Path.Combine(tempDir, Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolder);

            Node node = NodeFactory.FromDirectory(tempDir);
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.AreEqual(1, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithFilesAndFilter()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, "file1.bin");
            File.Create(tempFile1).Dispose();
            string tempFile2 = Path.Combine(tempDir, "arch2.bin");
            File.Create(tempFile2).Dispose();
            string tempFile3 = Path.Combine(tempDir, "file3.txt");
            File.Create(tempFile3).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, "file*");
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile3)));
            node.Dispose();

            node = NodeFactory.FromDirectory(tempDir, "*.bin");
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile2)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryAndNameAndEmpty()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            Node node = NodeFactory.FromDirectory(tempDir, "*", "MyTempNode");
            Assert.AreEqual("MyTempNode", node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.IsEmpty(node.Children);

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithFilesAndNameAndFilter()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, "file1.bin");
            File.Create(tempFile1).Dispose();
            string tempFile2 = Path.Combine(tempDir, "arch2.bin");
            File.Create(tempFile2).Dispose();
            string tempFile3 = Path.Combine(tempDir, "file3.txt");
            File.Create(tempFile3).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, "file*", "MyDir");
            Assert.AreEqual("MyDir", node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile3)));
            node.Dispose();

            node = NodeFactory.FromDirectory(tempDir, "*.bin", "MyDir");
            Assert.AreEqual("MyDir", node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile2)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithSubfolders()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, "file1.bin");
            File.Create(tempFile1).Dispose();
            string tempFolder = Path.Combine(tempDir, "folder");
            Assert.That(Directory.CreateDirectory(tempFolder).Exists, Is.True);
            string tempFile2 = Path.Combine(tempFolder, "file2.txt");
            File.Create(tempFile2).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, "*", "MyDir", true);
            Assert.AreEqual("MyDir", node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.That(node.Children["file1.bin"], Is.Not.Null);
            Assert.That(node.Children["folder"], Is.Not.Null);
            Assert.That(node.Children["folder"].Children.Count, Is.EqualTo(1));
            Assert.That(node.Children["folder"].Children["file2.txt"], Is.Not.Null);
            Assert.That(
                node.Children["folder"].Children["file2.txt"].Path,
                Is.EqualTo("/MyDir/folder/file2.txt"));

            node.Dispose();
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

        [Test]
        public void CreateContainersAndAdd()
        {
            Node root = new Node("root");
            string path = "/parent1/parent2/";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreEqual(1, root.Children.Count);
            Assert.AreEqual("parent1", root.Children[0].Name);
            Assert.AreEqual(1, root.Children[0].Children.Count);
            Assert.AreEqual("parent2", root.Children[0].Children[0].Name);
            Assert.AreEqual(1, root.Children[0].Children[0].Children.Count);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainerWithWindowsPaths()
        {
            Node root = new Node("root");
            string path = @"\parent1\parent2\";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.That(root.Children["parent1"], Is.Not.Null);
            Assert.That(root.Children["parent1"].Children["parent2"], Is.Not.Null);
            Assert.That(
                root.Children["parent1"].Children["parent2"].Children["child"],
                Is.EqualTo(child));
            Assert.That(child.Path, Is.EqualTo("/root/parent1/parent2/child"));
        }

        [Test]
        public void CreateContainersWithEmptyParents()
        {
            Node root = new Node("root");
            string path = "/parent1///parent2/";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenPathDoesNotStartWithSeparator()
        {
            Node root = new Node("root");
            string path = "parent1/parent2";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenPathIsEmpty()
        {
            Node root = new Node("root");
            string path = string.Empty;
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0]);
            Assert.AreEqual("/root/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenSomeContainersExists()
        {
            Node root = new Node("root");
            Node parent1 = new Node("parent1");
            root.Add(parent1);
            string path = "/parent1/parent2/";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(parent1, root.Children[0]);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenAllContainersExists()
        {
            Node root = new Node("root");
            Node parent1 = new Node("parent1");
            Node parent2 = new Node("parent2");
            parent1.Add(parent2);
            root.Add(parent1);
            string path = "/parent1///parent2/";
            Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(parent1, root.Children["parent1"]);
            Assert.AreSame(parent2, root.Children["parent1"].Children["parent2"]);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenRootIsNullThrowsException()
        {
            Node root = null;
            string path = string.Empty;
            Node child = new Node("child");

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateContainersForChildWhenPathIsNullThrowsException()
        {
            Node root = new Node("root");
            string path = null;
            Node child = new Node("child");

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateContainersForChildWhenChildIsNullThrowsException()
        {
            Node root = new Node("root");
            string path = string.Empty;
            Node child = null;

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateFromMemory()
        {
            Node node = NodeFactory.FromMemory("node");
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream, Is.Not.Null);
            Assert.That(node.Stream.BaseStream, Is.TypeOf<MemoryStream>());
        }

        [Test]
        public void CreateFromMemoryWithInvalidNameThrowsException()
        {
            Assert.That(
                () => NodeFactory.FromMemory(null),
                Throws.TypeOf<ArgumentNullException>());
            Assert.That(
                () => NodeFactory.FromMemory(string.Empty),
                Throws.TypeOf<ArgumentNullException>());
        }
    }
}
