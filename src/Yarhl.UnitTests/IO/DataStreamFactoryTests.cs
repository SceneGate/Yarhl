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
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Yarhl.IO;
    using Yarhl.IO.StreamFormat;

    [TestFixture]
    public class DataStreamFactoryTests
    {
        [Test]
        public void CreateFromStreamUseStream()
        {
            var stream = new MemoryStream();
            using var dataStream = DataStreamFactory.FromStream(stream);

            Assert.That(dataStream.BaseStream, Is.SameAs(stream));
        }

        [Test]
        public void CreateFromStreamAllowsToExpand()
        {
            using var stream = new MemoryStream();
            var dataStream = DataStreamFactory.FromStream(stream);
            Assert.That(() => stream.WriteByte(0xFE), Throws.Nothing);
            Assert.That(stream.Length, Is.EqualTo(1));
            dataStream.Dispose();
        }

        [Test]
        public void CreateFromStreamWithDataStreamReuseBaseStream()
        {
            using var stream = new MemoryStream();
            using var dataStream1 = DataStreamFactory.FromStream(stream);
            using var dataStream2 = DataStreamFactory.FromStream(dataStream1);

            Assert.That(dataStream1.BaseStream, Is.SameAs(dataStream2.BaseStream));

            // Especially important check for thread-safety
            Assert.That(dataStream1.InternalInfo.LockObj, Is.SameAs(dataStream2.InternalInfo.LockObj));
        }

        [Test]
        public void CreateFromStreamThrowIfInvalidArgument()
        {
            Assert.That(
                () => DataStreamFactory.FromStream(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromSubStreamUseStream()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBE);
            using var dataStream = DataStreamFactory.FromStream(stream, 1, 2);

            Assert.That(dataStream.BaseStream, Is.SameAs(stream));
            Assert.That(dataStream.Position, Is.EqualTo(0));
            Assert.That(dataStream.Offset, Is.EqualTo(1));
            Assert.That(dataStream.Length, Is.EqualTo(2));
        }

        [Test]
        public void CreateFromSubStreamDoesNotAllowToExpand()
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBE);
            var dataStream = DataStreamFactory.FromStream(stream, 1, 2);

            dataStream.Position = 2;
            Assert.That(() => dataStream.WriteByte(0xAA), Throws.InvalidOperationException);
        }

        [Test]
        public void CreateFromSubStreamTransferOwnership()
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            int beforeCount = DataStream.ActiveStreams;

            var dataStream = DataStreamFactory.FromStream(stream, 0, 1);
            Assert.That(DataStream.ActiveStreams, Is.EqualTo(beforeCount + 1));

            dataStream.Dispose();
            Assert.That(() => stream.ReadByte(), Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void CreateFromSubStreamWithDataStreamReuseBaseStream()
        {
            using var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
            using var dataStream1 = DataStreamFactory.FromStream(stream, 1, 2);
            using var dataStream2 = DataStreamFactory.FromStream(dataStream1, 1, 1);

            Assert.That(dataStream1.BaseStream, Is.SameAs(dataStream2.BaseStream));

            // Especially important check for thread-safety
            Assert.That(dataStream1.InternalInfo.LockObj, Is.SameAs(dataStream2.InternalInfo.LockObj));
        }

        [Test]
        public void CreateFromSubStreamThrowIfInvalidArgument()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);

            Assert.That(
                () => DataStreamFactory.FromStream(null, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => DataStreamFactory.FromStream(stream, -1, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStream(stream, 0, -1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStream(stream, 3, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStream(stream, 1, 2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());

            stream.Dispose();
        }

        [Test]
        public void CreateFromSubStreamKeepingOwnershipUseStream()
        {
            using var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);
            stream.WriteByte(0xBE);
            using var dataStream = DataStreamFactory.FromStreamKeepingOwnership(stream, 1, 2);

            Assert.That(dataStream.BaseStream, Is.SameAs(stream));
            Assert.That(dataStream.Position, Is.EqualTo(0));
            Assert.That(dataStream.Offset, Is.EqualTo(1));
            Assert.That(dataStream.Length, Is.EqualTo(2));
        }

        [Test]
        public void CreateFromSubStreamKeepsOwnership()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            int beforeCount = DataStream.ActiveStreams;

            var dataStream = DataStreamFactory.FromStreamKeepingOwnership(stream, 0, 1);
            Assert.That(DataStream.ActiveStreams, Is.EqualTo(beforeCount));

            dataStream.Dispose();
            Assert.That(() => stream.ReadByte(), Throws.Nothing);
            stream.Dispose();
        }

        [Test]
        public void CreateFromSubStreamKeepingOwnershipWithDataStreamReuseBaseStream()
        {
            using var stream = new MemoryStream();
            stream.Write(new byte[] { 1, 2, 3, 4 }, 0, 4);
            using var dataStream1 = DataStreamFactory.FromStreamKeepingOwnership(stream, 1, 2);
            using var dataStream2 = DataStreamFactory.FromStreamKeepingOwnership(dataStream1, 1, 1);

            Assert.That(dataStream1.BaseStream, Is.SameAs(dataStream2.BaseStream));

            // Especially important check for thread-safety
            Assert.That(dataStream1.InternalInfo.LockObj, Is.SameAs(dataStream2.InternalInfo.LockObj));
        }

        [Test]
        public void CreateFromSubStreamKeepingOwnershipThrowIfInvalidArgument()
        {
            var stream = new MemoryStream();
            stream.WriteByte(0xCA);
            stream.WriteByte(0xFE);

            Assert.That(
                () => DataStreamFactory.FromStreamKeepingOwnership(null, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => DataStreamFactory.FromStreamKeepingOwnership(stream, -1, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStreamKeepingOwnership(stream, 0, -1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStreamKeepingOwnership(stream, 3, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromStreamKeepingOwnership(stream, 1, 2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());

            stream.Dispose();
        }

        [Test]
        public void CreateFromMemoryUseMemoryStream()
        {
            var dataStream = DataStreamFactory.FromMemory();

            Assert.That(dataStream.BaseStream, Is.AssignableFrom<RecyclableMemoryStream>());

            dataStream.Dispose();
        }

        [Test]
        public void CreateFromMemoryAllowToExpand()
        {
            var stream = DataStreamFactory.FromMemory();
            Assert.That(() => stream.WriteByte(0xFE), Throws.Nothing);
            stream.Dispose();
        }

        [Test]
        public void CreateFromArray()
        {
            byte[] data = new byte[] { 0x01, 0x02 };
            using var stream = DataStreamFactory.FromArray(data);
            Assert.That(stream.Position, Is.EqualTo(0));
            Assert.That(stream.Length, Is.EqualTo(2));
            Assert.That(stream.ReadByte(), Is.EqualTo(0x01));
            Assert.That(stream.ReadByte(), Is.EqualTo(0x02));
        }

        [Test]
        public void CreateFromArrayGuards()
        {
            Assert.That(() => DataStreamFactory.FromArray(null), Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromArrayReadsArray()
        {
            byte[] data = new byte[] { 0x01, 0x2, 0x3 };

            var stream = DataStreamFactory.FromArray(data, 1, 2);
            Assert.AreEqual(0, stream.Position);
            Assert.AreEqual(1, stream.Offset);
            Assert.AreEqual(2, stream.Length);
            Assert.That(stream.ReadByte(), Is.EqualTo(2));
            stream.Dispose();
        }

        [Test]
        public void CreateFromArrayWritesToArray()
        {
            byte[] data = new byte[] { 0x01, 0x2, 0x3 };
            var stream = DataStreamFactory.FromArray(data, 1, 2);

            stream.WriteByte(0xFE);
            Assert.That(data[1], Is.EqualTo(0xFE));
            stream.Dispose();
        }

        [Test]
        public void CreateFromArrayDoesNotAllowToExpand()
        {
            byte[] data = new byte[] { 0x01, 0x2, 0x3 };
            var stream = DataStreamFactory.FromArray(data, 1, 2);

            stream.Position = 2;
            Assert.That(() => stream.WriteByte(0xFE), Throws.InvalidOperationException);
            stream.Dispose();
        }

        [Test]
        public void CreateFromArrayWithInvalidThrows()
        {
            byte[] data = new byte[] { 0x01 };

            Assert.That(
                () => DataStreamFactory.FromArray(null, 0, 0),
                Throws.ArgumentNullException);
            Assert.That(
                () => DataStreamFactory.FromArray(data, -1, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromArray(data, 2, 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromArray(data, 0, -1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromArray(data, 0, 2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(
                () => DataStreamFactory.FromArray(data, 1, 1),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CreateFromPathWritesFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);

            int beforeCount = DataStream.ActiveStreams;
            DataStream writeStream = null;
            FileStream readStream = null;
            try {
                writeStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Write);
                Assert.AreEqual(0x0, writeStream.Offset);
                Assert.AreEqual(0x0, writeStream.Length);
                Assert.AreEqual(0x0, writeStream.Position);
                writeStream.WriteByte(0xCA);
                writeStream.Dispose();
                writeStream = null; // prevent two dispose

                Assert.That(File.Exists(tempFile), Is.True);
                readStream = new FileStream(tempFile, FileMode.Open);
                Assert.AreEqual(0x1, readStream.Length);
                Assert.AreEqual(0xCA, readStream.ReadByte());
            } finally {
                writeStream?.Dispose();
                readStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromPathAllowsToExpand()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            int beforeCount = DataStream.ActiveStreams;
            DataStream writeStream = null;
            try {
                writeStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Write);
                Assert.That(() => writeStream.WriteByte(0xCA), Throws.Nothing);
            } finally {
                writeStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromPathReadsFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);

            int beforeCount = DataStream.ActiveStreams;
            FileStream writeStream = null;
            DataStream readStream = null;
            try {
                writeStream = new FileStream(tempFile, FileMode.CreateNew);
                Assert.AreEqual(0x0, writeStream.Length);
                writeStream.WriteByte(0xCA);
                writeStream.Dispose();
                writeStream = null; // prevent two disposes

                readStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read);
                Assert.IsNull(readStream.ParentDataStream);
                Assert.AreEqual(0x0, readStream.Offset);
                Assert.AreEqual(0x1, readStream.Length);
                Assert.AreEqual(0x0, readStream.Position);
                Assert.AreEqual(0xCA, readStream.ReadByte());
            } finally {
                writeStream?.Dispose();
                readStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromPathWithInvalidArgThrows()
        {
            Assert.That(
                () => DataStreamFactory.FromFile(null, FileOpenMode.Append),
                Throws.ArgumentNullException);
            Assert.That(
                () => DataStreamFactory.FromFile(string.Empty, FileOpenMode.Append),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CreateFromPathChecksIfFileExists()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Read),
                Throws.InstanceOf<FileNotFoundException>());
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Write),
                Throws.Nothing);
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.ReadWrite),
                Throws.Nothing);
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Append),
                Throws.Nothing);

            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 0, 0),
                Throws.InstanceOf<FileNotFoundException>());
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Write, 0, 0),
                Throws.Nothing);
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.ReadWrite, 0, 0),
                Throws.Nothing);
            Assert.That(
                () => DataStreamFactory.FromFile(tempFile, FileOpenMode.Append, 0, 0),
                Throws.Nothing);
        }

        [Test]
        public void CreateFromSectionPathWritesFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);

            int beforeCount = DataStream.ActiveStreams;
            FileStream createStream = null;
            DataStream writeStream = null;
            FileStream readStream = null;
            try {
                createStream = new FileStream(tempFile, FileMode.CreateNew);
                createStream.WriteByte(0xCA);
                createStream.WriteByte(0xFE);
                createStream.WriteByte(0xAA);
                createStream.Dispose();
                createStream = null; // prevent two disposes

                writeStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Write, 1, 2);
                Assert.AreEqual(0x1, writeStream.Offset);
                Assert.AreEqual(0x2, writeStream.Length);
                Assert.AreEqual(0x0, writeStream.Position);
                writeStream.WriteByte(0xB4);
                writeStream.Dispose();
                writeStream = null; // prevent two dispose

                readStream = new FileStream(tempFile, FileMode.Open);
                readStream.Position = 1;
                Assert.AreEqual(0xB4, readStream.ReadByte());
            } finally {
                createStream?.Dispose();
                writeStream?.Dispose();
                readStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromSectionPathDoesNotAllowToExpand()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            int beforeCount = DataStream.ActiveStreams;
            FileStream createStream = null;
            DataStream writeStream = null;
            try {
                createStream = new FileStream(tempFile, FileMode.CreateNew);
                createStream.WriteByte(0xCA);
                createStream.WriteByte(0xFE);
                createStream.WriteByte(0xAA);
                createStream.Dispose();
                createStream = null; // prevent two disposes

                writeStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Write, 1, 2);
                writeStream.Position = 2;
                Assert.That(() => writeStream.WriteByte(0xB4), Throws.InvalidOperationException);
            } finally {
                createStream?.Dispose();
                writeStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromSectionPathReadsFile()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(tempFile), Is.False);

            int beforeCount = DataStream.ActiveStreams;
            FileStream writeStream = null;
            DataStream readStream = null;
            try {
                writeStream = new FileStream(tempFile, FileMode.CreateNew);
                writeStream.WriteByte(0xCA);
                writeStream.WriteByte(0xFE);
                writeStream.WriteByte(0xAA);
                writeStream.Dispose();
                writeStream = null; // prevent two disposes

                readStream = DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 1, 2);
                Assert.IsNull(readStream.ParentDataStream);
                Assert.AreEqual(0x1, readStream.Offset);
                Assert.AreEqual(0x2, readStream.Length);
                Assert.AreEqual(0x0, readStream.Position);
                Assert.AreEqual(0xFE, readStream.ReadByte());
            } finally {
                writeStream?.Dispose();
                readStream?.Dispose();
                File.Delete(tempFile);
            }

            Assert.AreEqual(beforeCount, DataStream.ActiveStreams);
        }

        [Test]
        public void CreateFromSectionPathInvalidArgsThrows()
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try {
                using (var stream = new FileStream(tempFile, FileMode.CreateNew)) {
                    stream.WriteByte(0xCA);
                    stream.WriteByte(0xFE);
                }

                Assert.Throws<ArgumentNullException>(() =>
                    DataStreamFactory.FromFile(null, FileOpenMode.Append, 0, 0));
                Assert.Throws<ArgumentNullException>(() =>
                    DataStreamFactory.FromFile(string.Empty, FileOpenMode.Append, 0, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, -1, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 3, 0));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 0, -1));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 0, 3));
                Assert.Throws<ArgumentOutOfRangeException>(() =>
                    DataStreamFactory.FromFile(tempFile, FileOpenMode.Read, 1, 2));
            } finally {
                File.Delete(tempFile);
            }
        }

        [Test]
        public void CreateFromSymlinkPathRedirectsToTarget()
        {
            string originalFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string symlinkFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(originalFile), Is.False);
            Assert.That(File.Exists(symlinkFile), Is.False);

            try {
                File.WriteAllBytes(originalFile, new byte[] { 0xCA, 0xFE, 0xBA, 0xBE });
                Assert.That(File.Exists(originalFile), Is.True);

                CreateSymlinkFile(originalFile, symlinkFile);
                Assert.That(File.Exists(symlinkFile), Is.True);

                using var symlinkStream = DataStreamFactory.FromFile(symlinkFile, FileOpenMode.Read);
                Assert.That(symlinkStream.Length, Is.EqualTo(4));
                Assert.That(symlinkStream.ReadByte(), Is.EqualTo(0xCA));
            } finally {
                File.Delete(symlinkFile);
                File.Delete(originalFile);
            }
        }

        [Test]
        public void CreateFromSymlinkSectionPathRedirectsToTarget()
        {
            string originalFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            string symlinkFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Assert.That(File.Exists(originalFile), Is.False);
            Assert.That(File.Exists(symlinkFile), Is.False);

            try {
                File.WriteAllBytes(originalFile, new byte[] { 0xCA, 0xFE, 0xBA, 0xBE });
                Assert.That(File.Exists(originalFile), Is.True);

                CreateSymlinkFile(originalFile, symlinkFile);
                Assert.That(File.Exists(symlinkFile), Is.True);

                using var symlinkStream = DataStreamFactory.FromFile(symlinkFile, FileOpenMode.Read, 2, 2);
                Assert.That(symlinkStream.Length, Is.EqualTo(2));
                Assert.That(symlinkStream.ReadByte(), Is.EqualTo(0xBA));
            } finally {
                File.Delete(symlinkFile);
                File.Delete(originalFile);
            }
        }

        private void CreateSymlinkFile(string originalFile, string symlinkFile)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                using var p = Process.Start("cmd.exe", $"/C mklink {symlinkFile} {originalFile}");
                p.WaitForExit();
                Assert.That(p.ExitCode, Is.EqualTo(0));
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                using var p = Process.Start("ln", $"-s {originalFile} {symlinkFile}");
                p.WaitForExit();
                Assert.That(p.ExitCode, Is.EqualTo(0));
            } else {
                Assert.Inconclusive("Unknown how to create symlinks for this platform");
            }
        }
    }
}
