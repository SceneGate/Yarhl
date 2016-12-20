//
// NodeTests.cs
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
    using Mono.Addins;
    using NUnit.Framework;
    using Libgame.FileFormat;
    using Libgame.FileSystem;
    using FileFormat;

    [TestFixture]
    public class NodeTests
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
        public void ConstructorSetName()
        {
            Node node = new Node("mytest");
            Assert.AreEqual("mytest", node.Name);
        }

        [Test]
        public void ConstructorSetNameAndFormat()
        {
            Format dummyFormat = new StringFormatTest("");
            Node node = new Node("mytest", dummyFormat);
            Assert.AreEqual("mytest", node.Name);
            Assert.AreSame(dummyFormat, node.Format);
        }

        [Test]
        public void ConstructorFormatInProperty()
        {
            Format dummyFormat = new StringFormatTest("");
            Node node = new Node("mytest", dummyFormat);
            Assert.AreSame(dummyFormat, node.Format);
        }

        [Test]
        public void ConstructorWithoutFormatNullProperty()
        {
            Node node = new Node("mytest");
            Assert.IsNull(node.Format);
        }

        [Test]
        public void TransformChangeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);

            node.Transform<IntFormatTest>();
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(3, (node.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformTypedChangeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);

            node.Transform(typeof(IntFormatTest));
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(3, (node.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformWithoutFormatThrowException()
        {
            Node node = new Node("mytest");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                node.Transform<IntFormatTest>());
            Assert.AreEqual("Cannot transform a node without format", ex.Message);
        }

        [Test]
        public void TransformReturnsItself()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            Assert.AreSame(node, node.Transform<IntFormatTest>());
        }

        [Test]
        public void TransformConcatenating()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);

            node.Transform<IntFormatTest>().Transform<StringFormatTest>();
            Assert.IsInstanceOf<StringFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual("3", (node.Format as StringFormatTest).Value);
        }

        [Test]
        public void TransformDisposeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            node.Transform<IntFormatTest>();
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.Format.Disposed);
        }

        [Test]
        public void TransformNotDisposingFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            node.Transform<IntFormatTest>(false);
            Assert.IsFalse(dummyFormat.Disposed);
            Assert.IsFalse(node.Format.Disposed);
        }

        [Test]
        public void TransformWithConverterChangeFormat()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);

            node.Transform<IntFormatTest>(converter: converter);
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(4, (node.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformWithAndTypeConverterChangeFormat()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);

            node.Transform(typeof(IntFormatTest), converter: converter);
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(4, (node.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformWithConverterConcatenating()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new IntFormatTest(3);
            Node node = new Node("mytest", dummyFormat);

            node.Transform<StringFormatTest>()
                .Transform<IntFormatTest>(converter: converter);
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(4, (node.Format as IntFormatTest).Value);
        }

        [Test]
        public void SetFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            Node node = new Node("mytest", dummyFormat1);
            node.Format = dummyFormat2;
            Assert.AreNotSame(node.Format, dummyFormat1);
            Assert.AreSame(node.Format, dummyFormat2);
        }

        [Test]
        public void SetFormatWithoutPreviousFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest");
            node.Format = dummyFormat;
            Assert.AreSame(node.Format, dummyFormat);
        }

        [Test]
        public void SetFormatDoesNotDisposePreviousFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            Node node = new Node("mytest", dummyFormat1);
            node.Format = dummyFormat2;
            Assert.IsFalse(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void SetFormatToNull()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            node.Format = null;
            Assert.IsNull(node.Format);
        }

        [Test]
        public void DisposeDoesDisposeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            Assert.IsFalse(node.Disposed);
            Assert.IsFalse(dummyFormat.Disposed);
            node.Dispose();
            Assert.IsTrue(node.Disposed);
            Assert.IsTrue(dummyFormat.Disposed);
        }

        [Test]
        public void SetContainerFormatAddChildren()
        {
            Node node = new Node("MyTest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Add(new Node("Child"));
            node.Format = format;

            Assert.AreEqual(1, node.Children.Count);
            Assert.AreSame(format.Children[0], node.Children[0]);
        }

        [Test]
        public void SetFromContainerToDifferentCleanChildren()
        {
            Node node = new Node("mytest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Add(new Node("Child"));
            node.Format = format;
            Assert.IsNotEmpty(node.Children);

            StringFormatTest newFormat = new StringFormatTest("3");
            node.Format = newFormat;
            Assert.IsEmpty(node.Children);
        }

        public class PrivateConverter : 
            IConverter<StringFormatTest, IntFormatTest>,
            IConverter<IntFormatTest, StringFormatTest>
        {
            public IntFormatTest Convert(StringFormatTest test)
            {
                return new IntFormatTest(System.Convert.ToInt32(test.Value) + 1);
            }

            public StringFormatTest Convert(IntFormatTest test)
            {
                return new StringFormatTest(test.Value.ToString());
            }
        }
    }
}
