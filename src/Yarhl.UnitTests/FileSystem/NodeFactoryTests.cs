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
namespace Yarhl.UnitTests.FileSystem
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NUnit.Framework;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class NodeFactoryTests
    {
        [Test]
        public void CreateContainerWithName()
        {
            using Node container = NodeFactory.CreateContainer("MyTest");
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

            // It's lazy initialize, so we need to trigger an operation
            Assert.IsFalse(File.Exists(tempFile));
            tempNode.Stream.WriteByte(0xCA);

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
        public void CreateFromDirectoryWithNameAndFinalSlash()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile1).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir + Path.DirectorySeparatorChar, "*", "MyTempNode");
            Assert.AreEqual("MyTempNode", node.Name);
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

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(string.Empty));

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(null, "*", "name"));

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(string.Empty, "*", "name"));
        }

        [Test]
        public void CreateFromDirectoryAndNullFilter()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(tempDir, (string)null));

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(tempDir, string.Empty));

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(tempDir, (string)null, "name"));

            Assert.Throws<ArgumentNullException>(() => NodeFactory.FromDirectory(tempDir, string.Empty, "name"));

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryAndNullName()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.FromDirectory(tempDir, "*", null));

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryHasValidInfo()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFolder = Path.Combine(tempDir, "folder");
            Assert.That(Directory.CreateDirectory(tempFolder).Exists, Is.True);

            string tempFile1 = Path.Combine(tempFolder, "file1.bin");
            File.Create(tempFile1).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, "*", "MyDir", true);

            Assert.That(node.Tags.Count, Is.EqualTo(1));
            Assert.That(node.Tags.ContainsKey("DirectoryInfo"), Is.True);
            Assert.That(node.Tags.ContainsKey("FileInfo"), Is.False);
            Assert.That(node.Tags["DirectoryInfo"], Is.TypeOf<DirectoryInfo>());

            Assert.That(node.Children["folder"].Tags.Count, Is.EqualTo(1));
            Assert.That(node.Children["folder"].Tags.ContainsKey("DirectoryInfo"), Is.True);
            Assert.That(node.Children["folder"].Tags.ContainsKey("FileInfo"), Is.False);
            Assert.That(node.Children["folder"].Tags["DirectoryInfo"], Is.TypeOf<DirectoryInfo>());

            Assert.That(node.Children["folder"].Children["file1.bin"].Tags.Count, Is.EqualTo(1));
            Assert.That(node.Children["folder"].Children["file1.bin"].Tags.ContainsKey("DirectoryInfo"), Is.False);
            Assert.That(node.Children["folder"].Children["file1.bin"].Tags.ContainsKey("FileInfo"), Is.True);
            Assert.That(node.Children["folder"].Children["file1.bin"].Tags["FileInfo"], Is.TypeOf<FileInfo>());

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateContainersAndAdd()
        {
            using Node root = new Node("root");
            string path = "/parent1/parent2/";
            using Node child = new Node("child");

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
            using Node root = new Node("root");
            string path = @"\parent1\parent2\";
            using Node child = new Node("child");

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
            using Node root = new Node("root");
            string path = "/parent1///parent2/";
            using Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenPathDoesNotStartWithSeparator()
        {
            using Node root = new Node("root");
            string path = "parent1/parent2";
            using Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenPathIsEmpty()
        {
            using Node root = new Node("root");
            string path = string.Empty;
            using Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(child, root.Children[0]);
            Assert.AreEqual("/root/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenSomeContainersExists()
        {
            using Node root = new Node("root");
            using Node parent1 = new Node("parent1");
            root.Add(parent1);
            string path = "/parent1/parent2/";
            using Node child = new Node("child");

            NodeFactory.CreateContainersForChild(root, path, child);
            Assert.AreSame(parent1, root.Children[0]);
            Assert.AreSame(child, root.Children[0].Children[0].Children[0]);
            Assert.AreEqual("/root/parent1/parent2/child", child.Path);
        }

        [Test]
        public void CreateContainersForChildWhenAllContainersExists()
        {
            using Node root = new Node("root");
            using Node parent1 = new Node("parent1");
            using Node parent2 = new Node("parent2");
            parent1.Add(parent2);
            root.Add(parent1);
            string path = "/parent1///parent2/";
            using Node child = new Node("child");

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
            using Node child = new Node("child");

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateContainersForChildWhenPathIsNullThrowsException()
        {
            using Node root = new Node("root");
            string path = null;
            using Node child = new Node("child");

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateContainersForChildWhenChildIsNullThrowsException()
        {
            using Node root = new Node("root");
            string path = string.Empty;
            Node child = null;

            Assert.Throws<ArgumentNullException>(() =>
                NodeFactory.CreateContainersForChild(root, path, child));
        }

        [Test]
        public void CreateFromMemory()
        {
            using Node node = NodeFactory.FromMemory("node");
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("node"));
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream, Is.Not.Null);
            Assert.That(node.Stream.BaseStream, Is.TypeOf<RecyclableMemoryStream>());
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

        [Test]
        public void CreateFromArray()
        {
            byte[] data = new byte[] { 1, 2 };
            using var node = NodeFactory.FromArray("node", data);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("node"));
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream.Length, Is.EqualTo(2));
            Assert.That(node.Stream.ReadByte(), Is.EqualTo(1));
        }

        [Test]
        public void CreateFromArrayGuards()
        {
            byte[] data = new byte[1];
            Assert.That(() => NodeFactory.FromArray(null, data), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromArray(string.Empty, data), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromArray("node", null), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromSubArray()
        {
            byte[] data = new byte[] { 1, 2, 3 };
            using var node = NodeFactory.FromArray("node", data, 1, 1);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("node"));
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream.Position, Is.EqualTo(0));
            Assert.That(node.Stream.Length, Is.EqualTo(1));
            Assert.That(node.Stream.ReadByte(), Is.EqualTo(2));
        }

        [Test]
        public void CreateFromSubArrayGuards()
        {
            byte[] data = new byte[3];
            Assert.That(() => NodeFactory.FromArray(null, data, 1, 2), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromArray(string.Empty, data, 1, 2), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromArray("node", data, -1, 2), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => NodeFactory.FromArray("node", data, 3, 2), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => NodeFactory.FromArray("node", data, 1, 4), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => NodeFactory.FromArray("node", data, 1, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => NodeFactory.FromArray("node", null, 0, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromStream()
        {
            using var stream = new DataStream();
            using var node = NodeFactory.FromStream("node", stream);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("node"));
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream.BaseStream, Is.SameAs(stream.BaseStream));
        }

        [Test]
        public void CreateFromStreamGuards()
        {
            using var stream = new DataStream();
            Assert.That(() => NodeFactory.FromStream(null, stream), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromStream(string.Empty, stream), Throws.ArgumentNullException);
            Assert.That(() => NodeFactory.FromStream("node", null), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromSubstream()
        {
            using DataStream main = new DataStream();
            main.WriteByte(0x00);
            main.WriteByte(0x01);
            main.WriteByte(0x02);

            Node node = NodeFactory.FromSubstream("node", main, 0x01, 0x02);
            Assert.That(node, Is.Not.Null);
            Assert.That(node.Name, Is.EqualTo("node"));
            Assert.That(node.Format, Is.TypeOf<BinaryFormat>());
            Assert.That(node.Stream.BaseStream, Is.SameAs(main.BaseStream));
            Assert.That(node.Stream.Offset, Is.EqualTo(1));
            Assert.That(node.Stream.Length, Is.EqualTo(2));
        }

        [Test]
        public void CreateFromSubstreamOfDataStreamDoesNotTransferOwnership()
        {
            DataStream stream = new DataStream();
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);

            Node node = NodeFactory.FromSubstream("node", stream, 1, 2);
            Assert.AreNotSame(stream, node.Stream);
            Assert.AreSame(stream.BaseStream, node.Stream.BaseStream);

            node.Dispose();
            Assert.That(node.Disposed, Is.True);
            Assert.That(stream.Disposed, Is.False);
            stream.Dispose();
        }

        [Test]
        public void CreateFromStandardSubstreamTransferOwnership()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);

            Node node = NodeFactory.FromSubstream("node", stream, 1, 2);

            Assert.That(node.Stream.BaseStream, Is.SameAs(stream));

            node.Dispose();
            Assert.That(() => stream.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void CreateFromSubstreamWithInvalidNameThrowsException()
        {
            using DataStream main = new DataStream();
            Assert.That(
                () => NodeFactory.FromSubstream(null, main, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromSubstream(string.Empty, main, 0, 0),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromSubstreamWithNullStreamThrowsException()
        {
            Assert.That(
                () => NodeFactory.FromSubstream("node", null, 0, 0),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromDirectoryWithDoubleSlashes()
        {
            // Issue #139
            string root = Path.GetTempPath();
            string child = Path.GetRandomFileName();
            string tempDir = Path.Combine(root, child);
            Directory.CreateDirectory(tempDir);

            string tempFolder = Path.Combine(tempDir, "folder");
            Assert.That(Directory.CreateDirectory(tempFolder).Exists, Is.True);
            string tempFile = Path.Combine(tempFolder, "file.txt");
            File.Create(tempFile).Dispose();

            Node node = NodeFactory.FromDirectory(
                string.Concat(root, Path.DirectorySeparatorChar, Path.DirectorySeparatorChar, child),
                "*",
                "Issue139",
                true);

            Directory.Delete(tempDir, true);

            Assert.AreEqual(1, node.Children.Count);
            Assert.That(node.Children["folder"], Is.Not.Null);
            Assert.AreEqual(1, node.Children["folder"].Children.Count);
            Assert.That(node.Children["folder"].Children["file.txt"], Is.Not.Null);

            node.Dispose();
        }

        [Test]
        public void ReadFromReadonlyFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllBytes(tempFile, new byte[] { 0xCA, 0xFE, 0x00, 0xFF });
            File.SetAttributes(tempFile, FileAttributes.ReadOnly);

            Node node = NodeFactory.FromFile(tempFile, FileOpenMode.Read);
            byte[] buffer = new byte[4];
            _ = node.Stream.Read(buffer, 0, 4);
            node.Dispose();

            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }

        [Test]
        public void ReadFromReadonlyFileUsingWriteModeThrowsException()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            File.WriteAllBytes(tempFile, new byte[] { 0xCA, 0xFE, 0x00, 0xFF });
            File.SetAttributes(tempFile, FileAttributes.ReadOnly);

            Node node = NodeFactory.FromFile(tempFile, FileOpenMode.ReadWrite);
            byte[] buffer = new byte[4];
            Assert.Throws<UnauthorizedAccessException>(() => _ = node.Stream.Read(buffer, 0, 4));
            node.Dispose();

            File.SetAttributes(tempFile, FileAttributes.Normal);
            File.Delete(tempFile);
        }

        // Advanced filter
        [Test]
        public void CreateFromDirectoryAdvancedFilterAndEmptyPath()
        {
            Assert.That(
                () => NodeFactory.FromDirectory(null, _ => true),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromDirectory(string.Empty, _ => true),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromDirectory(null, _ => true, "name"),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromDirectory(string.Empty, _ => true, "name"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromDirectoryAdvancedFilterAndEmptyFilter()
        {
            Assert.That(
                () => NodeFactory.FromDirectory("dir", (Func<string, bool>)null),
                Throws.ArgumentNullException);

            Assert.That(
                () => NodeFactory.FromDirectory("dir", (Func<string, bool>)null, "name"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromDirectoryAdvancedFilterAndEmptyName()
        {
            Assert.That(
                () => NodeFactory.FromDirectory("dir", _ => true, null),
                Throws.ArgumentNullException);
            Assert.That(
                () => NodeFactory.FromDirectory("dir", _ => true, string.Empty),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromDirectoryAdvancedFilterWithFinalSlash()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, Path.GetRandomFileName());
            File.Create(tempFile1).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir + Path.DirectorySeparatorChar, _ => true);
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.AreEqual(1, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            node.Dispose();

            node = NodeFactory.FromDirectory(tempDir + Path.DirectorySeparatorChar, _ => true, "name");
            Assert.AreEqual("name", node.Name);
            Assert.IsTrue(node.IsContainer);
            Assert.AreEqual(1, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            node.Dispose();

            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithFilesAndAdvancedFilter()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, "file1.bin");
            File.Create(tempFile1).Dispose();
            string tempFile2 = Path.Combine(tempDir, "arch2.bin");
            File.Create(tempFile2).Dispose();
            string tempFile3 = Path.Combine(tempDir, "file3.txt");
            File.Create(tempFile3).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, x => Regex.IsMatch(x, @"file\d\.(txt|bin)$"));
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile3)));
            node.Dispose();

            node = NodeFactory.FromDirectory(tempDir, x => x.EndsWith(".bin"));
            Assert.AreEqual(Path.GetFileName(tempDir), node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile2)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }

        [Test]
        public void CreateFromDirectoryWithFilesAndNameAndAdvancedFilter()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);

            string tempFile1 = Path.Combine(tempDir, "file1.bin");
            File.Create(tempFile1).Dispose();
            string tempFile2 = Path.Combine(tempDir, "arch2.bin");
            File.Create(tempFile2).Dispose();
            string tempFile3 = Path.Combine(tempDir, "file3.txt");
            File.Create(tempFile3).Dispose();

            Node node = NodeFactory.FromDirectory(tempDir, x => Regex.IsMatch(x, @"file\d\.(txt|bin)$"), "MyDir");
            Assert.AreEqual("MyDir", node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile3)));
            node.Dispose();

            node = NodeFactory.FromDirectory(tempDir, x => x.EndsWith(".bin"), "MyDir");
            Assert.AreEqual("MyDir", node.Name);
            Assert.AreEqual(2, node.Children.Count);
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile1)));
            Assert.IsTrue(node.Children.Any(n => n.Name == Path.GetFileName(tempFile2)));

            node.Dispose();
            Directory.Delete(tempDir, true);
        }
    }
}
