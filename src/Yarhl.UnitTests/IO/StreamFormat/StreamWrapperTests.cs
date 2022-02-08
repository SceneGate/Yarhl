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
namespace Yarhl.UnitTests.IO.StreamFormat
{
    using System;
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class StreamWrapperTests
    {
        [Test]
        public void ConstructorSetProperties()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);
            Assert.That(wrapper.BaseStream, Is.SameAs(stream));
        }

        [Test]
        public void FeaturePropertiesFromBaseStream()
        {
            var stream = new MemoryStream(new byte[2], writable: false);
            using var wrapper = new StreamWrapperImpl(stream);
            Assert.That(wrapper.CanRead, Is.True);
            Assert.That(wrapper.CanWrite, Is.False);
            Assert.That(wrapper.CanSeek, Is.True);
            Assert.That(wrapper.CanTimeout, Is.False);
        }

        [Test]
        public void LengthGetterIsSyncWithStream()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0x11);
            using var wrapper = new StreamWrapperImpl(stream);
            Assert.That(wrapper.Length, Is.EqualTo(1));

            stream.WriteByte(0xAA);
            Assert.That(wrapper.Length, Is.EqualTo(2));
        }

        [Test]
        public void DisposeCloseStream()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0xAA);
            var wrapper = new StreamWrapperImpl(stream);

            wrapper.Dispose();

            Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
        }

        [Test]
        public void DisposeTwiceDoesNotThrowException()
        {
            var stream = new MemoryStream();
            var wrapper = new StreamWrapperImpl(stream);

            wrapper.Dispose();
            Assert.DoesNotThrow(wrapper.Dispose);
        }

        [Test]
        public void DisposeChangesDisposed()
        {
            var stream = new MemoryStream();
            var wrapper = new StreamWrapperImpl(stream);
            Assert.IsFalse(wrapper.Disposed);
            wrapper.Dispose();
            Assert.IsTrue(wrapper.Disposed);
        }

        [Test]
        public void SetLengthChangesStreamLength()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);
            Assert.That(wrapper.Length, Is.EqualTo(0));
            Assert.That(stream.Length, Is.EqualTo(0));

            wrapper.SetLength(10);

            Assert.That(wrapper.Length, Is.EqualTo(10));
            Assert.That(stream.Length, Is.EqualTo(10));
        }

        [Test]
        public void ReadByteReadsAndAdvance()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);
            stream.WriteByte(0x11);
            stream.WriteByte(0x12);
            stream.WriteByte(0x13);
            Assert.That(wrapper.Position, Is.EqualTo(3));

            stream.Position = 0;
            Assert.That(wrapper.ReadByte(), Is.EqualTo(0x11));
            Assert.That(wrapper.Position, Is.EqualTo(1));

            wrapper.Position = 2;
            Assert.That(wrapper.ReadByte(), Is.EqualTo(0x13));
            Assert.That(wrapper.Position, Is.EqualTo(0x03));
        }

        [Test]
        public void ReadArrayReadsAndAdvance()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);
            stream.WriteByte(0x11);
            stream.WriteByte(0x12);
            stream.WriteByte(0x13);
            stream.WriteByte(0x14);
            Assert.That(wrapper.Position, Is.EqualTo(4));

            byte[] buffer = new byte[10];
            buffer[2] = 0xCA;
            wrapper.Position = 0;
            wrapper.Read(buffer, 0, 2);
            Assert.That(buffer[0], Is.EqualTo(0x11));
            Assert.That(buffer[1], Is.EqualTo(0x12));
            Assert.That(buffer[2], Is.EqualTo(0xCA));
            Assert.That(wrapper.Position, Is.EqualTo(2));

            wrapper.Position = 3;
            wrapper.Read(buffer, 9, 1);
            Assert.That(buffer[9], Is.EqualTo(0x14));
            Assert.That(buffer[0], Is.EqualTo(0x11));
            Assert.That(wrapper.Position, Is.EqualTo(4));
        }

        [Test]
        public void WriteByteWritesAndAdvance()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);

            wrapper.WriteByte(0x11);
            Assert.That(wrapper.Position, Is.EqualTo(1));
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x11));

            wrapper.WriteByte(0x12);
            Assert.That(wrapper.Position, Is.EqualTo(2));
            stream.Position = 1;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x12));

            wrapper.Position = 0;
            wrapper.WriteByte(0x13);
            Assert.That(wrapper.Position, Is.EqualTo(1));
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x13));
        }

        [Test]
        public void WriteArrayWritesAndAdvance()
        {
            var stream = new MemoryStream();
            using var wrapper = new StreamWrapperImpl(stream);

            byte[] buffer = new byte[] { 0x11, 0x12, 0x13, 0xFF, 0x14 };
            wrapper.Write(buffer, 0, 2);
            Assert.That(wrapper.Position, Is.EqualTo(2));
            Assert.That(wrapper.Length, Is.EqualTo(2));
            Assert.That(stream.Length, Is.EqualTo(2));
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x11));
            Assert.That(stream.ReadByte(), Is.EqualTo(0x12));

            wrapper.Position = 1;
            wrapper.Write(buffer, 4, 1);
            Assert.That(wrapper.Position, Is.EqualTo(2));
            Assert.That(wrapper.Length, Is.EqualTo(2));
            Assert.That(stream.Length, Is.EqualTo(2));
            stream.Position = 1;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x14));
        }

        [Test]
        public void PublicMethodThrowAfterDispose()
        {
            var stream = new MemoryStream();
            var wrapper = new StreamWrapperImpl(stream);
            wrapper.Dispose();

            byte[] buffer = new byte[1];
            Assert.That(
                () => wrapper.SetLength(10),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => wrapper.WriteByte(0x00),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => wrapper.Write(buffer, 0, 1),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => wrapper.ReadByte(),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => wrapper.Read(buffer, 0, 1),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => wrapper.Seek(1, SeekOrigin.Begin),
                Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void SeekChangesPosition()
        {
            using var wrapper = new StreamWrapperImpl(new MemoryStream());
            wrapper.WriteByte(0xCA);
            wrapper.WriteByte(0xFE);
            Assert.That(wrapper.Position, Is.EqualTo(2));

            wrapper.Seek(0, SeekOrigin.Begin);
            Assert.That(wrapper.Position, Is.EqualTo(0));

            wrapper.Seek(0, SeekOrigin.End);
            Assert.That(wrapper.Position, Is.EqualTo(2));

            wrapper.Seek(-1, SeekOrigin.Current);
            Assert.That(wrapper.Position, Is.EqualTo(1));
        }

        [Test]
        public void TestFlushCallsStreamFlush()
        {
            var innerStream = new Mock<Stream>();
            using var stream = new StreamWrapperImpl(innerStream.Object);
            stream.Flush();
            Assert.That(
                () => innerStream.Verify(s => s.Flush(), Times.Once),
                Throws.Nothing);
        }

        [Test]
        public void TestFlushThrowsIfDisposed()
        {
            var innerStream = new Mock<Stream>();
            var stream = new StreamWrapperImpl(innerStream.Object);
            stream.Dispose();
            Assert.That(
                () => stream.Flush(),
                Throws.InstanceOf<ObjectDisposedException>());
        }

        private sealed class StreamWrapperImpl : StreamWrapper
        {
            public StreamWrapperImpl(Stream stream)
                : base(stream)
            {
            }
        }
    }
}
