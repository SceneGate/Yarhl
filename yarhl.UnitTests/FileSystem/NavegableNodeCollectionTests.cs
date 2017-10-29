//
// NavegableNodeCollectionTests.cs
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
    using System.Collections.Generic;
    using NUnit.Framework;
    using Yarhl.FileSystem;

    [TestFixture]
    public class NavegableNodeCollectionTests
    {
       [Test]
        public void GetElementAsList()
        {
            var children = new List<DummyNavegable>();
            var collection = new NavegableNodeCollection<DummyNavegable>(children);
            Assert.IsEmpty(collection);

            children.Add(new DummyNavegable("Child1"));
            children.Add(new DummyNavegable("Child2"));
            children.Add(new DummyNavegable("Child3"));
            Assert.AreEqual(3, collection.Count);
            Assert.AreSame(children[0], collection[0]);
            Assert.AreSame(children[1], collection[1]);
            Assert.AreSame(children[2], collection[2]);
        }

        [Test]
        public void GetNodesByName()
        {
            var children = new List<DummyNavegable>();
            var collection = new NavegableNodeCollection<DummyNavegable>(children);
            children.Add(new DummyNavegable("Child1"));
            children.Add(new DummyNavegable("Child2"));
            children.Add(new DummyNavegable("Child3"));
            Assert.AreSame(children[0], collection["Child1"]);
            Assert.AreSame(children[1], collection["Child2"]);
            Assert.AreSame(children[2], collection["Child3"]);
        }

        [Test]
        public void UnknownNameReturnsNull()
        {
            var children = new List<DummyNavegable>();
            var collection = new NavegableNodeCollection<DummyNavegable>(children);
            Assert.IsNull(collection["Child1"]);
        }

        class DummyNavegable : NavegableNode<DummyNavegable>
        {
            public DummyNavegable(string name) : base(name)
            {
            }
        }
    }
}
