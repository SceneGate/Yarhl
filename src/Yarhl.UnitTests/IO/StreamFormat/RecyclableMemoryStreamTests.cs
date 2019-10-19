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
    using NUnit.Framework;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class RecyclableMemoryStreamTests
    {
        [Test]
        public void ConstructorCreatesAMemoryStream()
        {
            var stream = new RecyclableMemoryStream();
            Assert.That(
                stream.BaseStream,
                Is.TypeOf<Microsoft.IO.RecyclableMemoryStream>());
            stream.Dispose();
        }

        [Test]
        public void CanIncreaseLength()
        {
            using var stream = new RecyclableMemoryStream();
            Assert.That(() => stream.SetLength(4), Throws.Nothing);
            Assert.That(stream.Length, Is.EqualTo(4));
        }

        [Test]
        public void UpdatedLongBufferIsClean()
        {
            var stream = new RecyclableMemoryStream();
            stream.WriteByte(0x42);
            stream.SetLength(11);
            Assert.That(stream.Length, Is.EqualTo(11));

            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0x42));
            for (int i = 0; i < 10; i++) {
                Assert.That(stream.ReadByte(), Is.EqualTo(0));
            }

            stream.SetLength(80 * 1024);
            stream.Position = 1;
            for (int i = 1; i < stream.Length; i++) {
                Assert.That(stream.ReadByte(), Is.EqualTo(0));
            }

            stream.Dispose();
        }

        [Test]
        public void CanDecreaseLength()
        {
            using var stream = new RecyclableMemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            Assert.That(stream.Length, Is.EqualTo(2));
            Assert.That(stream.Position, Is.EqualTo(2));

            stream.SetLength(1);

            Assert.That(stream.Length, Is.EqualTo(1));
            Assert.That(stream.Position, Is.EqualTo(1));
        }

        [Test]
        public void PublicMethodThrowAfterDispose()
        {
            var stream = new RecyclableMemoryStream();
            stream.Dispose();

            byte[] buffer = new byte[1];
            Assert.That(
                () => stream.SetLength(10),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => stream.WriteByte(0x00),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => stream.Write(buffer, 0, 1),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => stream.ReadByte(),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => stream.Read(buffer, 0, 1),
                Throws.InstanceOf<ObjectDisposedException>());
        }
    }
}