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
namespace Yarhl.UnitTests.IO
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Yarhl.IO;
    using Yarhl.IO.StreamFormat;
    using Yarhl.UnitTests.FileFormat;

    [TestFixture]
    public class BinaryFormatTests : BaseGeneralTests<BinaryFormat>
    {
        [Test]
        public void DisposeChangesDisposed()
        {
            BinaryFormat format = CreateDummyFormat();
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void DisposeTwiceDoesNotThrow()
        {
            BinaryFormat format = CreateDummyFormat();
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
            Assert.That(() => format.Dispose(), Throws.Nothing);
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void DefaultConstructorDisposeAlsoDisposesStream()
        {
            BinaryFormat format = CreateDummyFormat();
            format.Dispose();
            Assert.IsTrue(format.Disposed);
            Assert.IsTrue(format.Stream.Disposed);
        }

        [Test]
        public void ConstructorWithStreamTransferOwnership()
        {
            using DataStream stream = new DataStream();
            BinaryFormat format = new BinaryFormat(stream);
            Assert.AreSame(stream, format.Stream);
            Assert.AreSame(stream.BaseStream, format.Stream.BaseStream);
            format.Dispose();
            Assert.That(stream.Disposed, Is.True);
        }

        [Test]
        public void ConstructorWithStandardStreamTransferOwnership()
        {
            var stream = new MemoryStream();
            var format = new BinaryFormat(stream);

            Assert.That(format.Stream.BaseStream, Is.SameAs(stream));

            format.Dispose();
            Assert.That(() => stream.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void ConstructorWithNullStreamThrows()
        {
            Assert.That(
                () => new BinaryFormat((DataStream)null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConstructorWithSubstream()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0x1);
            stream.WriteByte(0x2);
            stream.WriteByte(0x3);

            BinaryFormat format = new BinaryFormat(stream, 1, 2);
            Assert.AreNotSame(stream, format.Stream);
            Assert.AreSame(stream.BaseStream, format.Stream.BaseStream);
            Assert.AreEqual(0, format.Stream.Position);
            Assert.AreEqual(3, stream.Position);
            Assert.AreEqual(1, format.Stream.Offset);
            Assert.AreEqual(0, stream.Offset);
            Assert.AreEqual(2, format.Stream.Length);
            Assert.AreEqual(3, stream.Length);
            format.Dispose();
            stream.Dispose();
        }

        [Test]
        public void ConstructorWithSubstreamOfDataStreamDoesNotTransferOwnership()
        {
            DataStream stream = new DataStream();
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);

            BinaryFormat format = new BinaryFormat(stream, 1, 2);
            Assert.AreNotSame(stream, format.Stream);
            Assert.AreSame(stream.BaseStream, format.Stream.BaseStream);

            format.Dispose();
            Assert.That(format.Disposed, Is.True);
            Assert.That(stream.Disposed, Is.False);
            stream.Dispose();
        }

        [Test]
        public void ConstructorWithStandardSubstreamTransferOwnership()
        {
            var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3 }, 0, 3);

            BinaryFormat format = new BinaryFormat(stream, 1, 2);

            Assert.That(format.Stream.BaseStream, Is.SameAs(stream));

            format.Dispose();
            Assert.That(() => stream.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void ConstructorWithStreamArgsInvalidThrows()
        {
            using DataStream stream = new DataStream();
            stream.WriteByte(0x01);

            Assert.That(
                () => new BinaryFormat((DataStream)null, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => new BinaryFormat(stream, -1, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(stream, 2, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(stream, 0, -1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(stream, 0, 2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(stream, 1, 1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void MemoryConstructor()
        {
            BinaryFormat format = new BinaryFormat();
            Assert.IsInstanceOf<RecyclableMemoryStream>(format.Stream.BaseStream);
            Assert.AreEqual(0, format.Stream.Position);
            Assert.AreEqual(0, format.Stream.Length);
            format.Dispose();
        }

        [Test]
        public void Clone()
        {
            byte[] data = { 0x01, 0x02, 0x03 };
            using DataStream stream = DataStreamFactory.FromArray(data, 0, data.Length);
            using BinaryFormat format = new BinaryFormat(stream);

            using BinaryFormat clone = (BinaryFormat)format.DeepClone();

            Assert.AreNotSame(format, clone);
            Assert.IsTrue(format.Stream.Compare(clone.Stream));
        }

        protected override BinaryFormat CreateDummyFormat()
        {
            using DataStream stream = new DataStream();
            return new BinaryFormat(stream);
        }
    }
}
