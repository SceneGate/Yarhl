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
    using System.Collections.Generic;
    using NUnit.Framework;
    using Yarhl.FileSystem;

    [TestFixture]
    public class NavigableNodeTests
    {
        [Test]
        public void DefaultValues()
        {
            using var node = new DummyNavigable("NodeName");
            Assert.AreEqual("NodeName", node.Name);
            Assert.AreEqual("/NodeName", node.Path);
            Assert.IsNull(node.Parent);
            Assert.IsEmpty(node.Children);
            Assert.IsEmpty(node.Tags);
        }

        [Test]
        public void ExceptionIfInvalidName()
        {
            Assert.Throws<ArgumentNullException>(() => new DummyNavigable(null));
            Assert.That(
                () => new DummyNavigable(string.Empty),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ExceptionIfInvalidCharacters()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new DummyNavigable("MyT/est"));
            Assert.That(
                ex.Message,
                Contains.Substring("Name contains invalid characters"));
        }

        [Test]
        public void NameProperty()
        {
            using var node = new DummyNavigable("MyNameTest");
            Assert.AreEqual("MyNameTest", node.Name);
        }

        [Test]
        public void PathIfParentIsNull()
        {
            using var node = new DummyNavigable("MyNameTest");
            Assert.AreEqual("/MyNameTest", node.Path);
        }

        [Test]
        public void PathWithParent()
        {
            using var parentNode = new DummyNavigable("MyParent");
            var node = new DummyNavigable("MyChild");
            parentNode.Add(node);

            Assert.AreEqual("/MyParent/MyChild", node.Path);
            Assert.AreEqual("/MyParent", parentNode.Path);
        }

        [Test]
        public void TagsAllowAdding()
        {
            using var node = new DummyNavigable("MyNameTest");
            node.Tags["MyTag"] = 5;
            Assert.AreEqual(5, node.Tags["MyTag"]);
        }

        [Test]
        public void AddChildUpdatesChildrenAndParent()
        {
            using var parentNode = new DummyNavigable("MyParent");
            var node = new DummyNavigable("MyChild");
            parentNode.Add(node);

            Assert.AreSame(parentNode, node.Parent);
            Assert.AreEqual(1, parentNode.Children.Count);
            Assert.AreSame(node, parentNode.Children[0]);
        }

        [Test]
        public void AddParentThrowException()
        {
            using var grandparent = new DummyNavigable("Grandparent");
            using var parent = new DummyNavigable("Parent");
            using var child = new DummyNavigable("Child");
            grandparent.Add(parent);
            parent.Add(child);

            Assert.That(() => child.Add(parent), Throws.ArgumentException);
            Assert.That(() => child.Add(grandparent), Throws.ArgumentException);
            Assert.That(() => parent.Add(grandparent), Throws.ArgumentException);
        }

        [Test]
        public void ChildrenGetsByName()
        {
            using var parentNode = new DummyNavigable("MyParent");
            var node = new DummyNavigable("MyChild");
            parentNode.Add(node);
            Assert.AreSame(node, parentNode.Children["MyChild"]);
        }

        [Test]
        public void ExceptionIfNullChild()
        {
            using var node = new DummyNavigable("MyParent");
            DummyNavigable child = null;
            Assert.Throws<ArgumentNullException>(() => node.Add(child));
        }

        [Test]
        public void ReplaceIfSameName()
        {
            using var children1 = new DummyNavigable("MyChild1");
            using var children2 = new DummyNavigable("MyChild1");
            using var parent = new DummyNavigable("MyParent");

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
            using var child1 = new DummyNavigable("MyChild1");
            using var child2 = new DummyNavigable("MyChild1");
            using var parent = new DummyNavigable("MyParent");

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
            using var parent = new DummyNavigable("MyParent");
            var children = new List<DummyNavigable>();
            children.Add(new DummyNavigable("MyChild1"));
            children.Add(new DummyNavigable("MyChild2"));
            children.Add(new DummyNavigable("MyChild3"));

            parent.Add(children);
            Assert.AreEqual(3, parent.Children.Count);
            Assert.AreSame(children[0], parent.Children[0]);
            Assert.AreSame(children[1], parent.Children[1]);
            Assert.AreSame(children[2], parent.Children[2]);
        }

        [Test]
        public void AddThrowsExceptionAfterDispose()
        {
            var node = new DummyNavigable("MyParent");
            DummyNavigable child = null;
            node.Dispose();
            Assert.That(
                () => node.Add(child),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void AddChildrenThrowExceptionIfNull()
        {
            using var node = new DummyNavigable("MyParent");
            List<DummyNavigable> children = null;
            Assert.Throws<ArgumentNullException>(() => node.Add(children));
        }

        [Test]
        public void AddChildrenThrowsExceptionAfterDispose()
        {
            var node = new DummyNavigable("MyParent");
            var children = new List<DummyNavigable> { new DummyNavigable("child") };
            node.Dispose();
            Assert.That(
                () => node.Add(children),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveChildByNode()
        {
            using var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            var child2 = new DummyNavigable("Child2");
            node.Add(child1);
            node.Add(child2);

            Assert.That(node.Children.Count, Is.EqualTo(2));
            Assert.That(node.Remove(child1), Is.True);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(node.Children[0], Is.SameAs(child2));
            Assert.That(child1.Parent, Is.Null);
        }

        [Test]
        public void RemoveChildByNodeDoesNotDispose()
        {
            using var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            node.Add(child1);

            Assert.That(node.Remove(child1), Is.True);

            Assert.That(child1.Disposed, Is.False);
        }

        [Test]
        public void RemoveChildByNodeReturnsFalseIfNoFound()
        {
            using var node = new DummyNavigable("My parent");
            using var node2 = new DummyNavigable("My parent2");
            using var child1 = new DummyNavigable("Child1");
            node2.Add(child1);
            using var child2 = new DummyNavigable("Child2");
            node.Add(child2);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(node.Children[0], Is.SameAs(child2));
            Assert.That(node.Remove(child1), Is.False);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(child1.Disposed, Is.False);
            Assert.That(child1.Parent, Is.SameAs(node2));
        }

        [Test]
        public void RemoveChildByNodeThrowsWhenNull()
        {
            using var node = new DummyNavigable("My parent");
            var child1 = new DummyNavigable("Child1");
            node.Add(child1);

            Assert.That(
                () => node.Remove((DummyNavigable)null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void RemoveChildByNodeThrowsWhenDisposed()
        {
            var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            node.Add(child1);
            node.Dispose();

            Assert.That(
                () => node.Remove(child1),
                Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveChildByName()
        {
            using var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            using var child2 = new DummyNavigable("Child2");
            node.Add(child1);
            node.Add(child2);

            Assert.That(node.Children.Count, Is.EqualTo(2));
            Assert.That(node.Remove("Child1"), Is.True);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(node.Children[0], Is.SameAs(child2));
        }

        [Test]
        public void RemoveChildByNameDisposes()
        {
            using var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            node.Add(child1);

            Assert.That(node.Remove("Child1"), Is.True);
            Assert.That(child1.Disposed, Is.True);
        }

        [Test]
        public void RemoveChildByNameReturnsFalseIfNoFound()
        {
            using var node = new DummyNavigable("My parent");
            using var node2 = new DummyNavigable("My parent2");
            using var child1 = new DummyNavigable("Child1");
            node2.Add(child1);
            using var child2 = new DummyNavigable("Child2");
            node.Add(child2);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(node.Children[0], Is.SameAs(child2));
            Assert.That(node.Remove("Child1"), Is.False);

            Assert.That(node.Children.Count, Is.EqualTo(1));
            Assert.That(child1.Disposed, Is.False);
            Assert.That(child1.Parent, Is.SameAs(node2));
        }

        [Test]
        public void RemoveChildByNameThrowsWhenNull()
        {
            using var node = new DummyNavigable("My parent");
            var child1 = new DummyNavigable("Child1");
            node.Add(child1);

            Assert.That(
                () => node.Remove((string)null),
                Throws.ArgumentNullException);
            Assert.That(
                () => node.Remove(string.Empty),
                Throws.ArgumentNullException);
        }

        [Test]
        public void RemoveChildByNameThrowsWhenDisposed()
        {
            var node = new DummyNavigable("My parent");
            using var child1 = new DummyNavigable("Child1");
            node.Add(child1);
            node.Dispose();

            Assert.That(
                () => node.Remove("Child1"),
                Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveChildren()
        {
            using var parent = new DummyNavigable("MyParent");
            var children = new List<DummyNavigable>();
            children.Add(new DummyNavigable("MyChild1"));
            children.Add(new DummyNavigable("MyChild2"));
            children.Add(new DummyNavigable("MyChild3"));

            parent.Add(children);
            Assert.AreEqual(3, parent.Children.Count);

            parent.RemoveChildren();
            Assert.IsEmpty(parent.Children);
        }

        [Test]
        public void RemoveChildrenRemovesInnerChildren()
        {
            using DummyNavigable parent = new DummyNavigable("Parent");
            using DummyNavigable child1 = new DummyNavigable("Child1");
            using DummyNavigable child2 = new DummyNavigable("Child2");
            using DummyNavigable subchild1 = new DummyNavigable("Subchild1");
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
            using DummyNavigable parent = new DummyNavigable("Parent");
            using DummyNavigable child1 = new DummyNavigable("Child1");
            using DummyNavigable child2 = new DummyNavigable("Child2");
            using DummyNavigable subchild1 = new DummyNavigable("Subchild1");
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
            var node = new DummyNavigable("node");
            using var child = new DummyNavigable("child");
            node.Add(child);
            node.Dispose();
            Assert.That(node.RemoveChildren, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void DisposeChangesProperty()
        {
            var node = new DummyNavigable("node");
            Assert.That(node.Disposed, Is.False);
            node.Dispose();
            Assert.That(node.Disposed, Is.True);
        }

        [Test]
        public void DisposeTwiceDoesNotThrowException()
        {
            var node = new DummyNavigable("node");
            node.Dispose();
            Assert.That(node.Dispose, Throws.Nothing);
        }

        [Test]
        public void DisposeRemoveChildrens()
        {
            DummyNavigable parent = new DummyNavigable("Parent");
            using DummyNavigable child1 = new DummyNavigable("Child1");
            using DummyNavigable child2 = new DummyNavigable("Child2");
            using DummyNavigable subchild1 = new DummyNavigable("Subchild1");
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

        [Test]
        public void MergeNodeChildren()
        {
            using var parent1 = new DummyNavigable("Parent1");
            using var child1 = new DummyNavigable("Child");
            using var subchild1 = new DummyNavigable("Subchild1");

            child1.Tags.Add("Tag1", "Value1");

            using var parent2 = new DummyNavigable("Parent2");
            using var child2 = new DummyNavigable("Child");
            using var child3 = new DummyNavigable("Child2");
            using var subchild2 = new DummyNavigable("Subchild2");

            child2.Tags.Add("Tag1", "Value2");
            child2.Tags.Add("Tag2", "Value3");

            parent1.Add(child1);
            child1.Add(subchild1);

            parent2.Add(child2);
            parent2.Add(child3);
            child2.Add(subchild2);

            parent1.Add(parent2.Children, false);

            Assert.AreEqual(2, parent1.Children.Count);
            Assert.AreEqual(2, child1.Children.Count);
            Assert.AreEqual(2, child1.Tags.Count);
            Assert.AreEqual("Value1", child1.Tags["Tag1"]);
            Assert.AreEqual("Value3", child1.Tags["Tag2"]);
        }

        class DummyNavigable : NavigableNode<DummyNavigable>
        {
            public DummyNavigable(string name)
                : base(name)
            {
            }
        }
    }
}
