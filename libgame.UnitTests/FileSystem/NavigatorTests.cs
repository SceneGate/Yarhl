//
// NavigatorTests.cs
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
    using System.Linq;
    using NUnit.Framework;
    using Libgame.FileSystem;

    [TestFixture]
    public class NavigatorTests
    {
        [Test]
        public void ConstructorSetNode()
        {
            Node node = new Node("MyNode");
            var navigator = new Navigator<Node>(node);
            Assert.AreSame(node, navigator.Node);
        }

        [Test]
        public void ConstructorThrowExceptionIfNullNode()
        {
            Assert.Throws<ArgumentNullException>(() => new Navigator<Node>(null));
        }

        [Test]
        public void SearchNullPathThrowsException()
        {
            Node node = new Node("MyNode");
            var navigator = new Navigator<Node>(node);
            Assert.Throws<ArgumentNullException>(() => navigator.SearchFile(null));
        }

        [Test]
        public void SearchEmptyPath()
        {
            Node node = new Node("MyNode");
            var navigator = new Navigator<Node>(node);
            Assert.Throws<ArgumentNullException>(() => navigator.SearchFile(""));
        }

        [Test]
        public void SearchInvalidPrefixPath()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            var navigator = new Navigator<Node>(nodeChild);
            Assert.IsNull(navigator.SearchFile("/OtherParent"));
        }

        [Test]
        public void SearchExactPath()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            var navigator = new Navigator<Node>(nodeChild);
            Assert.AreSame(nodeChild, navigator.SearchFile("/Parent/Child"));
        }

        [Test]
        public void SearchInChildren()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            var navigator = new Navigator<Node>(nodeParent);
            Assert.AreSame(nodeChild, navigator.SearchFile("/Parent/Child"));
        }

        [Test]
        public void SearchRecursivelyTwoLevels()
        {
            Node nodeSubChild = new Node("SubChild");
            Node nodeChild = new Node("Child");
            nodeChild.Add(nodeSubChild);
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            var navigator = new Navigator<Node>(nodeParent);
            Assert.AreSame(nodeSubChild, navigator.SearchFile("/Parent/Child/SubChild"));
        }

        [Test]
        public void SearchNotFound()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            var navigator = new Navigator<Node>(nodeParent);
            Assert.IsNull(navigator.SearchFile("/Parent/Child2"));
        }

        [Test]
        public void IterateNoChildren()
        {
            Node node = new Node("MyTest");
            var navigator = new Navigator<Node>(node);
            Assert.IsEmpty(navigator.IterateNodes());
        }

        [Test]
        public void IterateChildren()
        {
            Node child1 = new Node("Child1");
            Node child2 = new Node("Child2");
            Node node = new Node("MyTest");
            node.Add(child1);
            node.Add(child2);

            var navigator = new Navigator<Node>(node);
            var iteration = navigator.IterateNodes().ToList();
            Assert.AreEqual(2, iteration.Count);
            Assert.AreSame(child1, iteration[0]);
            Assert.AreSame(child2, iteration[1]);
        }

        [Test]
        public void IterateRecursivelyTwoLevels()
        {
            Node subChild1 = new Node("SubChild1");
            Node child1 = new Node("Child1");
            child1.Add(subChild1);
            Node child2 = new Node("Child2");
            Node node = new Node("MyTest");
            node.Add(child1);
            node.Add(child2);

            var navigator = new Navigator<Node>(node);
            var iteration = navigator.IterateNodes().ToList();
            Assert.AreEqual(3, iteration.Count);
            Assert.AreSame(child1, iteration[0]);
            Assert.AreSame(child2, iteration[1]);
            Assert.AreSame(subChild1, iteration[2]);
        }
    }
}
