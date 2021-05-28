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
    /// Open file for reading or writing on the first operation (lazily).
    /// </summary>
    public sealed class LazyFileStream : StreamWrapper
    {
        readonly string path;
        readonly FileOpenMode mode;
        readonly long initialLength;

        bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="LazyFileStream"/> class.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="mode">Mode to open the file.</param>
        public LazyFileStream(string path, FileOpenMode mode)
        {
            this.path = path;
            this.mode = mode;

            // Before the stream is initialized, we can get the
            // length so we can report it wihthout opening the file.
            var info = new FileInfo(path);
            initialLength = info.Exists ? info.Length : 0;
        }

        /// <summary>
        /// Gets the length of this stream.
        /// </summary>
        public override long Length => BaseStream?.Length ?? initialLength;

        /// <summary>
        /// Sets the length of the stream.
        /// </summary>
        /// <param name="length">The new length of the stream.</param>
        public override void SetLength(long length)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LazyFileStream));

            if (!isInitialized) {
                Initialize();
            }

            base.SetLength(length);
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
                throw new ObjectDisposedException(nameof(LazyFileStream));

            if (!isInitialized) {
                Initialize();
            }

            return base.Read(buffer, index, count);
        }

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <returns>The next byte.</returns>
        public override int ReadByte()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LazyFileStream));

            if (!isInitialized) {
                Initialize();
            }

            return base.ReadByte();
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
                throw new ObjectDisposedException(nameof(LazyFileStream));

            if (!isInitialized) {
                Initialize();
            }

            base.Write(buffer, index, count);
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="data">Byte value.</param>
        public override void WriteByte(byte data)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(LazyFileStream));

            if (!isInitialized) {
                Initialize();
            }

            base.WriteByte(data);
        }

        void Initialize()
        {
            isInitialized = true;
            BaseStream = new FileStream(path, mode.ToFileMode(), mode.ToFileAccess());
        }
    }
}
