// BinaryFormatTests.cs
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
namespace Yarhl.UnitTests.IO
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Yarhl.IO;
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
        public void DisposeTWicheDoesNotThrow()
        {
            BinaryFormat format = CreateDummyFormat();
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
            Assert.That(() => format.Dispose(), Throws.Nothing);
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void ConstructorWithStream()
        {
            DataStream stream = new DataStream();
            BinaryFormat format = new BinaryFormat(stream);
            Assert.AreNotSame(stream, format.Stream);
            Assert.AreSame(stream.BaseStream, format.Stream.BaseStream);
            format.Dispose();
        }

        [Test]
        public void ConstructorWithNullStreamThrows()
        {
            Assert.That(
                () => new BinaryFormat((DataStream)null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConstructorWithStreamArgs()
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
        public void ConstructorWithStreamArgsInvalidThrows()
        {
            DataStream stream = new DataStream();
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
            Assert.IsInstanceOf<MemoryStream>(format.Stream.BaseStream);
            Assert.AreEqual(0, format.Stream.Position);
            Assert.AreEqual(0, format.Stream.Length);
            format.Dispose();
        }

        [Test]
        public void ConstructorWithPathAllowReadWrite()
        {
            string tempPath = Path.GetTempFileName();
            BinaryFormat format = new BinaryFormat(tempPath);

            Assert.DoesNotThrow(() => format.Stream.WriteByte(0xAE));
            format.Stream.Seek(0, SeekMode.Start);
            Assert.AreEqual(0xAE, format.Stream.ReadByte());

            format.Dispose();
            File.Delete(tempPath);
        }

        [Test]
        public void ConstructorWithPathCreatesFile()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            BinaryFormat format = new BinaryFormat(tempPath);
            Assert.IsTrue(File.Exists(tempPath));

            format.Dispose();
            File.Delete(tempPath);
        }

        [Test]
        public void ConstructorWithNullPathThrows()
        {
            Assert.That(
                () => new BinaryFormat((string)null),
                Throws.ArgumentNullException);
            Assert.That(
                () => new BinaryFormat(string.Empty),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConstructorWithArray()
        {
            byte[] data = new byte[] { 0x01, 0x2, 0x3 };

            BinaryFormat format = new BinaryFormat(data, 1, 2);
            Assert.IsInstanceOf<MemoryStream>(format.Stream.BaseStream);
            Assert.AreEqual(0, format.Stream.Position);
            Assert.AreEqual(1, format.Stream.Offset);
            Assert.AreEqual(2, format.Stream.Length);
            format.Dispose();
        }

        [Test]
        public void ConstructorFromArrayWithInvalidThrows()
        {
            byte[] data = new byte[] { 0x01 };

            Assert.That(
                () => new BinaryFormat((byte[])null, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => new BinaryFormat(data, -1, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(data, 2, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(data, 0, -1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(data, 0, 2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => new BinaryFormat(data, 1, 1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void DisposeIsDisposingFormat()
        {
            string tempPath = Path.GetTempFileName();
            BinaryFormat format = new BinaryFormat(tempPath);
            format.Dispose();
            Assert.DoesNotThrow(() => File.Delete(tempPath));
        }

        [Test]
        public void DisposeDoesNotAffectToOtherFormatOrStreams()
        {
            DataStream baseStream = new DataStream();
            BinaryFormat format1 = new BinaryFormat(baseStream);
            BinaryFormat format2 = new BinaryFormat(baseStream);

            format1.Dispose();
            Assert.IsTrue(format1.Disposed);
            Assert.IsFalse(format2.Disposed);
            Assert.IsFalse(baseStream.Disposed);

            format2.Dispose();
            Assert.IsTrue(format2.Disposed);
            Assert.IsFalse(baseStream.Disposed);

            baseStream.Dispose();
            Assert.IsTrue(baseStream.Disposed);
        }

        protected override BinaryFormat CreateDummyFormat()
        {
            DataStream stream = new DataStream(new MemoryStream(), 0, 0);
            return new BinaryFormat(stream);
        }
    }
}
