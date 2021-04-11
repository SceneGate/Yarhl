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
    using System.Composition;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.UnitTests.FileFormat;

    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void ConstructorSetName()
        {
            using Node node = new Node("mytest");
            Assert.AreEqual("mytest", node.Name);
        }

        [Test]
        public void ConstructorSetNameAndFormat()
        {
            var dummyFormat = new StringFormatTest(string.Empty);
            using Node node = new Node("mytest", dummyFormat);
            Assert.AreEqual("mytest", node.Name);
            Assert.AreSame(dummyFormat, node.Format);
        }

        [Test]
        public void ConstructorWithoutFormatNullProperty()
        {
            using Node node = new Node("mytest");
            Assert.IsNull(node.Format);
        }

        [Test]
        public void ConstructorAllowsNullFormat()
        {
            using Node node = new Node("mytest", null);
            Assert.That(node.Format, Is.Null);
        }

        [Test]
        public void GetStreamWhenBinaryFormatReturnsStream()
        {
            BinaryFormat format = new BinaryFormat();
            using Node node = new Node("myteset", format);
            Assert.AreEqual(format.Stream, node.Stream);
            node.Dispose();
        }

        [Test]
        public void GetStreamIfFormatIsNotBinaryFormatReturnsNull()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            Assert.IsNull(node.Stream);
        }

        [Test]
        public void GetStreamWithoutFormatReturnsNull()
        {
            using Node node = new Node("mytest");
            Assert.IsNull(node.Stream);
        }

        [Test]
        public void IsContainerIfFormatNodeContainer()
        {
            NodeContainerFormat format = new NodeContainerFormat();
            using Node node = new Node("NodeTest", format);
            Assert.IsTrue(node.IsContainer);
        }

        [Test]
        public void IsNotContainerForDifferentFormat()
        {
            StringFormatTest format = new StringFormatTest("3");
            using Node node = new Node("NodeTest", format);
            Assert.IsFalse(node.IsContainer);
        }

        [Test]
        public void IsNotContainerIfNoFormat()
        {
            using Node node = new Node("mytest");
            Assert.That(node.IsContainer, Is.False);
        }

        [Test]
        public void IsContainerIfFormatDerivedFromNodeContainer()
        {
            MyContainer format = new MyContainer();
            using Node node = new Node("MyTest", format);
            Assert.IsTrue(node.IsContainer);
        }

        [Test]
        public void GetFormatAs()
        {
            StringFormatTest format = new StringFormatTest("3");
            using Node node = new Node("NodeTest", format);
            Assert.AreSame(node.Format, node.GetFormatAs<StringFormatTest>());
        }

        [Test]
        public void GetFormatAsReturnNullIfNoCastingPossible()
        {
            StringFormatTest format = new StringFormatTest("3");
            using Node node = new Node("NodeTest", format);
            Assert.IsNull(node.GetFormatAs<IntFormatTest>());
        }

        [Test]
        public void GetFormatAsAfterDisposeThrowsException()
        {
            using StringFormatTest format = new StringFormatTest("3");
            Node node = new Node("NodeTest", format);
            node.Dispose();
            Assert.That(
                node.GetFormatAs<StringFormatTest>,
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GetFormatReturnNullIfNoFormat()
        {
            using Node node = new Node("test");
            Assert.That(node.GetFormatAs<StringFormatTest>(), Is.Null);
        }

        [Test]
        public void ChangeFormat()
        {
            var dummyFormat1 = new StringFormatTest("3");
            var dummyFormat2 = new IntFormatTest(4);
            using Node node = new Node("mytest", dummyFormat1);
            node.ChangeFormat(dummyFormat2);
            Assert.AreNotSame(node.Format, dummyFormat1);
            Assert.AreSame(node.Format, dummyFormat2);
        }

        [Test]
        public void ChangeFormatWithoutPreviousFormat()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest");
            node.ChangeFormat(dummyFormat);
            Assert.AreSame(node.Format, dummyFormat);
        }

        [Test]
        public void ChangeFormatDisposePreviousFormat()
        {
            var dummyFormat1 = new StringFormatTest("3");
            var dummyFormat2 = new IntFormatTest(4);
            using Node node = new Node("mytest", dummyFormat1);
            node.ChangeFormat(dummyFormat2);
            Assert.IsTrue(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void ChangeFormatDoesNothingForSameFormat()
        {
            var dummyFormat1 = new StringFormatTest("3");
            var dummyFormat2 = dummyFormat1;
            using Node node = new Node("mytest", dummyFormat1);
            node.ChangeFormat(dummyFormat2);
            Assert.AreEqual(dummyFormat2, node.Format);
            Assert.IsFalse(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void ChangeFormatWithoutDisposingPreviousFormat()
        {
            var dummyFormat1 = new StringFormatTest("3");
            var dummyFormat2 = new IntFormatTest(4);
            using Node node = new Node("mytest", dummyFormat1);
            node.ChangeFormat(dummyFormat2, false);
            Assert.IsFalse(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void ChangeFormatToNull()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            node.ChangeFormat(null);
            Assert.IsNull(node.Format);
        }

        [Test]
        public void SetContainerFormatAddChildren()
        {
            using Node node = new Node("MyTest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));
            node.ChangeFormat(format);

            Assert.AreEqual(1, node.Children.Count);
            Assert.AreSame(format.Root.Children[0], node.Children[0]);
            Assert.AreEqual("/MyTest/Child", node.Children[0].Path);
            Assert.AreSame(node, node.Children[0].Parent);
        }

        [Test]
        public void SetContainerFormatTransferHandlingToNode()
        {
            using Node node = new Node("MyTest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));
            node.ChangeFormat(format);

            // It must be the same parent
            Assert.AreSame(node, format.Root);
            format.Dispose();
            Assert.IsFalse(node.Disposed);
            Assert.IsNotEmpty(node.Children);
            Assert.IsFalse(node.Children[0].Disposed);
        }

        [Test]
        public void SetFromContainerToDifferentCleanChildren()
        {
            using Node node = new Node("mytest");
            NodeContainerFormat format = new NodeContainerFormat();
            format.Root.Add(new Node("Child"));
            node.ChangeFormat(format);
            Assert.IsNotEmpty(node.Children);

            StringFormatTest newFormat = new StringFormatTest("3");
            node.ChangeFormat(newFormat);
            Assert.IsEmpty(node.Children);
        }

        [Test]
        public void SetFormatThrowExceptionIfDisposed()
        {
            using Node node = new Node("MyTest");
            node.Dispose();
            StringFormatTest format = new StringFormatTest("3");
            Assert.Throws<ObjectDisposedException>(() => node.ChangeFormat(format));
        }

        [Test]
        public void TransformToGeneric()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            node.TransformTo<IntFormatTest>();
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(3, node.GetFormatAs<IntFormatTest>().Value);
        }

        [Test]
        public void TransformToGenericReturnNode()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            Assert.That(node.TransformTo<IntFormatTest>(), Is.SameAs(node));
        }

        [Test]
        public void TransformToWithType()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            var result = node.TransformTo(typeof(IntFormatTest));
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(3, node.GetFormatAs<IntFormatTest>().Value);
            Assert.AreSame(node, result);
        }

        [Test]
        public void TransformToWithTypeReturnNode()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            Assert.That(node.TransformTo(typeof(IntFormatTest)), Is.SameAs(node));
        }

        [Test]
        public void TransformToWithTypeNullThrowsException()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(() => node.TransformTo(null), Throws.ArgumentNullException);
        }

        [Test]
        public void TransformToWithTypeThrowsIfConverterDoesNotReturnIFormat()
        {
            var dummy = new StringFormatTest("3");
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformTo(typeof(NoFormat)),
                Throws.InvalidOperationException);
        }

        [Test]
        public void TransformToWithTypeDoesNotThrowIfReturnsNull()
        {
            var dummy = new NullSource();
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformTo(typeof(NullDestination)),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);
        }

        [Test]
        public void TransformToGenericDisposeFormat()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            node.TransformTo<IntFormatTest>();
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.GetFormatAs<IntFormatTest>().Disposed);
            node.Dispose();
        }

        [Test]
        public void TransformToWithTypeDisposeFormat()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            node.TransformTo(typeof(IntFormatTest));
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(node.GetFormatAs<IntFormatTest>().Disposed);
            node.Dispose();
        }

        [Test]
        public void TransformWithGeneric()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            node.TransformWith<PrivateConverter>();
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(4, node.GetFormatAs<IntFormatTest>().Value);
        }

        [Test]
        public void TransformWithGenericReturnsNode()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(node.TransformWith<PrivateConverter>(), Is.SameAs(node));
        }

        [Test]
        public void TransformWithGenericThrowsIfConverterDoesNotReturnIFormat()
        {
            var dummy = new StringFormatTest("3");
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith<StringFormatTest2NoFormat>(),
                Throws.InvalidOperationException);
        }

        [Test]
        public void TransformWithGenericDoesNotThrowIfReturnsNull()
        {
            var dummy = new NullSource();
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith<NullConverter>(),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);
        }

        [Test]
        public void TransformWithInit()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            node.TransformWith<PrivateConverter, int>(4);
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(7, node.GetFormatAs<IntFormatTest>().Value);
        }

        [Test]
        public void TransformWithInitReturnsNode()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(
                node.TransformWith<PrivateConverter, int>(4),
                Is.SameAs(node));
        }

        [Test]
        public void TransformWithInitThrowsIfConverterDoesNotReturnIFormat()
        {
            var dummy = new StringFormatTest("3");
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith<StringFormatTest2NoFormat, int>(2),
                Throws.InvalidOperationException);
        }

        [Test]
        public void TransformWithInitDoesNotThrowIfReturnsNull()
        {
            var dummy = new NullSource();
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith<NullConverter, int>(2),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);
        }

        [Test]
        public void TransformWithType()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            node.TransformWith(typeof(StringFormatTest2IntFormatTestConverter));
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(3, node.GetFormatAs<IntFormatTest>().Value);
        }

        [Test]
        public void TransformWithTypeReturnsNode()
        {
            var dummyFormat = new IntFormatTest();
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(
                node.TransformWith(typeof(StringFormatTest2IntFormatTestConverter)),
                Is.SameAs(node));
        }

        [Test]
        public void TransformWithPrivateTypeThrowsException()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(
                () => node.TransformWith(typeof(PrivateConverter)),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Cannot find converter " +
                    typeof(PrivateConverter).FullName));
        }

        [Test]
        public void TransformWithTypeThrowsIfNull()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Type myType = null;
            Assert.That(
                () => node.TransformWith(myType),
                Throws.ArgumentNullException);
        }

        [Test]
        public void TransformWithTypeThrowsIfConverterDoesNotReturnIFormat()
        {
            var dummy = new StringFormatTest("3");
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith(typeof(StringFormatTest2NoFormat)),
                Throws.InvalidOperationException);
        }

        [Test]
        public void TransformWithTypeDoesNotThrowIfReturnsNull()
        {
            var dummy = new NullSource();
            using Node node = new Node("mytest", dummy);

            Assert.That(
                () => node.TransformWith(typeof(NullConverter)),
                Throws.Nothing);
            Assert.That(node.Format, Is.Null);
        }

        [Test]
        public void TransformWithThrowsIfNoConverterImplementation()
        {
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            var msg = "Converter doesn't implement IConverter<,>";
            Assert.That(
                () => node.TransformWith<ConverterWithoutGenericInterface>(),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => node.TransformWith<ConverterWithoutGenericInterface, int>(0),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            // It won't be discovered
            Assert.That(
                () => node.TransformWith(typeof(ConverterWithoutGenericInterface)),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Cannot find converter " +
                    typeof(ConverterWithoutGenericInterface).FullName));
        }

        [Test]
        public void TransformWithInstance()
        {
            PrivateConverter converter = new PrivateConverter();
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            node.TransformWith<StringFormatTest, IntFormatTest>(converter);
            Assert.IsInstanceOf<IntFormatTest>(node.Format);
            Assert.AreNotSame(dummyFormat, node.Format);
            Assert.AreEqual(4, node.GetFormatAs<IntFormatTest>().Value);
        }

        [Test]
        public void TransformWithInstanceReturnsNode()
        {
            PrivateConverter converter = new PrivateConverter();
            var dummyFormat = new IntFormatTest(3);
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(
                node.TransformWith<IntFormatTest, StringFormatTest>(converter),
                Is.SameAs(node));
        }

        [Test]
        public void TransformWithInstanceThrowsExceptionIfSelectWrongConverter()
        {
            PrivateConverter converter = new PrivateConverter();
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);

            Assert.That(
                () => node.TransformWith<IntFormatTest, StringFormatTest>(converter),
                Throws.InstanceOf<InvalidCastException>());
        }

        [Test]
        public void TransformWithInstanceThrowsIfConverterNull()
        {
            PrivateConverter converter = null;
            var dummyFormat = new StringFormatTest("3");
            using Node node = new Node("mytest", dummyFormat);
            Assert.That(
                () => node.TransformWith<StringFormatTest, IntFormatTest>(converter),
                Throws.ArgumentNullException);
        }

        [Test]
        public void TransformAfterDisposeThrowsException()
        {
            using var dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            node.Dispose();

            Assert.That(
                node.TransformTo<IntFormatTest>,
                Throws.TypeOf<ObjectDisposedException>());
            Assert.That(
                () => node.TransformTo(typeof(IntFormatTest)),
                Throws.TypeOf<ObjectDisposedException>());
            Assert.That(
                () => node.TransformWith<PrivateConverter>(),
                Throws.TypeOf<ObjectDisposedException>());
            Assert.That(
                () => node.TransformWith<PrivateConverter, int>(0),
                Throws.TypeOf<ObjectDisposedException>());
            Assert.That(
                () => node.TransformWith(typeof(PrivateConverter)),
                Throws.TypeOf<ObjectDisposedException>());

            var converter = new PrivateConverter();
            Assert.That(
                () => node.TransformWith<StringFormatTest, IntFormatTest>(converter),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void TransformWithoutFormatThrowException()
        {
            string msg = "Cannot transform a node without format";
            using Node node = new Node("mytest");
            Assert.That(
                node.TransformTo<IntFormatTest>,
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => node.TransformTo(typeof(IntFormatTest)),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => node.TransformWith<PrivateConverter>(),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => node.TransformWith<PrivateConverter, int>(0),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => node.TransformWith(typeof(PrivateConverter)),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            var converter = new PrivateConverter();
            Assert.That(
                () => node.TransformWith<StringFormatTest, IntFormatTest>(converter),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
        }

        [Test]
        public void DisposeDoesDisposeFormat()
        {
            using var dummyFormat = new StringFormatTest("3");
            Node node = new Node("mytest", dummyFormat);
            Assert.IsFalse(node.Disposed);
            Assert.IsFalse(dummyFormat.Disposed);
            node.Dispose();
            Assert.IsTrue(node.Disposed);
            Assert.IsTrue(dummyFormat.Disposed);
        }

        [Test]
        public void DisposeTwiceDoesNotThrow()
        {
            using Node node = new Node("mytest", new StringFormatTest("3"));
            node.Dispose();
            Assert.DoesNotThrow(node.Dispose);
        }

        [Test]
        public void CloneNullNodeThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new Node((Node)null));
        }

        [Test]
        public void NotCloneableFormatThrowsException()
        {
            using Node node = new Node("test", new StringFormatTest("3"));
            Assert.Throws<InvalidOperationException>(() => _ = new Node(node));
        }

        [Test]
        public void CloneNodeHasTags()
        {
            using Node node = NodeFactory.FromMemory("test");
            node.Tags["TestTag"] = 23;

            using Node clone = new Node(node);

            Assert.AreNotSame(node, clone);
            Assert.AreEqual(1, clone.Tags.Count);
            Assert.AreEqual(23, clone.Tags["TestTag"]);
        }

        [Test]
        public void CloneNullFormatNode()
        {
            using Node node = new Node("test");
            using Node clone = new Node(node);

            Assert.AreNotSame(node, clone);
            Assert.IsNull(clone.Format);
        }

        [PartNotDiscoverable]
        public class PrivateConverter :
            IConverter<StringFormatTest, IntFormatTest>,
            IConverter<IntFormatTest, StringFormatTest>,
            IInitializer<int>
        {
            int offset;

            public PrivateConverter()
            {
                offset = 1;
            }

            public void Initialize(int parameters)
            {
                offset = parameters;
            }

            public IntFormatTest Convert(StringFormatTest source)
            {
                return new IntFormatTest(System.Convert.ToInt32(source.Value) + offset);
            }

            public StringFormatTest Convert(IntFormatTest source)
            {
                return new StringFormatTest(source.Value.ToString());
            }
        }

        class MyContainer : NodeContainerFormat
        {
        }
    }
}
