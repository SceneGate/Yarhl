//
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
namespace Libgame.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Data stream.
    /// </summary>
    /// <remarks>
    /// Custom implementation of a Stream based on System.IO.Stream.
    /// </remarks>
    public class DataStream : IDisposable
    {
        static readonly Dictionary<Stream, int> Instances = new Dictionary<Stream, int>();
        long position;
        long length;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <param name="offset">Offset from the base stream origin.</param>
        /// <param name="length">
        /// Length of this DataStream.
        /// If it's -1 then it takes the stream length.
        /// </param>
        public DataStream(Stream stream, long offset, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (offset < 0 || offset > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < -1 || offset + length > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (!Instances.ContainsKey(stream))
                Instances.Add(stream, 1);
            else
                Instances[stream] += 1;

            BaseStream = stream;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        public DataStream(Stream stream)
            : this(stream, 0, -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        public DataStream()
            : this(new MemoryStream())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <param name="mode">File open mode.</param>
        public DataStream(string filePath, FileOpenMode mode)
            : this(new FileStream(filePath, mode.ToFileMode(), mode.ToFileAccess()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataStream"/> class.
        /// </summary>
        /// <param name="stream">Base stream.</param>
        /// <param name="offset">Offset from the DataStream start.</param>
        /// <param name="length">
        /// Length of this DataStream.
        /// If it's -1 then it takes the stream length.
        /// </param>
        public DataStream(DataStream stream, long offset, long length)
            : this(stream?.BaseStream, offset + (stream?.Offset ?? 0), length)
        {
        }

        ~DataStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="DataStream"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed {
            get;
            private set;
        }

        /// <summary>
        /// Gets the offset from the BaseStream.
        /// </summary>
        /// <value>The offset.</value>
        public long Offset {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the position from the start of this stream.
        /// </summary>
        /// <value>The position.</value>
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
        /// If the value set is -1, then the length is taken from the BaseStream.
        /// </summary>
        /// <value>The length.</value>
        public long Length {
            get {
                return length;
            }

            set {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(DataStream));
                if (value < -1 || Offset + value > BaseStream.Length)
                    throw new ArgumentOutOfRangeException(nameof(value));

                length = (value != -1) ? value : BaseStream.Length;

                if (Position > Length)
                    Position = Length;
            }
        }

        /// <summary>
        /// Gets the base stream.
        /// </summary>
        /// <value>The base stream.</value>
        public Stream BaseStream {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether the position is at end of the stream.
        /// </summary>
        /// <value><c>true</c> if end of stream; otherwise, <c>false</c>.</value>
        public bool EndOfStream {
            get {
                return Position >= Length;
            }
        }

        /// <summary>
        /// Gets the position from the base stream.
        /// </summary>
        /// <value>The absolute position.</value>
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
        /// <param name="mode">Start to move position.</param>
        public void Seek(long shift, SeekMode mode)
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
            }
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public byte ReadByte()
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
        public int Read(byte[] buffer, int index, int count)
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
        /// Writes a byte.
        /// </summary>
        /// <param name="val">Byte value.</param>
        public void WriteByte(byte val)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            BaseStream.Position = AbsolutePosition;
            BaseStream.WriteByte(val);

            if (Position == Length)
                Length++;
            Position++;
        }

        /// <summary>
        /// Writes the a portion of the buffer to the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="count">Bytes to write.</param>
        public void Write(byte[] buffer, int index, int count)
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

        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            if (Disposed)
                return;

            Disposed = true;

            // BaseStream will be null if the constructor throws exception
            if (BaseStream != null) {
                Instances[BaseStream] -= 1;
                if (freeManagedResourcesAlso && Instances[BaseStream] == 0)
                    BaseStream.Dispose();
            }
        }
    }
}
