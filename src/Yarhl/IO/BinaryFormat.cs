// BinaryFormat.cs
//
// Author:
//      Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.IO
{
    using System;

    /// <summary>
    /// Binary format.
    /// </summary>
    public class BinaryFormat : IBinary, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormat"/> class.
        /// </summary>
        /// <remarks>This format creates a substream from the provided stream.</remarks>
        /// <param name="stream">Binary stream.</param>
        /// <param name="offset">Offset from the DataStream start.</param>
        /// <param name="length">Length of the substream.</param>
        public BinaryFormat(DataStream stream, long offset, long length)
        {
            Stream = new DataStream(stream, offset, length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormat"/> class.
        /// </summary>
        /// <remarks>This format creates a substream from the provided stream.</remarks>
        /// <param name="stream">Binary stream.</param>
        public BinaryFormat(DataStream stream)
            : this(stream, 0, stream?.Length ?? -1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormat"/> class.
        /// Creates a stream in memory.
        /// </summary>
        public BinaryFormat()
        {
            Stream = new DataStream();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFormat"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public BinaryFormat(string filePath)
        {
            Stream = new DataStream(filePath, FileOpenMode.ReadWrite);
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public DataStream Stream {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="BinaryFormat"/>
        /// is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed {
            get;
            private set;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="BinaryFormat"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="BinaryFormat"/> object.
        /// </summary>
        /// <param name="disposing">
        /// If set to <c>true</c> free managed resources also.
        /// It happens from Dispose() calls.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            Disposed = true;
            if (disposing) {
                Stream.Dispose();
            }
        }
    }
}
