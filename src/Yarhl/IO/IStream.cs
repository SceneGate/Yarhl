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
namespace Yarhl.IO
{
    using System;

    /// <summary>
    /// Generic data stream interface.
    /// </summary>
    public interface IStream : IDisposable
    {
        /// <summary>
        /// Gets the lock object of this stream (and all its substreams).
        /// </summary>
        object LockObj { get; }

        /// <summary>
        /// Gets or sets the position from the start of this stream.
        /// </summary>
        long Position { get; set; }

        /// <summary>
        /// Gets the length of this stream.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IStream" />
        /// has been disposed.
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="length">The new length of the stream.</param>
        /// <remarks><para>
        /// Some streams may not implement or support changing the length.
        /// </para></remarks>
        void SetLength(long length);

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data
        /// to be written to the underlying device.
        /// </summary>
        void Flush();

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        int ReadByte();

        /// <summary>
        /// Reads from the stream to the buffer.
        /// </summary>
        /// <returns>The number of bytes read.</returns>
        /// <param name="buffer">Buffer to copy data.</param>
        /// <param name="index">Index to start copying in buffer.</param>
        /// <param name="count">Number of bytes to read.</param>
        int Read(byte[] buffer, int index, int count);

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">Byte value.</param>
        void WriteByte(byte data);

        /// <summary>
        /// Writes the a portion of the buffer to the stream.
        /// </summary>
        /// <param name="buffer">Buffer to write.</param>
        /// <param name="index">Index in the buffer.</param>
        /// <param name="count">Bytes to write.</param>
        void Write(byte[] buffer, int index, int count);
    }
}
