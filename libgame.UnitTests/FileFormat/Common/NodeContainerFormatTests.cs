//
// NodeContainerFormatTests.cs
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
namespace Libgame.UnitTests.FileFormat.Common
{
    using System;
    using Libgame.FileSystem;
    using Libgame.FileFormat.Common;
    using NUnit.Framework;

    [TestFixture]
    public class NodeContainerFormatTests : BaseGeneralTests<NodeContainerFormat>
    {
        [Test]
        public void CorrectName()
        {
            NameIsCorrect("libgame", "nodecontainer");
        }

        [Test]
        public void ConstructorSetProperties()
        {
            NodeContainerFormat format = new NodeContainerFormat();
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
        public void AddAfterDisposeDoesNotThrowException()
        {
            NodeContainerFormat format = CreateDummyFormat();
            format.Dispose();
            Node child = new Node("Child");
            Assert.DoesNotThrow(() => format.Root.Add(child));
        }

        [Test]
        public void MoveChildrenReplaceRootNode()
        {
            Node node = new Node("MyTest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));

            format.MoveChildrenTo(node);
            Assert.AreSame(node, format.Root);
        }

        [Test]
        public void MoveChildrenDoesNotDisposeOldNode()
        {
            Node node = new Node("MyTest");
            NodeContainerFormat format = new NodeContainerFormat();
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
            NodeContainerFormat format = new NodeContainerFormat();
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
            Node dummy = new Node("Dummy");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.MoveChildrenTo(dummy));
        }

        [Test]
        public void MoveChildrenToNodeThrowsIfNullNode()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => format.MoveChildrenTo(null));
        }

        protected override NodeContainerFormat CreateDummyFormat()
        {
            return new NodeContainerFormat();
        }
    }
}
