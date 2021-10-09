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
namespace Yarhl.IO.StreamFormat
{
    using System;
    using System.IO;

    /// <summary>
    /// Wrapper over .NET streams.
    /// </summary>
    public class StreamWrapper : Stream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapper" /> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        public StreamWrapper(Stream stream)
        {
            BaseStream = stream;
            LockObj = new object();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapper" /> class.
        /// </summary>
        protected StreamWrapper()
        {
            LockObj = new object();
        }

        /// <summary>
        /// Gets or sets the base stream.
        /// </summary>
        public Stream BaseStream {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the lock object of this stream (and all its substreams).
        /// </summary>
        public object LockObj {
            get;
        }

        /// <summary>
        /// Gets or sets the position from the start of this stream.
        /// </summary>
        public override long Position {
            get;
            set;
        }

        /// <summary>
        /// Gets the length of this stream.
        /// </summary>
        public override long Length => BaseStream.Length;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStream" />
        /// has been disposed.
        /// </summary>
        public bool Disposed {
            get;
            private set;
        }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="length">The new length of the stream.</param>
        public override void SetLength(long length)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            BaseStream.SetLength(length);
        }

        /// <inheritdoc />
        public override void Flush()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            BaseStream.Flush();
        }

        /// <summary>
        /// Reads from the stream to the buffer.
        /// </summary>
        /// <returns>The number of bytes read.</returns>
        /// <param name="buffer">Buffer to copy data.</param>
        /// <param name="index">Index to start copying in buffer.</param>
        /// <param name="count">Number of bytes to read.</param>
        public override int Read(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            BaseStream.Position = Position;
            int read = BaseStream.Read(buffer, index, count);
            Position += read;
            return read;
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public override int ReadByte()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            long position = Position;
            if (BaseStream.Position != position) {
                BaseStream.Position = position;
            }

            Position = position + 1;
            return BaseStream.ReadByte();
        }

        /// <summary>
        /// Writes the a portion of the buffer to the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="count">Bytes to write.</param>
        public override void Write(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            BaseStream.Position = Position;
            BaseStream.Write(buffer, index, count);
            Position += count;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">Byte value.</param>
        public override void WriteByte(byte data)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(StreamWrapper));

            BaseStream.Position = Position;
            BaseStream.WriteByte(data);
            Position++;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="StreamWrapper"/>
        /// object.
        /// </summary>
        /// <param name="freeManaged">If set to
        /// <see langword="true" /> free managed resources also.</param>
        protected override void Dispose(bool freeManaged)
        {
            if (Disposed) {
                return;
            }

            Disposed = true;
            if (freeManaged) {
                BaseStream?.Dispose();
            }
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();
    }
}
