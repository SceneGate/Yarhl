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
namespace Yarhl.UnitTests.IO
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    [TestFixture]
    public class DataStreamTests
    {
        [Test]
        public void ConstructorFromStreamAndOffsetInitializes()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            int beforeCount = DataStream.ActiveStreams;
            DataStream stream = new DataStream(baseStream, 0x1, 0x1);
            Assert.AreSame(baseStream, stream.BaseStream);
            Assert.AreEqual(0x1, stream.Offset);
            Assert.AreEqual(0x1, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorThrowExceptionIfNullStream()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((Stream)null, 0, 0));
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
        public void ConstructorThrowExceptionIfLengthLessThanZero()
        {
            Stream baseStream = new MemoryStream();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, -1));
        }

        [Test]
        public void DataStreamLengthLargerThanBaseLengthIsNotAllowed()
        {
            Stream baseStream = new MemoryStream();
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, 100));
        }

        [Test]
        public void ConstructorWithOnlyStreamInitializes()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            int beforeCount = DataStream.ActiveStreams;
            DataStream stream = new DataStream(baseStream);
            Assert.AreSame(baseStream, stream.BaseStream);
            Assert.AreEqual(0x0, stream.Offset);
            Assert.AreEqual(0x2, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorStreamThrowsIfNull()
        {
            Assert.That(
                () => new DataStream((Stream)null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void DefaultConstructorCreatesAMemoryStream()
        {
            int beforeCount = DataStream.ActiveStreams;
            DataStream stream = new DataStream();
            Assert.IsInstanceOf<MemoryStream>(stream.BaseStream);
            Assert.IsTrue(stream.BaseStream.CanWrite);
            Assert.IsTrue(stream.BaseStream.CanRead);
            Assert.AreEqual(0x0, stream.Offset);
            Assert.AreEqual(0x0, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorFromArrayInitializes()
        {
            int beforeCount = DataStream.ActiveStreams;
            byte[] data = new byte[] { 0x01, 0x02, 0x03 };
            DataStream stream = new DataStream(data, 1, 2);
            Assert.That(stream.BaseStream, Is.InstanceOf<MemoryStream>());
            Assert.That(stream.BaseStream.CanWrite, Is.True);
            Assert.That(stream.BaseStream.CanRead, Is.True);
            Assert.That(stream.Offset, Is.EqualTo(1));
            Assert.That(stream.Length, Is.EqualTo(2));
            Assert.That(stream.Position, Is.EqualTo(0));
            Assert.IsNull(stream.ParentDataStream);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorFromArrayWithInvalidThrows()
        {
            byte[] data = new byte[] { 0x01 };

            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((byte[])null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(data, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(data, 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(data, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(data, 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(data, 1, 1));
        }

        [Test]
        public void FilePathConstructorOpenFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try {
                int beforeCount = DataStream.ActiveStreams;
                DataStream writeStream = new DataStream(tempFile, FileOpenMode.Write);
                Assert.IsInstanceOf<FileStream>(writeStream.BaseStream);
                Assert.IsTrue(writeStream.BaseStream.CanWrite);
                Assert.IsFalse(writeStream.BaseStream.CanRead);
                Assert.AreEqual(0x0, writeStream.Offset);
                Assert.AreEqual(0x0, writeStream.Length);
                Assert.AreEqual(0x0, writeStream.Position);
                Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
                writeStream.WriteByte(0xCA);
                writeStream.Dispose();

                DataStream readStream = new DataStream(tempFile, FileOpenMode.Read);
                Assert.IsInstanceOf<FileStream>(readStream.BaseStream);
                Assert.IsFalse(readStream.BaseStream.CanWrite);
                Assert.IsTrue(readStream.BaseStream.CanRead);
                Assert.IsNull(readStream.ParentDataStream);
                Assert.AreEqual(0x0, readStream.Offset);
                Assert.AreEqual(0x1, readStream.Length);
                Assert.AreEqual(0x0, readStream.Position);
                Assert.AreEqual(0xCA, readStream.ReadByte());
                readStream.Dispose();
            } finally {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void FilePathConstructorWithInvalidThrows()
        {
            Assert.That(
                () => new DataStream(null, FileOpenMode.Append),
                Throws.ArgumentNullException);
            Assert.That(
                () => new DataStream(string.Empty, FileOpenMode.Append),
                Throws.ArgumentNullException);
        }

        [Test]
        public void FilePathConstructorWithOffset()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            DataStream readStream = null;
            try {
                using (var stream = new DataStream(tempFile, FileOpenMode.Write)) {
                    stream.WriteByte(0xCA);
                    stream.WriteByte(0xFE);
                    stream.WriteByte(0xAF);
                    stream.WriteByte(0xFA);
                }

                int beforeCount = DataStream.ActiveStreams;
                readStream = new DataStream(tempFile, FileOpenMode.Read, 1, 2);
                Assert.AreEqual(0x1, readStream.Offset);
                Assert.AreEqual(0x2, readStream.Length);
                Assert.AreEqual(0x0, readStream.Position);
                Assert.AreEqual(0x1, readStream.AbsolutePosition);
                Assert.AreEqual(0xFE, readStream.ReadByte());
                Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
            } finally {
                readStream?.Dispose();
                File.Delete(tempFile);
            }
        }

        [Test]
        public void FilePathConstructorWithOffsetAndInvalidThrows()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try {
                using (var stream = new DataStream(tempFile, FileOpenMode.Write)) {
                    stream.WriteByte(0xCA);
                }

                Assert.Throws<ArgumentNullException>(() =>
                    new DataStream(null, FileOpenMode.Append, 0, 0));
                Assert.Throws<ArgumentNullException>(() =>
                    new DataStream(string.Empty, FileOpenMode.Append, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new DataStream(tempFile, FileOpenMode.Read, -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new DataStream(tempFile, FileOpenMode.Read, 2, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new DataStream(tempFile, FileOpenMode.Read, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new DataStream(tempFile, FileOpenMode.Read, 0, 2));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    new DataStream(tempFile, FileOpenMode.Read, 1, 1));
            } finally {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void ConstructorFromDataStreamSetProperties()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            baseStream.WriteByte(0xBE);
            baseStream.WriteByte(0xBA);
            DataStream stream1 = new DataStream(baseStream, 1, 3);

            int beforeCount = DataStream.ActiveStreams;
            DataStream stream2 = new DataStream(stream1, 1, 1);
            Assert.AreSame(baseStream, stream2.BaseStream);
            Assert.AreEqual(0x2, stream2.Offset);
            Assert.AreEqual(0x1, stream2.Length);
            Assert.AreEqual(0x0, stream2.Position);
            Assert.AreEqual(0xBE, stream2.ReadByte());
            Assert.AreSame(stream1, stream2.ParentDataStream);
            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorFromDataStreamInvalidThrows()
        {
            DataStream baseStream = new DataStream();
            baseStream.WriteByte(0x01);

            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((DataStream)null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 1, 1));
        }

        [Test]
        public void DisposeTwiceDoesNotThrowException()
        {
            DataStream stream = new DataStream();
            stream.Dispose();
            Assert.DoesNotThrow(stream.Dispose);
        }

        [Test]
        public void DisposeTwiceDoesNotAffectOtherStreams()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            baseStream.Position = 0;

            DataStream stream1 = new DataStream(baseStream);
            DataStream stream2 = new DataStream(baseStream);

            stream1.Dispose();
            stream1.Dispose();

            Assert.DoesNotThrow(() => baseStream.ReadByte());
            stream2.Dispose();
            Assert.Throws<ObjectDisposedException>(() => baseStream.ReadByte());
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
        public void ActiveStreamsUpdatesWithDisposes()
        {
            int currActive = DataStream.ActiveStreams;
            DataStream stream = new DataStream();
            Assert.AreEqual(currActive + 1, DataStream.ActiveStreams);
            stream.Dispose();
            Assert.AreEqual(currActive, DataStream.ActiveStreams);
        }

        [Test]
        public void ActiveStreamsDoesNotChangeWithSubStreams()
        {
            int currActive = DataStream.ActiveStreams;
            DataStream stream = new DataStream();
            Assert.AreEqual(currActive + 1, DataStream.ActiveStreams);

            DataStream stream2 = new DataStream(stream, 0, 0);
            Assert.AreEqual(currActive + 1, DataStream.ActiveStreams);
            stream2.Dispose();
            Assert.AreEqual(currActive + 1, DataStream.ActiveStreams);

            stream.Dispose();
            Assert.AreEqual(currActive, DataStream.ActiveStreams);
        }

        [Test]
        public void SetPositionChangesProperty()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Position = 1;
            Assert.AreEqual(1, stream.Position);
            stream.Position += 1;
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void SetPositionAfterDisposeThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Position = 2);
        }

        [Test]
        public void SetNegativePositionThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);
        }

        [Test]
        public void SetOutofRangePositionThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = 10);
        }

        [Test]
        public void SetLength()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream);
            Assert.AreEqual(2, stream.Length);
            stream.Length = 1;
            Assert.AreEqual(1, stream.Length);
        }

        [Test]
        public void CannotIncreaseLengthFromSubStream()
        {
            DataStream parentStream = new DataStream();
            parentStream.WriteByte(0x00);
            parentStream.WriteByte(0x00);

            DataStream childStream = new DataStream(parentStream, 1, 1);
            childStream.WriteByte(0x01);
            Assert.That(
                () => childStream.WriteByte(0x01),
                Throws.InvalidOperationException);

            DataStream childStream2 = new DataStream(parentStream, 0, 2);
            childStream2.WriteByte(0x02);
            childStream2.WriteByte(0x02);
            Assert.That(
                () => childStream2.WriteByte(0x02),
                Throws.InvalidOperationException);
        }

        [Test]
        public void CanPreallocateLengthForSubstream()
        {
            DataStream parent = new DataStream();
            parent.BaseStream.SetLength(11);
            parent.Length = 11;
            parent.WriteByte(0xFF);
            Assert.That(parent.Length, Is.EqualTo(11));

            DataStream child = new DataStream(parent, 1, 10);
            Assert.That(child.Length, Is.EqualTo(10));
            Assert.That(parent.BaseStream.Length, Is.EqualTo(11));

            for (int i = 0; i < 10; i++) {
                Assert.That(child.ReadByte(), Is.EqualTo(0));
            }
        }

        [Test]
        public void LengthAfterDisposeThrowException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            DataStream stream = new DataStream(baseStream, 0, 1);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Length = 1);
        }

        [Test]
        public void OutOfRangeLengthThrowException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            DataStream stream = new DataStream(baseStream, 0, 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Length = -2);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Length = 2);
        }

        [Test]
        public void DecreaseLengthAdjustPosition()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream);
            stream.Seek(2, SeekMode.Start);
            Assert.AreEqual(2, stream.Position);
            stream.Length = 1;
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void EndOfStreamIsSet()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.IsFalse(stream.EndOfStream);
            stream.Seek(2, SeekMode.Start);
            Assert.IsTrue(stream.EndOfStream);
        }

        [Test]
        public void GetValidAbsolutePosition()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 1, 1);
            stream.Seek(1, SeekMode.Start);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(2, stream.AbsolutePosition);
        }

        [Test]
        public void SeekFromOrigin()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Seek(1, SeekMode.Start);
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void SeekFromCurrent()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Seek(1, SeekMode.Start);
            Assert.AreEqual(1, stream.Position);
            stream.Seek(1, SeekMode.Current);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void SeekFromEnd()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Seek(0, SeekMode.End);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void SeekDefaultIsStart()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.Seek(0);
            Assert.That(stream.Position, Is.EqualTo(0));
            Assert.That(stream.AbsolutePosition, Is.EqualTo(0));
        }

        [Test]
        public void SeekWihtInvalidModeThrows()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            Assert.That(
                () => stream.Seek(0, (SeekMode)0x100),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void SeekWhenDisposedThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Seek(1, SeekMode.Start));
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void SeekToNegativeThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(10, SeekMode.End));
            Assert.AreEqual(0, stream.Position);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(-10, SeekMode.Current));
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void SeekToOutOfRangeThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Position = 1;
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(10, SeekMode.Start));
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void PushPositionMovePosition()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.AreEqual(2, stream.Position);
            stream.PushToPosition(-1, SeekMode.Current);
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void PushToPositionDefaultIsStart()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.PushToPosition(0);
            Assert.That(stream.Position, Is.EqualTo(0));
            Assert.That(stream.AbsolutePosition, Is.EqualTo(0));
        }

        [Test]
        public void PushPositionThrowExceptionAfterDispose()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
                stream.PushToPosition(-1, SeekMode.Current));
        }

        [Test]
        public void PushCurrentPositionDoesNotMovePosition()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.AreEqual(2, stream.Position);
            stream.PushCurrentPosition();
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void PushCurrentPositionThrowExceptionAfterDispose()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(stream.PushCurrentPosition);
        }

        [Test]
        public void PopRestoresPosition()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.AreEqual(2, stream.Position);
            stream.PushToPosition(-1, SeekMode.Current);
            Assert.AreEqual(1, stream.Position);
            stream.PushToPosition(-1, SeekMode.Current);
            Assert.AreEqual(0, stream.Position);
            stream.PushCurrentPosition();
            stream.Position = 2;
            Assert.AreEqual(2, stream.Position);
            stream.PopPosition();
            Assert.AreEqual(0, stream.Position);
            stream.PopPosition();
            Assert.AreEqual(1, stream.Position);
            stream.PopPosition();
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void PopWithoutPushThrowException()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.Throws<InvalidOperationException>(stream.PopPosition);
        }

        [Test]
        public void PopThrowsExceptionAfterDispose()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            stream.PushToPosition(-1, SeekMode.Current);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(stream.PopPosition);
        }

        [Test]
        public void RunInPositionMoves()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBA);
            stream.WriteByte(0xBE);
            stream.RunInPosition(
                () => Assert.That(stream.ReadByte(), Is.EqualTo(0xBA)),
                -2,
                SeekMode.Current);
        }

        [Test]
        public void RunInPositionMoveAndReset()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBA);
            stream.WriteByte(0xBE);
            stream.RunInPosition(
                () => Assert.That(stream.ReadByte(), Is.EqualTo(0xFE)),
                1,
                SeekMode.Start);
            Assert.That(stream.Position, Is.EqualTo(4));
        }

        [Test]
        public void RunInPositionDefaultIsStart()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBA);
            stream.WriteByte(0xBE);
            stream.RunInPosition(
                () => Assert.That(stream.ReadByte(), Is.EqualTo(0xFE)),
                1);
        }

        [Test]
        public void RunInPositionWithNullMethodThrows()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            Assert.That(() => stream.RunInPosition(null, 0), Throws.ArgumentNullException);
        }

        [Test]
        public void ReadsByte()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadByteAfterDisposeThrowException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
        }

        [Test]
        public void ReadByteWhenEOSThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Seek(0, SeekMode.End);
            Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadSetBaseStreamPosition()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            baseStream.Position = 1;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void ReadsBuffer()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            byte[] buffer = new byte[2];
            Assert.AreEqual(1, stream.Read(buffer, 0, 1));
            Assert.AreEqual(0xCA, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
            Assert.AreEqual(1, stream.Position);

            Assert.AreEqual(1, stream.Read(buffer, 1, 1));
            Assert.AreEqual(0xCA, buffer[0]);
            Assert.AreEqual(0xFE, buffer[1]);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadBufferfterDisposeThrowException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Dispose();
            byte[] buffer = new byte[2];
            Assert.Throws<ObjectDisposedException>(() => stream.Read(buffer, 0, 0));
        }

        [Test]
        public void ReadBufferWhenEOSThrowsException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            stream.Seek(0, SeekMode.End);
            byte[] buffer = new byte[2];
            Assert.Throws<EndOfStreamException>(() => stream.Read(buffer, 0, 1));
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadBufferSetBaseStreamPosition()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            baseStream.Position = 1;
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            Assert.AreEqual(0xCA, buffer[0]);
        }

        [Test]
        public void ReadBufferZeroBytes()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            byte[] buffer = new byte[2];
            Assert.DoesNotThrow(() => stream.Read(buffer, 10, 0));
            Assert.AreEqual(0, stream.Read(buffer, 10, 0));
        }

        [Test]
        public void ReadBufferButNullThrowException()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            Assert.DoesNotThrow(() => stream.Read(null, 0, 0));
            Assert.Throws<ArgumentNullException>(() => stream.Read(null, 0, 1));
        }

        [Test]
        public void ReadBufferOutOfRange()
        {
            Stream baseStream = new MemoryStream();
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2);
            byte[] buffer = new byte[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 0, 10));
        }

        [Test]
        public void ReadFormat()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xAF);
            stream.Position = 0x00;
            Assert.AreEqual(0xAF, stream.ReadFormat<byte>());
        }

        [Test]
        public void ReadFormAfeterDisposeThrowException()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xAF);
            stream.Position = 0x00;

            stream.Dispose();
            Assert.IsTrue(stream.Disposed);
            Assert.Throws<ObjectDisposedException>(() => stream.ReadFormat<byte>());
        }

        [Test]
        public void WritesAByteAndIncreasePosition()
        {
            MemoryStream baseStream = new MemoryStream(2);
            DataStream stream = new DataStream(baseStream);
            stream.WriteByte(0xCA);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteByteIncreaseLength()
        {
            DataStream stream = new DataStream();
            Assert.DoesNotThrow(() => stream.WriteByte(0xCA));
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteByteAfterDisposeThrowException()
        {
            DataStream stream = new DataStream();
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(0xCA));
        }

        [Test]
        public void WriteByteSetBaseStreamPosition()
        {
            MemoryStream baseStream = new MemoryStream(2);
            DataStream stream = new DataStream(baseStream);
            baseStream.Position = 1;
            stream.WriteByte(0xCA);
            baseStream.Position = 0;
            Assert.AreEqual(0xCA, baseStream.ReadByte());
        }

        [Test]
        public void WriteBufferAndIncreasePosition()
        {
            MemoryStream baseStream = new MemoryStream(2);
            DataStream stream = new DataStream(baseStream);
            byte[] buffer = { 0x00, 0xCA };
            stream.Write(buffer, 1, 1);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteBufferAndIncreaseLength()
        {
            MemoryStream baseStream = new MemoryStream();
            baseStream.WriteByte(0xFF);
            DataStream stream = new DataStream(baseStream);
            Assert.AreEqual(1, stream.Length);
            byte[] buffer = { 0xCA, 0xFE };
            Assert.DoesNotThrow(() => stream.Write(buffer, 0, 2));
            Assert.AreEqual(2, stream.Position);
            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
        }

        [Test]
        public void WriteBufferWhenEOSIncreaseLength()
        {
            DataStream stream = new DataStream();
            byte[] buffer = { 0xCA, 0xFE };
            Assert.DoesNotThrow(() => stream.Write(buffer, 0, 2));
            Assert.AreEqual(2, stream.Position);
            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
        }

        [Test]
        public void WriteBufferAfterDisposeThrowException()
        {
            DataStream stream = new DataStream();
            byte[] buffer = { 0xCA };
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Write(buffer, 0, 1));
        }

        [Test]
        public void WriteBufferSetBaseStreamPosition()
        {
            MemoryStream baseStream = new MemoryStream();
            baseStream.WriteByte(0xFF);
            DataStream stream = new DataStream(baseStream);
            Assert.AreEqual(1, baseStream.Position);
            byte[] buffer = { 0xCA };
            stream.Write(buffer, 0, 1);
            baseStream.Position = 0;
            Assert.AreEqual(0xCA, baseStream.ReadByte());
        }

        [Test]
        public void WriteBufferButNullThrowException()
        {
            DataStream stream = new DataStream();
            Assert.Throws<ArgumentNullException>(() => stream.Write(null, 0, 1));
        }

        [Test]
        public void WriteBufferOutOfRangeThrowException()
        {
            DataStream stream = new DataStream();
            byte[] buffer = { 0xCA };
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 0, 10));
        }

        [Test]
        public void WriteBufferWithZeroBytes()
        {
            DataStream stream = new DataStream();
            byte[] buffer = { 0xCA };
            Assert.DoesNotThrow(() => stream.Write(null, 0, 0));
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(0, stream.Length);
            Assert.DoesNotThrow(() => stream.Write(buffer, 10, 0));
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(0, stream.Length);
            stream.Write(buffer, 0, 0);
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(0, stream.Length);
        }

        [Test]
        public void WriteToStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream1.WriteTo(stream2);
            stream2.Position = 0;
            Assert.AreEqual(0xCA, stream2.ReadByte());
            Assert.AreEqual(0xFE, stream2.ReadByte());
            Assert.AreEqual(0x00, stream2.ReadByte());
            Assert.AreEqual(0xFF, stream2.ReadByte());
        }

        [Test]
        public void WriteToNullStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            Assert.Throws<ArgumentNullException>(
                () => stream1.WriteTo((DataStream)null));
        }

        [Test]
        public void WriteToDisposedStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream1.WriteTo(stream2));
        }

        [Test]
        public void WriteToAfterDisposeStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream1.WriteTo(stream2));
        }

        [Test]
        public void WriteToDoesNotChangePosition()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xBE);
            stream1.WriteTo(stream2);
            Assert.AreEqual(4, stream1.Position);
        }

        [Test]
        public void WriteToStartsFromBeginning()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream1.Position = 4;
            Assert.AreEqual(4, stream1.Position);
            stream1.WriteTo(stream2);
            stream2.Position = 0;
            Assert.AreEqual(0xCA, stream2.ReadByte());
            Assert.AreEqual(0xFE, stream2.ReadByte());
            Assert.AreEqual(0x00, stream2.ReadByte());
            Assert.AreEqual(0xFF, stream2.ReadByte());
        }

        [Test]
        public void WriteToChangeBaseStreamPosition()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream1.BaseStream.Position = 4;
            Assert.AreEqual(4, stream1.BaseStream.Position);
            stream1.WriteTo(stream2);
            stream2.Position = 0;
            Assert.AreEqual(0xCA, stream2.ReadByte());
            Assert.AreEqual(0xFE, stream2.ReadByte());
            Assert.AreEqual(0x00, stream2.ReadByte());
            Assert.AreEqual(0xFF, stream2.ReadByte());
        }

        [Test]
        public void WriteToMoreThanOneBuffer()
        {
            const int SIZE = 8 * 1024;
            DataStream stream1 = new DataStream();
            for (int i = 0; i < SIZE; i++)
                stream1.WriteByte((byte)(i % 256));
            DataStream stream2 = new DataStream();
            stream1.WriteTo(stream2);
            Assert.IsTrue(stream1.Compare(stream2));
        }

        [Test]
        public void WriteToDoesNotOverwrite()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xBE);
            stream1.WriteTo(stream2);
            Assert.AreEqual(4, stream1.Position);
            Assert.AreEqual(5, stream2.Position);
            Assert.AreEqual(5, stream2.Length);
            stream2.Position = 0;
            Assert.AreEqual(0xBE, stream2.ReadByte());
            Assert.AreEqual(0xCA, stream2.ReadByte());
            Assert.AreEqual(0xFE, stream2.ReadByte());
            Assert.AreEqual(0x00, stream2.ReadByte());
            Assert.AreEqual(0xFF, stream2.ReadByte());
        }

        [Test]
        public void WriteToFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x00);
            stream.WriteByte(0xFF);
            stream.WriteTo(tempFile);

            DataStream fileStream = new DataStream(tempFile, FileOpenMode.Read);
            Assert.IsTrue(stream.Compare(fileStream));

            fileStream.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void WriteToFileCreatesParentFolder()
        {
            string tempFile = Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName(),
                Path.GetRandomFileName());

            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteTo(tempFile);

            DataStream fileStream = new DataStream(tempFile, FileOpenMode.Read);
            Assert.That(() => stream.Compare(fileStream), Is.True);

            fileStream.Dispose();
            File.Delete(tempFile);
            stream.Dispose();
        }

        [Test]
        public void WriteToRelativePathFile()
        {
            string tempFile = Path.GetRandomFileName();

            string cwd = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetTempPath();

            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteTo(tempFile);

            DataStream fileStream = new DataStream(tempFile, FileOpenMode.Read);
            Assert.That(() => stream.Compare(fileStream), Is.True);

            fileStream.Dispose();
            File.Delete(tempFile);
            stream.Dispose();

            Environment.CurrentDirectory = cwd;
        }

        [Test]
        public void WriteToNullFile()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            Assert.Throws<ArgumentNullException>(
                () => stream1.WriteTo((string)null));
            Assert.Throws<ArgumentNullException>(() => stream1.WriteTo(string.Empty));
        }

        [Test]
        public void WriteToAfterDispose()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            stream1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream1.WriteTo("/ex"));
        }

        [Test]
        public void CompareTwoEqualStreams()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream2.WriteByte(0xFF);
            Assert.IsTrue(stream1.Compare(stream2));
            Assert.IsTrue(stream2.Compare(stream1));
        }

        [Test]
        public void CompareTwoDifferentContentStreams()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x01);
            stream2.WriteByte(0xFF);
            Assert.IsFalse(stream1.Compare(stream2));
            Assert.IsFalse(stream2.Compare(stream1));
        }

        [Test]
        public void CompareTwoDifferentLengthStreams()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            Assert.IsFalse(stream1.Compare(stream2));
            Assert.IsFalse(stream2.Compare(stream1));
        }

        [Test]
        public void CompareWithNullStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            Assert.Throws<ArgumentNullException>(() => stream1.Compare(null));
        }

        [Test]
        public void CompareWithDisposedStream()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            stream2.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream1.Compare(stream2));
        }

        [Test]
        public void CompareAfterDispose()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            stream1.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream1.Compare(stream2));
        }

        [Test]
        public void CompareStartsFromBeginning()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream2.WriteByte(0xFF);
            stream1.Position = 1;
            Assert.IsTrue(stream1.Compare(stream2));
        }

        [Test]
        public void CompareDoesNotResetPosition()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream2.WriteByte(0xFF);
            stream1.Position = 1;
            stream2.Position = 2;
            Assert.IsTrue(stream1.Compare(stream2));
            Assert.AreEqual(1, stream1.Position);
            Assert.AreEqual(2, stream2.Position);
        }

        [Test]
        public void CompareChangeBaseStreamPosition()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);
            DataStream stream2 = new DataStream();
            stream2.WriteByte(0xCA);
            stream2.WriteByte(0xFE);
            stream2.WriteByte(0x00);
            stream2.WriteByte(0xFF);
            stream1.BaseStream.Position = 1;
            stream2.BaseStream.Position = 2;
            Assert.IsTrue(stream1.Compare(stream2));
        }

        public class DummyBinaryConverter : IConverter<BinaryFormat, byte>
        {
            public byte Convert(BinaryFormat source)
            {
                return source.Stream.ReadByte();
            }
        }
    }
}
