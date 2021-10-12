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
    using NUnit.Framework;
    using Yarhl.IO;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class LazyFileStreamTests
    {
        string tempFile;

        [SetUp]
        public void SetUp()
        {
            tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(tempFile);
        }

        [Test]
        public void ConstructorDoesNotOpenStream()
        {
            using var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite);
            Assert.That(stream.BaseStream, Is.Null);
        }

        [Test]
        public void ConstructorSetFileLengthOrZero()
        {
            using var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite);
            Assert.That(stream.Length, Is.EqualTo(0));
            Assert.That(stream.BaseStream, Is.Null);
            Assert.That(File.Exists(tempFile), Is.False);

            using (var fs = new FileStream(tempFile, FileMode.CreateNew)) {
                fs.WriteByte(0xAA);
            }

            using (var stream2 = new LazyFileStream(tempFile, FileOpenMode.ReadWrite)) {
                Assert.That(stream2.Length, Is.EqualTo(1));
                Assert.That(stream2.BaseStream, Is.Null);
            }
        }

        [Test]
        public void GetPositionWithoutInitializeReturnsZero()
        {
            using var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite);
            Assert.That(stream.Position, Is.EqualTo(0));
        }

        [Test]
        public void GetPositionAfterInitialize()
        {
            using var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite);
            stream.WriteByte(0xCA);
            Assert.That(stream.Position, Is.EqualTo(1));
        }

        [Test]
        public void SetLengthOpensFile()
        {
            using (var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite)) {
                stream.SetLength(1);
                Assert.That(stream.BaseStream, Is.Not.Null);
                Assert.That(File.Exists(tempFile), Is.True);
                Assert.That(stream.Length, Is.EqualTo(1));
                Assert.That(stream.BaseStream.Length, Is.EqualTo(1));
            }
        }

        [Test]
        public void LengthSyncWithStream()
        {
            using (var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite)) {
                stream.SetLength(1);
                Assert.That(stream.Length, Is.EqualTo(1));
                stream.BaseStream.Position = 1;
                stream.BaseStream.WriteByte(0xAA);
                Assert.That(stream.Length, Is.EqualTo(2));
            }
        }

        [Test]
        public void ReadOpensFile()
        {
            using (var fs = new FileStream(tempFile, FileMode.CreateNew)) {
                fs.WriteByte(0x42);
                fs.WriteByte(0xAA);
            }

            using (var stream = new LazyFileStream(tempFile, FileOpenMode.Read)) {
                Assert.That(stream.BaseStream, Is.Null);
                Assert.That(stream.ReadByte(), Is.EqualTo(0x42));
                Assert.That(stream.BaseStream, Is.Not.Null);
            }

            using (var stream = new LazyFileStream(tempFile, FileOpenMode.Read)) {
                Assert.That(stream.BaseStream, Is.Null);
                byte[] buffer = new byte[2];
                stream.Read(buffer, 0, buffer.Length);
                Assert.That(buffer[1], Is.EqualTo(0xAA));
                Assert.That(stream.BaseStream, Is.Not.Null);
            }
        }

        [Test]
        public void WriteOpensFile()
        {
            using (var stream = new LazyFileStream(tempFile, FileOpenMode.Write)) {
                Assert.That(stream.BaseStream, Is.Null);
                stream.WriteByte(0x42);
                Assert.That(stream.BaseStream, Is.Not.Null);
            }

            using (var fs = new FileStream(tempFile, FileMode.Open)) {
                Assert.That(fs.ReadByte(), Is.EqualTo(0x42));
            }

            using (var stream = new LazyFileStream(tempFile, FileOpenMode.Append)) {
                stream.Position = 1;
                Assert.That(stream.BaseStream, Is.Null);
                stream.Write(new byte[] { 0xAA, 0xBB }, 1, 1);
                Assert.That(stream.BaseStream, Is.Not.Null);
            }

            using (var fs = new FileStream(tempFile, FileMode.Open)) {
                Assert.That(fs.ReadByte(), Is.EqualTo(0x42));
                Assert.That(fs.ReadByte(), Is.EqualTo(0xBB));
            }
        }

        [Test]
        public void PublicMethodThrowAfterDispose()
        {
            var stream = new LazyFileStream(tempFile, FileOpenMode.ReadWrite);
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