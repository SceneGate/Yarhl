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
namespace Libgame.UnitTests.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Mono.Addins;
    using Moq;
    using NUnit.Framework;
    using Libgame.FileSystem;

    [TestFixture]
    public class NavegableNodeTests
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
        public void DefaultValues()
        {
            var node = new Mock<NavegableNode>("NodeName");
            Assert.AreEqual("NodeName", node.Object.Name);
            Assert.AreEqual("/NodeName", node.Object.Path);
            Assert.IsNull(node.Object.Parent);
            Assert.IsEmpty(node.Object.Children);
            Assert.IsEmpty(node.Object.Tags);
        }

        [Test]
        public void ExceptionIfNullName()
        {
            var mock = new Mock<NavegableNode>(null);
            NavegableNode node;
            var ex = Assert.Throws<TargetInvocationException>(() => node = mock.Object);
            Assert.IsInstanceOf<ArgumentNullException>(ex.InnerException);
        }

        [Test]
        public void NameProperty()
        {
            var mock = new Mock<NavegableNode>("MyNameTest");
            Assert.AreEqual("MyNameTest", mock.Object.Name);
        }

        [Test]
        public void PathIfParentIsNull()
        {
            var mock = new Mock<NavegableNode>("MyNameTest");
            Assert.AreEqual("/MyNameTest", mock.Object.Path);
        }

        [Test]
        public void PathWithParent()
        {
            var mock = new Mock<NavegableNode>("MyChild");
            var parentMock = new Mock<NavegableNode>("MyParent");
            parentMock.Object.Add(mock.Object);

            Assert.AreEqual("/MyParent/MyChild", mock.Object.Path);
            Assert.AreEqual("/MyParent", parentMock.Object.Path);
        }

        [Test]
        public void TagsAllowAdding()
        {
            var mock = new Mock<NavegableNode>("MyNameTest");
            mock.Object.Tags["MyTag"] = 5;
            Assert.AreEqual(5, mock.Object.Tags["MyTag"]);
        }

        [Test]
        public void AddChildUpdatesChildrenAndParent()
        {
            var mock = new Mock<NavegableNode>("MyChild");
            var parentMock = new Mock<NavegableNode>("MyParent");
            parentMock.Object.Add(mock.Object);

            Assert.AreSame(parentMock.Object, mock.Object.Parent);
            Assert.AreEqual(1, parentMock.Object.Children.Count);
            Assert.AreSame(mock.Object, parentMock.Object.Children[0]);
        }

        [Test]
        public void ChildrenGetsByName()
        {
            var mock = new Mock<NavegableNode>("MyChild");
            var parentMock = new Mock<NavegableNode>("MyParent");
            parentMock.Object.Add(mock.Object);
            Assert.AreSame(mock.Object, parentMock.Object.Children["MyChild"]);
        }

        [Test]
        public void ExceptionIfNullChild()
        {
            var mock = new Mock<NavegableNode>("MyParent");
            NavegableNode child = null;
            Assert.Throws<ArgumentNullException>(() => mock.Object.Add(child));
        }

        [Test]
        public void ReplaceIfSameName()
        {
            var childrenMock1 = new Mock<NavegableNode>("MyChild1");
            var childrenMock2 = new Mock<NavegableNode>("MyChild1");
            var parentMock = new Mock<NavegableNode>("MyParent");

            parentMock.Object.Add(childrenMock1.Object);
            Assert.AreEqual(1, parentMock.Object.Children.Count);
            Assert.AreSame(childrenMock1.Object, parentMock.Object.Children[0]);

            parentMock.Object.Add(childrenMock2.Object);
            Assert.AreEqual(1, parentMock.Object.Children.Count);
            Assert.AreSame(childrenMock2.Object, parentMock.Object.Children[0]);
            Assert.AreNotSame(childrenMock1.Object, parentMock.Object.Children[0]);
        }

        [Test]
        public void AddAllChildren()
        {
            var children = new List<NavegableNode>();
            children.Add(new Mock<NavegableNode>("MyChild1").Object);
            children.Add(new Mock<NavegableNode>("MyChild2").Object);
            children.Add(new Mock<NavegableNode>("MyChild3").Object);
            var parentMock = new Mock<NavegableNode>("MyParent");

            parentMock.Object.Add(children);
            Assert.AreEqual(3, parentMock.Object.Children.Count);
            Assert.AreSame(children[0], parentMock.Object.Children[0]);
            Assert.AreSame(children[1], parentMock.Object.Children[1]);
            Assert.AreSame(children[2], parentMock.Object.Children[2]);
        }

        [Test]
        public void AddChildrenThrowExceptionIfNull()
        {
            var mock = new Mock<NavegableNode>("MyParent");
            List<NavegableNode> children = null;
            Assert.Throws<ArgumentNullException>(() => mock.Object.Add(children));
        }

        [Test]
        public void RemoveChildren()
        {
            var children = new List<NavegableNode>();
            children.Add(new Mock<NavegableNode>("MyChild1").Object);
            children.Add(new Mock<NavegableNode>("MyChild2").Object);
            children.Add(new Mock<NavegableNode>("MyChild3").Object);
            var parentMock = new Mock<NavegableNode>("MyParent");

            parentMock.Object.Add(children);
            Assert.AreEqual(3, parentMock.Object.Children.Count);

            parentMock.Object.RemoveChildren();
            Assert.IsEmpty(parentMock.Object.Children);
        }
    }
}
