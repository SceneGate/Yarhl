// Copyright (c) 2019 SceneGate Team
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
namespace Yarhl.IO.StreamFormat
{
    using System;
    using System.IO;

    /// <summary>
    /// Wrapper over .NET streams.
    /// </summary>
    public class StreamWrapper : IStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapper" /> class.
        /// </summary>
        /// <param name="stream">The stream to wrap.</param>
        public StreamWrapper(Stream stream)
        {
            BaseStream = stream;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamWrapper" /> class.
        /// </summary>
        protected StreamWrapper()
        {
        }

        /// <summary>
        /// Gets or sets the base stream.
        /// </summary>
        public Stream BaseStream {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the position from the start of this stream.
        /// </summary>
        public long Position {
            get;
            set;
        }

        /// <summary>
        /// Gets the length of this stream.
        /// </summary>
        public virtual long Length => BaseStream.Length;

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStream" />
        /// has been dispsosed.
        /// </summary>
        public bool Disposed {
            get;
            private set;
        }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="length">The new length of the stream.</param>
        public virtual void SetLength(long length)
        {
            BaseStream.SetLength(length);
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
            BaseStream.Position = Position;
            Position += count;
            return BaseStream.Read(buffer, index, count);
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public virtual byte ReadByte()
        {
            BaseStream.Position = Position;
            Position++;
            return (byte)BaseStream.ReadByte();
        }

        /// <summary>
        /// Writes the a portion of the buffer to the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="count">Bytes to write.</param>
        public virtual void Write(byte[] buffer, int index, int count)
        {
            BaseStream.Position = Position;
            BaseStream.Write(buffer, index, count);
            Position += count;
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">Byte value.</param>
        public virtual void WriteByte(byte data)
        {
            BaseStream.Position = Position;
            BaseStream.WriteByte(data);
            Position++;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="StreamWrapper"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="StreamWrapper"/>
        /// object.
        /// </summary>
        /// <param name="freeManaged">If set to
        /// <see langword="true" /> free managed resources also.</param>
        protected virtual void Dispose(bool freeManaged)
        {
            if (Disposed) {
                return;
            }

            Disposed = true;
            if (freeManaged) {
                BaseStream?.Dispose();
            }
        }
    }
}
