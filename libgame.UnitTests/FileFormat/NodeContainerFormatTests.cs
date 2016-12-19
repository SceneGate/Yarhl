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
namespace Libgame.UnitTests.FileFormat
{
    using System;
    using System.Collections.Generic;
    using Libgame.FileSystem;
    using Libgame.FileFormat;
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
            Assert.IsNotNull(format.Children);
            Assert.IsEmpty(format.Children);
        }

        [Test]
        public void AddNewChildren()
        {
            NodeContainerFormat format = CreateDummyFormat();

            Node child1 = new Node("Child1");
            format.Add(child1);
            Assert.AreEqual(1, format.Children.Count);
            Assert.AreSame(child1, format.Children[0]);

            Node child2 = new Node("Child2");
            format.Add(child2);
            Assert.AreEqual(2, format.Children.Count);
            Assert.AreSame(child2, format.Children[1]);
        }

        [Test]
        public void ReplaceChildren()
        {
            NodeContainerFormat format = CreateDummyFormat();

            Node child = new Node("Child1");
            format.Add(child);
            Assert.AreEqual(1, format.Children.Count);
            Assert.AreSame(child, format.Children[0]);

            Node childSameName = new Node("Child1");
            format.Add(childSameName);
            Assert.AreEqual(1, format.Children.Count);
            Assert.AreNotSame(child, format.Children[0]);
            Assert.AreSame(childSameName, format.Children[0]);
        }

        [Test]
        public void AddListOfNodes()
        {
            NodeContainerFormat format = CreateDummyFormat();

            List<Node> children = new List<Node>();
            children.Add(new Node("Child1"));
            children.Add(new Node("Child2"));
            children.Add(new Node("Child3"));
            
            format.Add(children);
            Assert.AreEqual(3, format.Children.Count);
            Assert.AreSame(children[0], format.Children[0]);
            Assert.AreSame(children[1], format.Children[1]);
            Assert.AreSame(children[2], format.Children[2]);
        }

        [Test]
        public void AddListOfNodesNull()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => format.Add((List<Node>)null));
        }

        [Test]
        public void AddNullNode()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => format.Add((Node)null));
        }

        [Test]
        public void AddAndGetNodeByName()
        {
            NodeContainerFormat format = CreateDummyFormat();
            Node child = new Node("Child1");
            format.Add(child);
            Assert.AreSame(child, format.Children["Child1"]);
        }

        [Test]
        public void AddAfterDisposeThrowException()
        {
            NodeContainerFormat format = CreateDummyFormat();
            format.Dispose();
            Node child = new Node("Child");
            Assert.Throws<ObjectDisposedException>(() => format.Add(child));
        }

        [Test]
        public void AddListOfNodesAfterDisposeThrowException()
        {
            NodeContainerFormat format = CreateDummyFormat();
            format.Dispose();
            List<Node> children = new List<Node>();
            children.Add(new Node("Child"));
            Assert.Throws<ObjectDisposedException>(() => format.Add(children));
        }

        protected override NodeContainerFormat CreateDummyFormat()
        {
            return new NodeContainerFormat();
        }
    }
}
