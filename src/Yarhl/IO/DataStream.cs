// DataStream.cs
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
namespace Yarhl.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Yarhl.FileFormat;

    /// <summary>
    /// Data stream.
    /// </summary>
    /// <remarks>
    /// <para>Custom implementation of a Stream based on System.IO.Stream.</para>
    /// </remarks>
    public class DataStream : IStream, IDisposable
    {
        static readonly Dictionary<Stream, int> Instances = new Dictionary<Stream, int>();
        readonly Stack<long> positionStack = new Stack<long>();
        readonly bool isSubstream;
        long position;
        long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// A new stream is created in the memory.
        /// </summary>
        public DataStream()
        {
            BaseStream = new MemoryStream();
            isSubstream = false;
            Offset = 0;
            this.length = 0;

            IncreaseStreamCounter(BaseStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        public DataStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BaseStream = stream;
            isSubstream = false;
            Offset = 0;
            this.length = stream.Length;

            IncreaseStreamCounter(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <param name="offset">Offset from the base stream.</param>
        /// <param name="length">Length of this DataStream.</param>
        public DataStream(Stream stream, long offset, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (offset < 0 || offset > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            BaseStream = stream;
            isSubstream = true;
            Offset = offset;
            this.length = length;

            IncreaseStreamCounter(stream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream" /> class.
        /// </summary>
        /// <param name="data">Stream data buffer.</param>
        /// <param name="offset">Offset of the data in the buffer.</param>
        /// <param name="length">Length of the data.</param>
        public DataStream(byte[] data, int offset, int length)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > data.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            BaseStream = new MemoryStream(data, 0, data.Length);
            isSubstream = true;
            Offset = offset;
            this.length = length;

            IncreaseStreamCounter(BaseStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="mode">File open mode.</param>
        public DataStream(string filePath, FileOpenMode mode)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            BaseStream = new FileStream(filePath, mode.ToFileMode(), mode.ToFileAccess());
            isSubstream = false;
            Offset = 0;
            this.length = BaseStream.Length;

            IncreaseStreamCounter(BaseStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="mode">File open mode.</param>
        /// <param name="offset">Offset from the start of the file.</param>
        /// <param name="length">Length of this DataStream.</param>
        public DataStream(string filePath, FileOpenMode mode, long offset, long length)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            long fileSize = new FileInfo(filePath).Length;
            if (offset < 0 || offset > fileSize)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > fileSize)
                throw new ArgumentOutOfRangeException(nameof(length));

            BaseStream = new FileStream(filePath, mode.ToFileMode(), mode.ToFileAccess());
            isSubstream = true;
            Offset = offset;
            this.length = length;

            IncreaseStreamCounter(BaseStream);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <param name="offset">Offset from the DataStream start.</param>
        /// <param name="length">Length of this DataStream.</param>
        public DataStream(DataStream stream, long offset, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (offset < 0 || offset > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < 0 || offset + length > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            ParentDataStream = stream;
            BaseStream = stream.BaseStream;
            isSubstream = true;
            Offset = stream.Offset + offset;
            this.length = length;

            IncreaseStreamCounter(BaseStream);
        }

        /// <summary>
        /// Gets the number of streams in use.
        /// </summary>
        public static int ActiveStreams => Instances.Count;

        /// <summary>
        /// Gets a value indicating whether this <see cref="DataStream"/> is disposed.
        /// </summary>
        public bool Disposed {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset from the BaseStream.
        /// </summary>
        public long Offset {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the position from the start of this stream.
        /// </summary>
        public long Position {
            get {
                return position;
            }

            set {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(DataStream));
                if (value < 0 || value > Length)
                    throw new ArgumentOutOfRangeException(nameof(value));

                position = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of this stream.
        /// </summary>
        public long Length {
            get {
                return length;
            }

            set {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(DataStream));
                if (value < 0 || Offset + value > BaseStream.Length)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (isSubstream) {
                    throw new InvalidOperationException(
                        "Offset is not 0. " +
                        "Cannot change the size of sub-streams.");
                }

                length = value;
                if (Position > Length) {
                    Position = Length;
                }
            }
        }

        /// <summary>
        /// Gets the parent DataStream.
        /// </summary>
        public DataStream ParentDataStream {
            get;
            private set;
        }

        /// <summary>
        /// Gets the base stream.
        /// </summary>
        public Stream BaseStream {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the position is at end of the stream.
        /// </summary>
        public bool EndOfStream {
            get {
                return Position >= Length;
            }
        }

        /// <summary>
        /// Gets the position from the base stream.
        /// </summary>
        public long AbsolutePosition {
            get { return Offset + Position; }
        }

        /// <summary>
        /// Releases all resource used by the <see cref="DataStream"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Move the position of the Stream.
        /// </summary>
        /// <param name="shift">Distance to move position.</param>
        /// <param name="mode">Mode to move position.</param>
        public void Seek(long shift, SeekMode mode = SeekMode.Start)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            switch (mode) {
            case SeekMode.Current:
                Position += shift;
                break;
            case SeekMode.Start:
                Position = shift;
                break;
            case SeekMode.End:
                Position = Length - shift;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        /// <summary>
        /// Push the current position into a stack and move to a new one.
        /// </summary>
        /// <param name="shift">Distance to move position.</param>
        /// <param name="mode">Mode to move position.</param>
        public void PushToPosition(long shift, SeekMode mode = SeekMode.Start)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            positionStack.Push(Position);
            Seek(shift, mode);
        }

        /// <summary>
        /// Push the current position into a stack.
        /// </summary>
        public void PushCurrentPosition()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            positionStack.Push(Position);
        }

        /// <summary>
        /// Pop the last position from the stack and move to it.
        /// </summary>
        public void PopPosition()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (positionStack.Count == 0)
                throw new InvalidOperationException("Position stack is empty");

            Position = positionStack.Pop();
        }

        /// <summary>
        /// Run a method in a specific position.
        /// This command will move into the position, run the method and return
        /// to the current position.
        /// </summary>
        /// <param name="action">Action to run.</param>
        /// <param name="position">Position to move.</param>
        /// <param name="mode">Mode to move position.</param>
        public void RunInPosition(
            Action action,
            long position,
            SeekMode mode = SeekMode.Start)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            PushToPosition(position, mode);
            action();
            PopPosition();
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public virtual byte ReadByte()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (Position >= Length)
                throw new EndOfStreamException();

            BaseStream.Position = AbsolutePosition;
            Position++;
            return (byte)BaseStream.ReadByte();
        }

        /// <summary>
        /// Reads from the stream to the buffer.
        /// </summary>
        /// <returns>The number of bytes read.</returns>
        /// <param name="buffer">Buffer to copy data.</param>
        /// <param name="index">Index to start copying in buffer.</param>
        /// <param name="count">Number of bytes to read.</param>
        public virtual int Read(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (count == 0)
                return 0;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (Position + count > Length)
                throw new EndOfStreamException();

            BaseStream.Position = AbsolutePosition;
            int read = BaseStream.Read(buffer, index, count);
            Position += count;

            return read;
        }

        /// <summary>
        /// Reads a format from this stream.
        /// </summary>
        /// <returns>The format read.</returns>
        /// <typeparam name="T">The type of the format to read.</typeparam>
        public T ReadFormat<T>()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            T format;
            using (var binary = new BinaryFormat(this))
                format = ConvertFormat.To<T>(binary);
            return format;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">Byte value.</param>
        public virtual void WriteByte(byte data)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            BaseStream.Position = AbsolutePosition;
            BaseStream.WriteByte(data);

            if (Position == Length) {
                Length++;
            }

            Position++;
        }

        /// <summary>
        /// Writes the a portion of the buffer to the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="count">Bytes to write.</param>
        public virtual void Write(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (count == 0)
                return;

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));
            if (index + count > buffer.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            BaseStream.Position = AbsolutePosition;
            BaseStream.Write(buffer, index, count);

            if (Position + count > Length)
                Length = Position + count;
            Position += count;
        }

        /// <summary>
        /// Writes the stream into a file.
        /// </summary>
        /// <param name="fileOut">Output file path.</param>
        public void WriteTo(string fileOut)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (string.IsNullOrEmpty(fileOut))
                throw new ArgumentNullException(nameof(fileOut));

            // Parent dir can be empty if we just specified the file name.
            // In that case, the folder (cwd) already exists.
            string parentDir = Path.GetDirectoryName(fileOut);
            if (!string.IsNullOrEmpty(parentDir))
                Directory.CreateDirectory(parentDir);

            using (var stream = new DataStream(fileOut, FileOpenMode.Write))
                WriteTo(stream);
        }

        /// <summary>
        /// Writes the stream into another DataStream.
        /// </summary>
        /// <param name="stream">Output DataStream.</param>
        public void WriteTo(DataStream stream)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (stream.Disposed)
                throw new ObjectDisposedException(nameof(stream));

            long currPos = Position;
            Seek(0, SeekMode.Start);

            const int BufferSize = 5 * 1024;
            byte[] buffer = new byte[BufferSize];

            int written = 0;
            int bytesToRead = 0;
            do {
                if (written + BufferSize > Length)
                    bytesToRead = (int)(Length - written);
                else
                    bytesToRead = BufferSize;

                written += Read(buffer, 0, bytesToRead);
                stream.Write(buffer, 0, bytesToRead);
            } while (written != Length);

            Seek(currPos, SeekMode.Start);
        }

        /// <summary>
        /// Compare the content of the stream with another one.
        /// </summary>
        /// <returns>The result of the comparaison.</returns>
        /// <param name="otherStream">Stream to compare with.</param>
        public bool Compare(DataStream otherStream)
        {
            if (otherStream == null)
                throw new ArgumentNullException(nameof(otherStream));
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));
            if (otherStream.Disposed)
                throw new ObjectDisposedException(nameof(otherStream));

            if (Length != otherStream.Length)
                return false;

            long startPosition = Position;
            long otherStreamStartPosition = otherStream.position;
            Seek(0, SeekMode.Start);
            otherStream.Seek(0, SeekMode.Start);

            bool result = true;
            while (!EndOfStream && result) {
                if (ReadByte() != otherStream.ReadByte())
                    result = false;
            }

            Seek(startPosition, SeekMode.Start);
            otherStream.Seek(otherStreamStartPosition, SeekMode.Start);

            return result;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="DataStream"/>
        /// object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">If set to
        /// <see langword="true" /> free managed resources also.</param>
        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            if (Disposed)
                return;

            Disposed = true;

            // BaseStream will be null if the constructor throws exception
            if (BaseStream != null) {
                Instances[BaseStream] -= 1;
                if (freeManagedResourcesAlso && Instances[BaseStream] == 0) {
                    BaseStream.Dispose();
                    Instances.Remove(BaseStream);
                }
            }
        }

        private static void IncreaseStreamCounter(Stream stream)
        {
            if (!Instances.ContainsKey(stream))
                Instances.Add(stream, 1);
            else
                Instances[stream] += 1;
        }
    }
}
