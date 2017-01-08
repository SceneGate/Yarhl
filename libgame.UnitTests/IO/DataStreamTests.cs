//
// DataStreamTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sánchez
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
namespace Libgame.UnitTests.IO
{
    using System;
    using System.IO;
    using Libgame.IO;
    using NUnit.Framework;

    [TestFixture]
    public class DataStreamTests
    {
        [Test]
        public void ConstructorSetStream()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            DataStream stream = new DataStream(baseStream, 0x1, 0x100);
            Assert.AreSame(baseStream, stream.BaseStream);
            Assert.AreEqual(0x1, stream.Offset);
            Assert.AreEqual(0x100, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
        }

        [Test]
        public void ConstructorThrowExceptionIfNullStream()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((Stream)null, 0x10, 0x10));
        }

        [Test]
        public void ConstructorThrowExceptionIfNegativeOffset()
        {
            Stream baseStream = new MemoryStream();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, -1, 4));
        }

        [Test]
        public void ConstructorThrowExceptionIfOffsetBiggerThanLength()
        {
            Stream baseStream = new MemoryStream();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 1, 0));
        }

        [Test]
        public void ConstructorThrowExceptionIfLengthLessThanMinusOne()
        {
            Stream baseStream = new MemoryStream();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, -2));
        }

        [Test]
        public void DataStreamLengthLargerThanBaseLengthIsAllowed()
        {
            Stream baseStream = new MemoryStream();
            Assert.DoesNotThrow(() =>
                new DataStream(baseStream, 0, 100));
        }

        [Test]
        public void ConstructorSetLengthIfMinusOne()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, -1);
            Assert.AreEqual(2, stream.Length);
        }

        [Test]
        public void DisposeChangesDisposed()
        {
            DataStream stream = new DataStream();
            Assert.IsFalse(stream.Disposed);
            stream.Dispose();
            Assert.IsTrue(stream.Disposed);
        }

        [Test]
        public void DiposingLastStreamDisposeBaseStream()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);

            DataStream stream1 = new DataStream(baseStream);
            DataStream stream2 = new DataStream(baseStream);

            stream1.Dispose();
            Assert.DoesNotThrow(() => baseStream.ReadByte());

            stream2.Dispose();
            Assert.Throws<ObjectDisposedException>(() => baseStream.ReadByte());
        }

        [Test]
        public void DiposeCloseCorrectStream()
        {
            Stream baseStream1 = new MemoryStream();
            baseStream1.WriteByte(0xCA);
            Stream baseStream2 = new MemoryStream();
            baseStream2.WriteByte(0xCA);

            DataStream stream1 = new DataStream(baseStream1);
            DataStream stream2 = new DataStream(baseStream2);

            stream1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => baseStream1.ReadByte());
            Assert.DoesNotThrow(() => baseStream2.ReadByte());

            stream2.Dispose();
        }

        [Test]
        public void ConstructorThrowExceptionIfNullDataStream()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((DataStream)null, 0x10, 0x10));
        }
    }
}
