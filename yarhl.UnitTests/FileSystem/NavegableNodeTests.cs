//
// NavegableNodeTests.cs
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
    using System.Collections.Generic;
    using NUnit.Framework;
    using Yarhl.FileSystem;

    [TestFixture]
    public class NavegableNodeTests
    {
        [Test]
        public void DefaultValues()
        {
            var node = new DummyNavegable("NodeName");
            Assert.AreEqual("NodeName", node.Name);
            Assert.AreEqual("/NodeName", node.Path);
            Assert.IsNull(node.Parent);
            Assert.IsEmpty(node.Children);
            Assert.IsEmpty(node.Tags);
        }

        [Test]
        public void ExceptionIfInvalidName()
        {
            Assert.Throws<ArgumentNullException>(() => new DummyNavegable(null));
            Assert.That(
                () => new DummyNavegable(string.Empty),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ExceptionIfInvalidCharacters()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new DummyNavegable("MyT/est"));
            Assert.AreEqual(
                "Name contains invalid characters" + Environment.NewLine +
                "Parameter name: name",
                ex.Message);
        }

        [Test]
        public void NameProperty()
        {
            var node = new DummyNavegable("MyNameTest");
            Assert.AreEqual("MyNameTest", node.Name);
        }

        [Test]
        public void PathIfParentIsNull()
        {
            var node = new DummyNavegable("MyNameTest");
            Assert.AreEqual("/MyNameTest", node.Path);
        }

        [Test]
        public void PathWithParent()
        {
            var node = new DummyNavegable("MyChild");
            var parentNode = new DummyNavegable("MyParent");
            parentNode.Add(node);

            Assert.AreEqual("/MyParent/MyChild", node.Path);
            Assert.AreEqual("/MyParent", parentNode.Path);
        }

        [Test]
        public void TagsAllowAdding()
        {
            var node = new DummyNavegable("MyNameTest");
            node.Tags["MyTag"] = 5;
            Assert.AreEqual(5, node.Tags["MyTag"]);
        }

        [Test]
        public void AddChildUpdatesChildrenAndParent()
        {
            var node = new DummyNavegable("MyChild");
            var parentNode = new DummyNavegable("MyParent");
            parentNode.Add(node);

            Assert.AreSame(parentNode, node.Parent);
            Assert.AreEqual(1, parentNode.Children.Count);
            Assert.AreSame(node, parentNode.Children[0]);
        }

        [Test]
        public void ChildrenGetsByName()
        {
            var node = new DummyNavegable("MyChild");
            var parentNode = new DummyNavegable("MyParent");
            parentNode.Add(node);
            Assert.AreSame(node, parentNode.Children["MyChild"]);
        }

        [Test]
        public void ExceptionIfNullChild()
        {
            var node = new DummyNavegable("MyParent");
            DummyNavegable child = null;
            Assert.Throws<ArgumentNullException>(() => node.Add(child));
        }

        [Test]
        public void ReplaceIfSameName()
        {
            var children1 = new DummyNavegable("MyChild1");
            var children2 = new DummyNavegable("MyChild1");
            var parent = new DummyNavegable("MyParent");

            parent.Add(children1);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(children1, parent.Children[0]);

            parent.Add(children2);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(children2, parent.Children[0]);
            Assert.AreNotSame(children1, parent.Children[0]);
        }

        [Test]
        public void ReplaceDisposePreviousChild()
        {
            var child1 = new DummyNavegable("MyChild1");
            var child2 = new DummyNavegable("MyChild1");
            var parent = new DummyNavegable("MyParent");

            parent.Add(child1);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(child1, parent.Children[0]);
            Assert.That(child1.Disposed, Is.False);

            parent.Add(child2);
            Assert.AreEqual(1, parent.Children.Count);
            Assert.AreSame(child2, parent.Children[0]);
            Assert.AreNotSame(child1, parent.Children[0]);
            Assert.That(child1.Disposed, Is.True);
            Assert.That(child2.Disposed, Is.False);
        }

        [Test]
        public void AddAllChildren()
        {
            var children = new List<DummyNavegable>();
            children.Add(new DummyNavegable("MyChild1"));
            children.Add(new DummyNavegable("MyChild2"));
            children.Add(new DummyNavegable("MyChild3"));
            var parent = new DummyNavegable("MyParent");

            parent.Add(children);
            Assert.AreEqual(3, parent.Children.Count);
            Assert.AreSame(children[0], parent.Children[0]);
            Assert.AreSame(children[1], parent.Children[1]);
            Assert.AreSame(children[2], parent.Children[2]);
        }

        [Test]
        public void AddThrowsExceptionAfterDispose()
        {
            var node = new DummyNavegable("MyParent");
            DummyNavegable child = null;
            node.Dispose();
            Assert.That(
                () => node.Add(child),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void AddChildrenThrowExceptionIfNull()
        {
            var node = new DummyNavegable("MyParent");
            List<DummyNavegable> children = null;
            Assert.Throws<ArgumentNullException>(() => node.Add(children));
        }

        [Test]
        public void AddChildrenThrowsExceptionAfterDispose()
        {
            var node = new DummyNavegable("MyParent");
            var children = new List<DummyNavegable> { new DummyNavegable("child") }; 
            node.Dispose();
            Assert.That(
                () => node.Add(children),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveChildren()
        {
            var children = new List<DummyNavegable>();
            children.Add(new DummyNavegable("MyChild1"));
            children.Add(new DummyNavegable("MyChild2"));
            children.Add(new DummyNavegable("MyChild3"));
            var parent = new DummyNavegable("MyParent");

            parent.Add(children);
            Assert.AreEqual(3, parent.Children.Count);

            parent.RemoveChildren();
            Assert.IsEmpty(parent.Children);
        }

        [Test]
        public void RemoveChildrenRemovesInnerChildren()
        {
            DummyNavegable parent = new DummyNavegable("Parent");
            DummyNavegable child1 = new DummyNavegable("Child1");
            DummyNavegable child2 = new DummyNavegable("Child2");
            DummyNavegable subchild1 = new DummyNavegable("Subchild1");
            child1.Add(subchild1);
            parent.Add(child1);
            parent.Add(child2);

            parent.RemoveChildren();
            Assert.IsEmpty(parent.Children);
            Assert.IsEmpty(child1.Children);
        }

        [Test]
        public void RemoveChildrenDisposeChildren()
        {
            DummyNavegable parent = new DummyNavegable("Parent");
            DummyNavegable child1 = new DummyNavegable("Child1");
            DummyNavegable child2 = new DummyNavegable("Child2");
            DummyNavegable subchild1 = new DummyNavegable("Subchild1");
            child1.Add(subchild1);
            parent.Add(child1);
            parent.Add(child2);

            Assert.IsFalse(parent.Disposed);
            Assert.IsFalse(child1.Disposed);
            Assert.IsFalse(child2.Disposed);
            Assert.IsFalse(subchild1.Disposed);

            parent.RemoveChildren();
            Assert.IsFalse(parent.Disposed);
            Assert.IsTrue(child1.Disposed);
            Assert.IsTrue(child2.Disposed);
            Assert.IsTrue(subchild1.Disposed);
        }

        [Test]
        public void RemoveChildrenAfterDisposeThrowsException()
        {
            var node = new DummyNavegable("node");
            node.Add(new DummyNavegable("child"));
            node.Dispose();
            Assert.That(node.RemoveChildren, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void DisposeChangesProperty()
        {
            var node = new DummyNavegable("node");
            Assert.That(node.Disposed, Is.False);
            node.Dispose();
            Assert.That(node.Disposed, Is.True);
        }

        [Test]
        public void DisposeTwiceDoesNotThrowException()
        {
            var node = new DummyNavegable("node");
            node.Dispose();
            Assert.That(node.Dispose, Throws.Nothing);
        }

        [Test]
        public void DisposeRemoveChildrens()
        {
            DummyNavegable parent = new DummyNavegable("Parent");
            DummyNavegable child1 = new DummyNavegable("Child1");
            DummyNavegable child2 = new DummyNavegable("Child2");
            DummyNavegable subchild1 = new DummyNavegable("Subchild1");
            child1.Add(subchild1);
            parent.Add(child1);
            parent.Add(child2);

            parent.Dispose();
            Assert.IsEmpty(parent.Children);
            Assert.IsEmpty(child1.Children);

            Assert.IsTrue(parent.Disposed);
            Assert.IsTrue(child1.Disposed);
            Assert.IsTrue(child2.Disposed);
            Assert.IsTrue(subchild1.Disposed);
        }

        class DummyNavegable : NavegableNode<DummyNavegable>
        {
            public DummyNavegable(string name) : base(name)
            {
            }
        }
    }
}
