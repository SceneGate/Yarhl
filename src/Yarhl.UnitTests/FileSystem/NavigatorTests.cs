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
namespace Yarhl.UnitTests.FileSystem
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileSystem;

    [TestFixture]
    public class NavigatorTests
    {
        [Test]
        public void SearchNullNodeThrowsException()
        {
            Node node = null;
            Assert.Throws<ArgumentNullException>(() => Navigator.SearchFile(node, "/a/"));
        }

        [Test]
        public void SearchNullPathThrowsException()
        {
            Node node = new Node("MyTest");
            Assert.Throws<ArgumentNullException>(() => Navigator.SearchFile(node, null));
        }

        [Test]
        public void SearchEmptyPath()
        {
            Node node = new Node("MyNode");
            Assert.Throws<ArgumentNullException>(() => Navigator.SearchFile(node, string.Empty));
        }

        [Test]
        public void SearchInvalidPrefixPath()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);
            Assert.IsNull(Navigator.SearchFile(nodeChild, "/OtherParent"));
        }

        [Test]
        public void SearchExactPath()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);
            Assert.AreSame(nodeChild, Navigator.SearchFile(nodeChild, "/Parent/Child"));
        }

        [Test]
        public void SearchInChildren()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);
            Assert.AreSame(nodeChild, Navigator.SearchFile(nodeParent, "/Parent/Child"));
        }

        [Test]
        public void SearchRecursivelyTwoLevels()
        {
            Node nodeSubChild = new Node("SubChild");
            Node nodeChild = new Node("Child");
            nodeChild.Add(nodeSubChild);
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);
            Assert.AreSame(nodeSubChild, Navigator.SearchFile(nodeParent, "/Parent/Child/SubChild"));
        }

        [Test]
        public void SearchNotFound()
        {
            Node nodeChild = new Node("Child");
            Node nodeParent = new Node("Parent");
            nodeParent.Add(nodeChild);

            Assert.IsNull(Navigator.SearchFile(nodeParent, "/Parent/Child2"));
        }

        [Test]
        public void IterateNullNodeThrowsException()
        {
            Node node = null;
            Assert.Throws<ArgumentNullException>(() => Navigator.IterateNodes(node).First());
        }

        [Test]
        public void IterateNoChildren()
        {
            Node node = new Node("MyTest");
            Assert.IsEmpty(Navigator.IterateNodes(node));
        }

        [Test]
        public void IterateChildren()
        {
            Node child1 = new Node("Child1");
            Node child2 = new Node("Child2");
            Node node = new Node("MyTest");
            node.Add(child1);
            node.Add(child2);

            var iteration = Navigator.IterateNodes(node).ToList();
            Assert.AreEqual(2, iteration.Count);
            Assert.AreSame(child1, iteration[0]);
            Assert.AreSame(child2, iteration[1]);
        }

        [Test]
        public void IterateRecursivelyTwoLevelsDefaultIsBreadthFirst()
        {
            Node subChild1 = new Node("SubChild1");
            Node child1 = new Node("Child1");
            child1.Add(subChild1);
            Node child2 = new Node("Child2");
            Node node = new Node("MyTest");
            node.Add(child1);
            node.Add(child2);

            var iteration = Navigator.IterateNodes(node).ToList();
            Assert.AreEqual(3, iteration.Count);
            Assert.AreSame(child1, iteration[0]);
            Assert.AreSame(child2, iteration[1]);
            Assert.AreSame(subChild1, iteration[2]);
        }

        [Test]
        public void IterateBreadthFirst()
        {
            Node node = new Node("A");
            node.Add(new Node("B")); 
            node.Add(new Node("C"));
            node.Add(new Node("D"));
            node.Children["B"].Add(new Node("E"));
            node.Children["C"].Add(new Node("F"));
            node.Children["C"].Add(new Node("G"));

            var navigationOrder = new[] { "B", "C", "D", "E", "F", "G" };

            Assert.That(
                Navigator.IterateNodes(node, NavigationMode.BreadthFirst)
                    .Select(n => n.Name),
                Is.EqualTo(navigationOrder));
        }

        [Test]
        public void IterateRecursiveDepthFirst()
        {
            Node node = new Node("A");
            node.Add(new Node("B")); 
            node.Add(new Node("C"));
            node.Add(new Node("D"));
            node.Children["B"].Add(new Node("E"));
            node.Children["C"].Add(new Node("F"));
            node.Children["C"].Add(new Node("G"));

            var navigationOrder = new[] { "B", "E", "C", "F", "G", "D" };

            Assert.That(
                Navigator.IterateNodes(node, NavigationMode.DepthFirst)
                    .Select(n => n.Name),
                Is.EqualTo(navigationOrder));
        }

        [Test]
        public void IterateRecursiveInvalidMode()
        {
            Node node = new Node("A");
            Assert.That(
                () => Navigator.IterateNodes(node, (NavigationMode)0x100),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
    }
}
