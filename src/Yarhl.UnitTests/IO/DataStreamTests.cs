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
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Threading.Tasks;
    using Moq;
    using NUnit.Framework;
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class DataStreamTests
    {
        Stream baseStream;

        [SetUp]
        public void SetUp()
        {
            baseStream = new RecyclableMemoryStream();
        }

        [TearDown]
        public void TearDown()
        {
            baseStream?.Dispose();
        }

        [Test]
        public void ConstructorFromStreamAndOffsetInitializes()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);

            DataStream stream = new DataStream(baseStream, 0x1, 0x1, false);
            Assert.AreSame(baseStream, stream.BaseStream);
            Assert.AreEqual(0x1, stream.Offset);
            Assert.AreEqual(0x1, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            stream.Dispose();
        }

        [Test]
        public void ConstructorFromStreamAndOffsetThrowsIfInvalid()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((Stream)null, 0, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, -1, 4, false));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 1, 0, false));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, -1, false));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, 100, false));

            var noOwnershipStream = new DataStream(baseStream, 0, 0, false);
            Assert.Throws<ArgumentException>(() =>
                new DataStream(noOwnershipStream, 0, 0, true));
        }

        [Test]
        public void ConstructorFromStreamAndOffsetNoOwnershipArgInitializes()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);

            using DataStream stream = new DataStream(baseStream, 0x1, 0x1);
            Assert.AreSame(baseStream, stream.BaseStream);
            Assert.AreEqual(0x1, stream.Offset);
            Assert.AreEqual(0x1, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            Assert.That(stream.InternalInfo.NumInstances, Is.EqualTo(1));
        }

        [Test]
        public void ConstructorFromStreamAndOffsetNoOwnershipArgThrowsIfInvalid()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((Stream)null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, -1, 4));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(baseStream, 0, 100));
        }

        [Test]
        public void ConstructorFromStreamAndOffsetNoOwnershipArgInheritOwnership()
        {
            using var noOwnershipStream = new DataStream(baseStream, 0, 0, false);
            Assert.That(() => new DataStream(noOwnershipStream, 0, 0), Throws.Nothing);
        }

        [Test]
        public void ConstructorWithOnlyStreamInitializes()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            int beforeCount = DataStream.ActiveStreams;
            using DataStream stream = new DataStream(baseStream);
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
            Assert.IsInstanceOf<RecyclableMemoryStream>(stream.BaseStream);
            Assert.AreEqual(0x0, stream.Offset);
            Assert.AreEqual(0x0, stream.Length);
            Assert.AreEqual(0x0, stream.Position);
            Assert.IsNull(stream.ParentDataStream);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
            stream.Dispose();
        }

        [Test]
        public void ConstructorFromDataStreamSetProperties()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            baseStream.WriteByte(0xBE);
            baseStream.WriteByte(0xBA);
            using DataStream stream1 = new DataStream(baseStream, 1, 3, false);

            int beforeCount = DataStream.ActiveStreams;
            using DataStream stream2 = new DataStream(stream1, 1, 1);
            Assert.AreSame(baseStream, stream2.BaseStream);
            Assert.AreEqual(0x2, stream2.Offset);
            Assert.AreEqual(0x1, stream2.Length);
            Assert.AreEqual(0x0, stream2.Position);
            Assert.AreEqual(0xBE, stream2.ReadByte());
            Assert.AreSame(stream1, stream2.ParentDataStream);
            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void ConstructorFromDataStreamForwardOwnershipStatus()
        {
            int beforeCount = DataStream.ActiveStreams;

            // No ownership
            DataStream parent = new DataStream(baseStream, 0, 0, false);
            DataStream child = new DataStream(parent, 0, 0);
            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
            child.Dispose();
            parent.Dispose();

            // Ownership
            parent = new DataStream(baseStream, 0, 0, true);
            child = new DataStream(parent, 0, 0);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);
            child.Dispose();
            parent.Dispose();
        }

        [Test]
        public void ConstructorFromDataStreamInvalidThrows()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0x01);

            Assert.Throws<ArgumentNullException>(() =>
                new DataStream((DataStream)null, 0, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(stream, -1, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(stream, 2, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(stream, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(stream, 0, 2));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new DataStream(stream, 1, 1));

            stream.Dispose();
        }

        [Test]
        public void ConstructorFromStreamWithoutOwnership()
        {
            int beforeCount = DataStream.ActiveStreams;
            baseStream.WriteByte(0x00);

            DataStream noOwnershipStream = new DataStream(baseStream, 0, 0, false);
            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);

            noOwnershipStream.Dispose();
            Assert.DoesNotThrow(() => baseStream.ReadByte());
        }

        [Test]
        public void ConstructorFromStreamWithOwnership()
        {
            int beforeCount = DataStream.ActiveStreams;
            baseStream.WriteByte(0x00);

            DataStream ownershipStream = new DataStream(baseStream, 0, 0, true);
            Assert.AreEqual(beforeCount + 1, DataStream.ActiveStreams);

            ownershipStream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => baseStream.ReadByte());
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
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            baseStream.Position = 0;

            DataStream stream1 = new DataStream(baseStream);
            DataStream stream2 = new DataStream(baseStream);

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
        public void DisposingLastStreamDisposeBaseStream()
        {
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
        public void DisposeCloseCorrectStream()
        {
            using var baseStream1 = new RecyclableMemoryStream();
            baseStream1.WriteByte(0xCA);
            using var baseStream2 = new RecyclableMemoryStream();
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
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Position = 1;
            Assert.AreEqual(1, stream.Position);
            stream.Position += 1;
            Assert.AreEqual(2, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void SetPositionAfterDisposeThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Position = 2);
        }

        [Test]
        public void SetNegativePositionThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = -1);
            stream.Dispose();
        }

        [Test]
        public void SetOutofRangePositionThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Position = 10);
            stream.Dispose();
        }

        [Test]
        public void SetLength()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream);
            Assert.AreEqual(2, stream.Length);
            stream.SetLength(1);
            Assert.AreEqual(1, stream.Length);
            stream.Dispose();
        }

        [Test]
        public void CannotExpandSubDataStream()
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
            byte[] data = new byte[] { 0x02, 0x02, 0x02 };
            Assert.That(
                () => childStream2.Write(data, 0, data.Length),
                Throws.InvalidOperationException);

            DataStream childStream3 = new DataStream(parentStream, 0, 1);
            Assert.That(
                () => childStream3.SetLength(2),
                Throws.InvalidOperationException);
            Assert.That(
                () => childStream3.SetLength(0),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(1),
                Throws.InvalidOperationException);

            childStream.Dispose();
            childStream2.Dispose();
            childStream3.Dispose();
            parentStream.Dispose();
        }

        [Test]
        public void CanExpandStream()
        {
            using var parentStream = new RecyclableMemoryStream();

            using DataStream childStream = new DataStream(parentStream);
            Assert.That(() => childStream.WriteByte(0x01), Throws.Nothing);

            using DataStream childStream2 = new DataStream(parentStream);
            byte[] data = new byte[] { 0x02, 0x02 };
            Assert.That(
                () => childStream2.Write(data, 0, data.Length),
                Throws.Nothing);

            using DataStream childStream3 = new DataStream(parentStream);
            parentStream.Position = 2;
            parentStream.WriteByte(0x03);
            Assert.That(
                () => childStream3.SetLength(3),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(0),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(1),
                Throws.Nothing);
        }

        [Test]
        public void CannotExpandSubStream()
        {
            using var parentStream = new RecyclableMemoryStream();
            parentStream.WriteByte(0x00);
            parentStream.WriteByte(0x00);

            using DataStream childStream = new DataStream(parentStream, 1, 1, false);
            childStream.WriteByte(0x01);
            Assert.That(
                () => childStream.WriteByte(0x01),
                Throws.InvalidOperationException);

            using DataStream childStream2 = new DataStream(parentStream, 0, 2, false);
            byte[] data = new byte[] { 0x02, 0x02, 0x02 };
            Assert.That(
                () => childStream2.Write(data, 0, data.Length),
                Throws.InvalidOperationException);

            using DataStream childStream3 = new DataStream(parentStream, 0, 1, false);
            Assert.That(
                () => childStream3.SetLength(2),
                Throws.InvalidOperationException);
            Assert.That(
                () => childStream3.SetLength(0),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(1),
                Throws.InvalidOperationException);
        }

        [Test]
        public void CanExpandMemoryStream()
        {
            DataStream childStream = new DataStream();
            Assert.That(() => childStream.WriteByte(0x01), Throws.Nothing);

            DataStream childStream2 = new DataStream();
            byte[] data = new byte[] { 0x02, 0x02 };
            Assert.That(
                () => childStream2.Write(data, 0, data.Length),
                Throws.Nothing);

            DataStream childStream3 = new DataStream();
            childStream3.BaseStream.WriteByte(0x03);
            Assert.That(
                () => childStream3.SetLength(1),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(0),
                Throws.Nothing);
            Assert.That(
                () => childStream3.SetLength(1),
                Throws.Nothing);

            childStream.Dispose();
            childStream2.Dispose();
            childStream3.Dispose();
        }

        [Test]
        public void CanPreallocateLengthForSubstream()
        {
            using DataStream parent = new DataStream();
            parent.SetLength(11);
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
            baseStream.WriteByte(0xCA);
            DataStream stream = new DataStream(baseStream, 0, 1, false);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.SetLength(1));
        }

        [Test]
        public void OutOfRangeLengthThrowException()
        {
            baseStream.WriteByte(0xCA);
            DataStream stream = new DataStream(baseStream, 0, 0, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.SetLength(-2));
            stream.Dispose();
        }

        [Test]
        public void DecreaseLengthAdjustPosition()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            using DataStream stream = new DataStream(baseStream);
            stream.Seek(2, SeekOrigin.Begin);
            Assert.AreEqual(2, stream.Position);
            stream.SetLength(1);
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void EndOfStreamIsSet()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.IsFalse(stream.EndOfStream);
            stream.Seek(2, SeekOrigin.Begin);
            Assert.IsTrue(stream.EndOfStream);
            stream.Dispose();
        }

        [Test]
        public void GetValidAbsolutePosition()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 1, 1, false);
            stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(2, stream.AbsolutePosition);
            stream.Dispose();
        }

        [Test]
        public void SeekFromOrigin()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(1, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void SeekFromCurrent()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Seek(1, SeekOrigin.Begin);
            Assert.AreEqual(1, stream.Position);
            stream.Seek(1, SeekOrigin.Current);
            Assert.AreEqual(2, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void SeekFromEnd()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Seek(0, SeekOrigin.End);
            Assert.AreEqual(2, stream.Position);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(1, SeekOrigin.End));
            stream.Seek(-2, SeekOrigin.End);
            Assert.AreEqual(0, stream.Position);
            stream.Dispose();
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
            stream.Dispose();
        }

        [Test]
        public void SeekWithInvalidModeThrows()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            Assert.That(
                () => stream.Seek(0, (SeekOrigin)0x100),
                Throws.TypeOf<ArgumentOutOfRangeException>());
            stream.Dispose();
        }

        [Test]
        public void SeekWhenDisposedThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.Seek(1, SeekOrigin.Begin));
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void SeekToNegativeThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(10, SeekOrigin.End));
            Assert.AreEqual(0, stream.Position);
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(-10, SeekOrigin.Current));
            Assert.AreEqual(0, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void SeekToOutOfRangeThrowsException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Position = 1;
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Seek(10, SeekOrigin.Begin));
            Assert.AreEqual(1, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void PushPositionMovePosition()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.AreEqual(2, stream.Position);
            stream.PushToPosition(-1, SeekOrigin.Current);
            Assert.AreEqual(1, stream.Position);
            stream.Dispose();
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
            stream.Dispose();
        }

        [Test]
        public void PushPositionThrowExceptionAfterDispose()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
                stream.PushToPosition(-1, SeekOrigin.Current));
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
            stream.Dispose();
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
            stream.PushToPosition(-1, SeekOrigin.Current);
            Assert.AreEqual(1, stream.Position);
            stream.PushToPosition(-1, SeekOrigin.Current);
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
            stream.Dispose();
        }

        [Test]
        public void PopWithoutPushThrowException()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            Assert.Throws<InvalidOperationException>(stream.PopPosition);
            stream.Dispose();
        }

        [Test]
        public void PopThrowsExceptionAfterDispose()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xA);
            stream.WriteByte(0xB);
            stream.PushToPosition(-1, SeekOrigin.Current);
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
                SeekOrigin.Current);
            stream.Dispose();
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
                SeekOrigin.Begin);
            Assert.That(stream.Position, Is.EqualTo(4));
            stream.Dispose();
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
            stream.Dispose();
        }

        [Test]
        public void RunInPositionWithNullMethodThrows()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            Assert.That(() => stream.RunInPosition(null, 0), Throws.ArgumentNullException);
            stream.Dispose();
        }

        [Test]
        public void ReadsByte()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(1, stream.Position);
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(2, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void ReadByteAfterDisposeThrowException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.ReadByte());
        }

        [Test]
        public void ReadByteWhenEOSReturnsMinusOne()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Seek(0, SeekOrigin.End);
            Assert.That(stream.ReadByte(), Is.EqualTo(-1));
            Assert.AreEqual(2, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void ReadSetBaseStreamPosition()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            baseStream.Position = 1;
            Assert.AreEqual(0xCA, stream.ReadByte());
            stream.Dispose();
        }

        [Test]
        public void ReadsBuffer()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            byte[] buffer = new byte[2];
            Assert.AreEqual(1, stream.Read(buffer, 0, 1));
            Assert.AreEqual(0xCA, buffer[0]);
            Assert.AreEqual(0x00, buffer[1]);
            Assert.AreEqual(1, stream.Position);

            Assert.AreEqual(1, stream.Read(buffer, 1, 1));
            Assert.AreEqual(0xCA, buffer[0]);
            Assert.AreEqual(0xFE, buffer[1]);
            Assert.AreEqual(2, stream.Position);
            stream.Dispose();
        }

        [Test]
        public void ReadBufferAfterDisposeThrowException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Dispose();
            byte[] buffer = new byte[2];
            Assert.Throws<ObjectDisposedException>(() => stream.Read(buffer, 0, 0));
            stream.Dispose();
        }

        [Test]
        public void ReadBufferWhenEOSReadsAsMuchAsPossible()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            stream.Seek(-1, SeekOrigin.End);
            byte[] buffer = new byte[2];
            Assert.DoesNotThrow(() => stream.Read(buffer, 0, 2));
            Assert.AreEqual(2, stream.Position);
            Assert.That(buffer[0], Is.EqualTo(0xFE));
            Assert.That(buffer[1], Is.EqualTo(0x00));
            stream.Dispose();
        }

        [Test]
        public void ReadBufferSetBaseStreamPosition()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            baseStream.Position = 1;
            byte[] buffer = new byte[1];
            stream.Read(buffer, 0, 1);
            Assert.AreEqual(0xCA, buffer[0]);
            stream.Dispose();
        }

        [Test]
        public void ReadBufferZeroBytes()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            using DataStream stream = new DataStream(baseStream, 0, 2, false);
            byte[] buffer = new byte[2];
            Assert.That(() => stream.Read(buffer, 10, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());

            Assert.That(() => stream.Read(null, 0, 0), Throws.InstanceOf<ArgumentNullException>());
            stream.Read(buffer, 0, 0);
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void ReadBufferButNullThrowException()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            using DataStream stream = new DataStream(baseStream, 0, 2, false);
            Assert.That(() => stream.Read(null, 0, 0), Throws.ArgumentNullException);
            Assert.Throws<ArgumentNullException>(() => stream.Read(null, 0, 1));
        }

        [Test]
        public void ReadBufferOutOfRange()
        {
            baseStream.WriteByte(0xCA);
            baseStream.WriteByte(0xFE);
            DataStream stream = new DataStream(baseStream, 0, 2, false);
            byte[] buffer = new byte[2];
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Read(buffer, 0, 10));
            stream.Dispose();
        }

        [Test]
        public void ReadFormat()
        {
            DataStream stream = new DataStream();
            stream.WriteByte(0xAF);
            stream.Position = 0x00;
            Assert.AreEqual(0xAF, stream.ReadFormat<byte>());
            stream.Dispose();
        }

        [Test]
        public void ReadFormAfterDisposeThrowException()
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
            using DataStream stream = new DataStream(baseStream);
            stream.WriteByte(0xCA);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteByteIncreaseLength()
        {
            using DataStream stream = new DataStream();
            Assert.DoesNotThrow(() => stream.WriteByte(0xCA));
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            stream.Dispose();
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
            using DataStream stream = new DataStream(baseStream);
            baseStream.Position = 1;
            stream.WriteByte(0xCA);
            baseStream.Position = 0;
            Assert.AreEqual(0xCA, baseStream.ReadByte());
        }

        [Test]
        public void WriteBufferAndIncreasePosition()
        {
            using DataStream stream = new DataStream(baseStream);
            byte[] buffer = { 0x00, 0xCA };
            stream.Write(buffer, 1, 1);
            Assert.AreEqual(1, stream.Position);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteBufferAndIncreaseLength()
        {
            baseStream.WriteByte(0xFF);
            using DataStream stream = new DataStream(baseStream);
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
            stream.Dispose();
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
            baseStream.WriteByte(0xFF);
            using DataStream stream = new DataStream(baseStream);
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
            stream.Dispose();
        }

        [Test]
        public void WriteBufferOutOfRangeThrowException()
        {
            DataStream stream = new DataStream();
            byte[] buffer = { 0xCA };
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 10, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => stream.Write(buffer, 0, 10));
            stream.Dispose();
        }

        [Test]
        public void WriteBufferWithZeroBytes()
        {
            using DataStream stream = new DataStream();
            byte[] buffer = { 0xCA };
            Assert.That(() => stream.Write(null, 0, 0), Throws.ArgumentNullException);
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(0, stream.Length);
            Assert.That(() => stream.Write(buffer, 10, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
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
            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void WriteToStreamGuards()
        {
            DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);

            Assert.Throws<ArgumentNullException>(() => stream1.WriteTo((Stream)null));

            DataStream disposedStream = new DataStream();
            disposedStream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => disposedStream.WriteTo(stream1));
            Assert.Throws<ObjectDisposedException>(() => stream1.WriteTo(disposedStream));

            stream1.Dispose();
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
            stream1.Dispose();
            stream2.Dispose();
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
            stream1.Dispose();
            stream2.Dispose();
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
            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void WriteToMoreThanOneBuffer()
        {
            const int SIZE = 80 * 1024;
            DataStream stream1 = new DataStream();
            for (int i = 0; i < SIZE; i++)
                stream1.WriteByte((byte)(i % 256));
            DataStream stream2 = new DataStream();
            stream1.WriteTo(stream2);
            Assert.IsTrue(stream1.Compare(stream2));

            stream1.Dispose();
            stream2.Dispose();
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

            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void WriteToFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x00);
            stream.WriteByte(0xFF);
            stream.WriteTo(tempFile);

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
            Assert.IsTrue(stream.Compare(fileStream));

            fileStream.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        [Ignore("Generates 6 GB of files and takes a lot of time (~1 min)")]
        [ExcludeFromCodeCoverage]
        public void WriteToLargeFiles()
        {
            const long Size = 3L * 1024 * 1024 * 1024; // 3 GB
            string inputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string outputFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            DataStream inputStream = null;
            DataStream outputStream = null;
            try {
                inputStream = DataStreamFactory.FromFile(inputFile, FileOpenMode.ReadWrite);
                byte[] buffer = new byte[70 * 1024]; // 70 KB (SOH)
                for (int i = 0; i < buffer.Length; i++) {
                    buffer[i] = (byte)(i % 256);
                }

                long written = 0;
                while (written < Size) {
                    int count = (Size - written) > buffer.Length
                        ? buffer.Length
                        : (int)(Size - written);
                    inputStream.Write(buffer, 0, count);
                    written += count;
                }

                inputStream.WriteTo(outputFile);

                outputStream = DataStreamFactory.FromFile(outputFile, FileOpenMode.Read);
                Assert.IsTrue(inputStream.Compare(outputStream));
            } finally {
                inputStream?.Dispose();
                outputStream?.Dispose();
                File.Delete(inputFile);
                File.Delete(outputFile);
            }
        }

        [Test]
        public void WriteToFileGuards()
        {
            using var stream = new DataStream();
            Assert.Throws<ArgumentNullException>(() => stream.WriteTo((string)null));
            Assert.Throws<ArgumentNullException>(() => stream.WriteTo(string.Empty));

            stream.Dispose();
            Assert.Throws<ObjectDisposedException>(() => stream.WriteTo("test"));
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

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
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

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
            Assert.That(() => stream.Compare(fileStream), Is.True);

            fileStream.Dispose();
            File.Delete(tempFile);
            stream.Dispose();

            Environment.CurrentDirectory = cwd;
        }

        [Test]
        public void WriteToWhenLengthZeroCreatesEmptyFile()
        {
            string tempFile = Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName(),
                Path.GetRandomFileName());

            DataStream stream = new DataStream();
            stream.WriteTo(tempFile);

            Assert.That(File.Exists(tempFile), Is.True);
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.Read);
            Assert.AreEqual(0, fs.Length);
            fs.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void WriteSegmentToStreamWithOffset()
        {
            using DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);

            using DataStream stream2 = new DataStream();
            stream1.WriteSegmentTo(2, stream2);

            stream2.Position = 0;
            Assert.IsTrue(stream2.Length == 2);
            Assert.AreEqual(0x00, stream2.ReadByte());
            Assert.AreEqual(0xFF, stream2.ReadByte());
        }

        [Test]
        public void WriteSegmentToFileWithOffset()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x00);
            stream.WriteByte(0xFF);
            stream.WriteSegmentTo(2, tempFile);

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
            Assert.That(fileStream.Length, Is.EqualTo(2));
            Assert.That(fileStream.ReadByte(), Is.EqualTo(0x00));
            Assert.That(fileStream.ReadByte(), Is.EqualTo(0xFF));

            fileStream.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void WriteSegmentToStreamWithOffsetAndLength()
        {
            using DataStream stream1 = new DataStream();
            stream1.WriteByte(0xCA);
            stream1.WriteByte(0xFE);
            stream1.WriteByte(0x00);
            stream1.WriteByte(0xFF);

            using DataStream stream2 = new DataStream();
            stream1.WriteSegmentTo(1, 2, stream2);

            stream2.Position = 0;
            Assert.IsTrue(stream2.Length == 2);
            Assert.AreEqual(0xFE, stream2.ReadByte());
            Assert.AreEqual(0x00, stream2.ReadByte());
        }

        [Test]
        public void WriteSegmentToFileWithOffsetAndLength()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            using DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x00);
            stream.WriteByte(0xFF);
            stream.WriteSegmentTo(1, 2, tempFile);

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
            Assert.That(fileStream.Length, Is.EqualTo(2));
            Assert.That(fileStream.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(fileStream.ReadByte(), Is.EqualTo(0x00));

            fileStream.Dispose();
            File.Delete(tempFile);
        }

        [Test]
        public void WriteSegmentToFileCreatesParentFolder()
        {
            string tempFile = Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName(),
                Path.GetRandomFileName());

            DataStream stream = new DataStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x01);
            stream.WriteByte(0x02);
            stream.WriteByte(0x03);
            stream.WriteByte(0x04);
            stream.WriteSegmentTo(0, 2, tempFile);

            DataStream fileStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
            using (var substream = new DataStream(stream, 0, 2)) {
                Assert.That(() => substream.Compare(fileStream), Is.True);
            }

            fileStream.Dispose();
            File.Delete(tempFile);
            stream.Dispose();
        }

        [Test]
        public void WriteSegmentToWithOffsetAndLengthLongerThanSOH()
        {
            DataStream stream1 = new DataStream();
            byte[] data = new byte[70 * 1024 * 2];
            Random random = new Random();
            random.NextBytes(data);
            for (int i = 0; i < data.Length; i++) {
                stream1.WriteByte(data[i]);
            }

            DataStream stream2 = new DataStream();
            stream1.WriteSegmentTo(0, data.Length - 10, stream2);
            stream2.Position = 0;
            Assert.AreEqual(data[0], stream2.ReadByte());
            Assert.AreEqual(data[1], stream2.ReadByte());
            Assert.IsTrue(stream2.Length == data.Length - 10);
            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void WriteSegmentToGuards()
        {
            using DataStream stream = new DataStream();
            stream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);

            using DataStream stream2 = new DataStream();

            DataStream disposedStream = new DataStream();
            disposedStream.Write(new byte[] { 5, 6, 7, 8 }, 0, 4);
            disposedStream.Dispose();

            // Null or empty
            Assert.That(
                () => stream.WriteSegmentTo(1, (string)null),
                Throws.ArgumentNullException);
            Assert.That(
                () => stream.WriteSegmentTo(1, string.Empty),
                Throws.ArgumentNullException);
            Assert.That(
                () => stream.WriteSegmentTo(1, (Stream)null),
                Throws.ArgumentNullException);
            Assert.That(
                () => stream.WriteSegmentTo(1, 2, (string)null),
                Throws.ArgumentNullException);
            Assert.That(
                () => stream.WriteSegmentTo(1, 2, string.Empty),
                Throws.ArgumentNullException);
            Assert.That(
                () => stream.WriteSegmentTo(1, 2, (Stream)null),
                Throws.ArgumentNullException);

            // Index out range
            Assert.That(
                () => stream.WriteSegmentTo(-2, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(5, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(-2, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(5, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(-2, 1, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(5, 0, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(1, -1, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(1, 5, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(3, 2, "test"),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(-2, 1, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(5, 0, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(1, -1, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(1, 5, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => stream.WriteSegmentTo(3, 2, stream2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());

            // From disposed stream
            Assert.That(
                () => disposedStream.WriteSegmentTo(1, "test"),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => disposedStream.WriteSegmentTo(1, stream),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => disposedStream.WriteSegmentTo(1, 2, "test"),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => disposedStream.WriteSegmentTo(1, 2, stream),
                Throws.InstanceOf<ObjectDisposedException>());

            // To disposed stream
            Assert.That(
                () => stream.WriteSegmentTo(1, disposedStream),
                Throws.InstanceOf<ObjectDisposedException>());
            Assert.That(
                () => stream.WriteSegmentTo(1, 2, disposedStream),
                Throws.InstanceOf<ObjectDisposedException>());
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

            stream1.Dispose();
            stream2.Dispose();
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

            stream1.Dispose();
            stream2.Dispose();
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

            stream1.Dispose();
            stream2.Dispose();
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
            stream1.Dispose();
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
            Assert.Throws<ObjectDisposedException>(() => stream2.Compare(stream1));
            stream1.Dispose();
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
            stream2.Dispose();
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
            stream1.Dispose();
            stream2.Dispose();
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

            stream1.Dispose();
            stream2.Dispose();
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

            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void CompareMoreThanOneBuffer()
        {
            DataStream stream1 = new DataStream();
            DataStream stream2 = new DataStream();
            byte[] data = new byte[80 * 1024];
            for (int i = 0; i < data.Length; i++) {
                data[i] = 0xAA;
            }

            stream1.Write(data, 0, data.Length);
            stream2.Write(data, 0, data.Length);

            Assert.That(stream1.Compare(stream2), Is.True);

            stream1.Position = (70 * 1024) + 128;
            stream1.WriteByte(0xBB);
            Assert.That(stream1.Compare(stream2), Is.False);

            stream1.Dispose();
            stream2.Dispose();
        }

        [Test]
        public void ReadAndWriteStreamsInParallel()
        {
            int streamCount = 100;
            int testDataLength = 1024;
            Random rdn = new Random();

            byte[] data = new byte[streamCount * testDataLength];
            for (int i = 0; i < data.Length; i++) {
                data[i] = (byte)rdn.Next(256);
            }

            byte[] zeros = new byte[streamCount * testDataLength];

            DataStream source = new DataStream();
            source.Write(data, 0, data.Length);

            DataStream[] streams = new DataStream[streamCount];
            for (int i = 0; i < streamCount; i++) {
                streams[i] = new DataStream(source, testDataLength * i, testDataLength);
            }

            DataStream result = new DataStream();
            result.Write(zeros, 0, zeros.Length);
            result.Position = 0;

            DataStream[] writeStreams = new DataStream[streamCount];
            for (int i = 0; i < streamCount; i++) {
                writeStreams[i] = new DataStream(result, testDataLength * i, testDataLength);
            }

            byte[][] read = new byte[streamCount][];
            for (int i = 0; i < streamCount; i++) {
                read[i] = new byte[testDataLength];
            }

            Parallel.For(0, streamCount, i => {
                streams[i].Read(read[i], 0, testDataLength);
            });

            Parallel.For(0, streamCount, i => {
                writeStreams[i].Write(read[i], 0, testDataLength);
            });

            for (int i = 0; i < streamCount; i++) {
                Assert.That(streams[i].Compare(writeStreams[i]), Is.True);
                streams[i].Dispose();
                writeStreams[i].Dispose();
            }

            Assert.That(source.Compare(result), Is.True);
            source.Dispose();
            result.Dispose();
        }

        [Test]
        public void TestStreamCanReadWriteAndSeek()
        {
            var stream = new DataStream();
            Assert.That(stream.CanRead, Is.True);
            Assert.That(stream.CanWrite, Is.True);
            Assert.That(stream.CanSeek, Is.True);
        }

        [Test]
        public void CannotTimeOutButDoesNotThrowException()
        {
            // Important to prevent exceptions in reflection UI controls
            using var stream = new DataStream();
            Assert.That(stream.CanTimeout, Is.False);
            Assert.That(stream.ReadTimeout, Is.EqualTo(-1));
            Assert.That(stream.WriteTimeout, Is.EqualTo(-1));
            Assert.That(() => stream.ReadTimeout = 300, Throws.InstanceOf<InvalidOperationException>());
            Assert.That(() => stream.WriteTimeout = 300, Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void TestStreamFlushGuards()
        {
            var stream = new DataStream();
            stream.Dispose();
            Assert.That(() => stream.Flush(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void TestFlushCallsStreamFlush()
        {
            var innerStream = new Mock<Stream>();

            using var stream = new DataStream(innerStream.Object);
            stream.Flush();
            innerStream.Verify(s => s.Flush(), Times.Once);
        }

        public class DummyBinaryConverter : IConverter<BinaryFormat, byte>
        {
            public byte Convert(BinaryFormat source)
            {
                return (byte)source.Stream.ReadByte();
            }
        }
    }
}
