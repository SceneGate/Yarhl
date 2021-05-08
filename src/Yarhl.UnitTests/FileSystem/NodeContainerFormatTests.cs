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
    using NUnit.Framework;
    using Yarhl.FileSystem;
    using Yarhl.UnitTests.FileFormat;

    [TestFixture]
    public class NodeContainerFormatTests : BaseGeneralTests<NodeContainerFormat>
    {
        [Test]
        public void DisposeChangesDisposed()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void DisposeTWicheDoesNotThrow()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
            Assert.That(() => format.Dispose(), Throws.Nothing);
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void ConstructorSetProperties()
        {
            using NodeContainerFormat format = new NodeContainerFormat();
            Assert.IsNotNull(format.Root);
            Assert.IsEmpty(format.Root.Children);
        }

        [Test]
        public void DisposeIsDisposingRoot()
        {
            NodeContainerFormat format = CreateDummyFormat();
            format.Dispose();
            Assert.IsTrue(format.Root.Disposed);
        }

        [Test]
        public void AddAfterDisposeThrowsException()
        {
            NodeContainerFormat format = CreateDummyFormat();
            format.Dispose();
            using Node child = new Node("Child");
            Assert.Throws<ObjectDisposedException>(
                () => format.Root.Add(child));
        }

        [Test]
        public void MoveChildrenReplaceRootNode()
        {
            Node node = new Node("MyTest");
            using NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));

            format.MoveChildrenTo(node);
            Assert.AreSame(node, format.Root);
        }

        [Test]
        public void MoveChildrenDoesNotDisposeOldNode()
        {
            Node node = new Node("MyTest");
            using NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));
            Node prevNode = format.Root;

            format.MoveChildrenTo(node);
            Assert.AreNotSame(prevNode, format.Root);
            Assert.IsFalse(prevNode.Disposed);
        }

        [Test]
        public void MoveChildrenDoesNotDisposeChildrenAfterDispose()
        {
            Node node = new Node("MyTest");
            using NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));
            format.MoveChildrenTo(node);

            format.Dispose();
            Assert.IsFalse(node.Disposed);
            Assert.IsNotEmpty(node.Children);
            Assert.IsFalse(node.Children[0].Disposed);
        }

        [Test]
        public void MoveChildrenToNodeAfterDisposeThrowsException()
        {
            NodeContainerFormat format = CreateDummyFormat();
            using Node dummy = new Node("Dummy");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.MoveChildrenTo(dummy));
        }

        [Test]
        public void MoveChildrenToNodeThrowsIfNullNode()
        {
            using NodeContainerFormat format = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => format.MoveChildrenTo(null));
        }

        [Test]
        public void MoveChildrenToNodeRemovesChildrenFromSource()
        {
            using NodeContainerFormat source = new NodeContainerFormat();
            Node sourceRoot = source.Root;

            using NodeContainerFormat destination = new NodeContainerFormat();

            using Node child = new Node("Child");

            source.Root.Add(child);
            source.MoveChildrenTo(destination.Root);

            Assert.That(sourceRoot.Children.Count, Is.Zero);
            Assert.That(destination.Root.Children.Count, Is.EqualTo(1));
        }

        [Test]
        public void MoveChildrenToNodeCanMergeContainers()
        {
            using Node source = new Node("source", new NodeContainerFormat());
            using Node destination = new Node("destination", new NodeContainerFormat());

            using Node folder1 = new Node("Folder1", new NodeContainerFormat());
            using Node folder2 = new Node("Folder2", new NodeContainerFormat());
            using Node folder3 = new Node("Folder1", new NodeContainerFormat());

            using Node node1 = new Node("File1");
            using Node node2 = new Node("File2");
            using Node node3 = new Node("File3");
            using Node node4 = new Node("File4");

            folder1.Add(node1);
            folder1.Add(node2);
            folder2.Add(node3);
            folder3.Add(node4);

            destination.Add(folder1);
            destination.Add(folder2);
            source.Add(folder3);

            Assert.That(source.Children.Count, Is.EqualTo(1));
            Assert.That(destination.Children.Count, Is.EqualTo(2));
            Assert.That(folder1.Children.Count, Is.EqualTo(2));
            Assert.That(folder2.Children.Count, Is.EqualTo(1));
            Assert.That(folder3.Children.Count, Is.EqualTo(1));

            source.GetFormatAs<NodeContainerFormat>().MoveChildrenTo(destination, true);

            Assert.That(source.Children.Count, Is.Zero);
            Assert.That(destination.Children.Count, Is.EqualTo(2));
            Assert.That(folder1.Children.Count, Is.EqualTo(3));
            Assert.That(folder2.Children.Count, Is.EqualTo(1));
            Assert.That(folder3.Children.Count, Is.Zero);
        }

        [Test]
        public void Clone()
        {
            using NodeContainerFormat format = new NodeContainerFormat();

            using Node child1 = NodeFactory.CreateContainer("child1");
            using Node child2 = new Node("child2");
            using Node grandchild = new Node("grandchild");

            format.Root.Add(child1);
            format.Root.Add(child2);
            child1.Add(grandchild);

            using NodeContainerFormat clone = (NodeContainerFormat)format.DeepClone();

            Node child1Clone = clone.Root.Children["child1"];
            Node child2Clone = clone.Root.Children["child2"];
            Node grandchildClone = child1Clone.Children["grandchild"];

            Assert.AreNotSame(format, clone);
            Assert.AreNotSame(child1, child1Clone);
            Assert.AreNotSame(child2, child2Clone);
            Assert.AreNotSame(grandchild, grandchildClone);
        }

        protected override NodeContainerFormat CreateDummyFormat()
        {
            return new NodeContainerFormat();
        }
    }
}
