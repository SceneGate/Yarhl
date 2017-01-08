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
        long length;

        public DataStream(Stream stream, long offset, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (offset < 0 || offset > stream.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            if (length < -1)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (!Instances.ContainsKey(stream))
                Instances.Add(stream, 1);
            else
                Instances[stream] += 1;

            BaseStream = stream;
            Offset = offset;
            Length = length;
        }

        public DataStream(Stream stream)
            : this(stream, 0, -1)
        {
        }

        public DataStream()
            : this(new MemoryStream())
        {
        }

        public DataStream(string filePath, FileMode mode)
            : this(new FileStream(filePath, mode, FileAccess.ReadWrite))
        {
        }

        public DataStream(DataStream stream, long offset, long length)
            : this(stream?.BaseStream, offset + (stream?.Offset ?? 0), length)
        {
        }

        ~DataStream()
        {
            Dispose(false);
        }

        public bool Disposed {
            get;
            private set;
        }

        public long Offset {
            get;
            private set;
        }

        public long Position {
            get;
            private set;
        }

        public long Length {
            get {
                return length;
            }

            set {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(DataStream));
                if (value < -1)
                    throw new ArgumentOutOfRangeException(nameof(value));

                length = (value != -1) ? value : BaseStream.Length;

                if (Position > Length)
                    Position = Length;
            }
        }

        public Stream BaseStream {
            get;
            private set;
        }

        public bool EndOfStream {
            get {
                return Position >= Length;
            }
        }

        public long AbsolutePosition {
            get { return Offset + Position; }
        }

        public static bool Compare(DataStream ds1, DataStream ds2)
        {
            if (ds1 == null)
                throw new ArgumentNullException(nameof(ds1));
            if (ds2 == null)
                throw new ArgumentNullException(nameof(ds2));

            if (ds1.Length != ds2.Length)
                return false;

            ds1.Seek(0, SeekMode.Start);
            ds2.Seek(0, SeekMode.Start);

            while (!ds1.EndOfStream) {
                if (ds1.ReadByte() != ds2.ReadByte())
                    return false;
            }

            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Flush()
        {
            BaseStream.Flush();
        }

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

            if (Position < 0)
                Position = 0;
            if (Position > Length)
                Position = Length;

            BaseStream.Position = AbsolutePosition;
        }

        public byte ReadByte()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (AbsolutePosition >= Offset + Length)
                throw new EndOfStreamException();

            BaseStream.Position = AbsolutePosition;
            Position++;
            return (byte)BaseStream.ReadByte();
        }

        public int Read(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (AbsolutePosition > Offset + Length + count)
                throw new EndOfStreamException();

            BaseStream.Position = AbsolutePosition;
            int read = BaseStream.Read(buffer, index, count);
            Position += count;

            return read;
        }

        public void WriteByte(byte val)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (AbsolutePosition > Offset + Length)
                throw new EndOfStreamException();

            if (AbsolutePosition == Offset + Length)
                Length++;

            BaseStream.Position = AbsolutePosition;
            Position++;
            BaseStream.WriteByte(val);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // If we're trying to write out the stream
            if (AbsolutePosition > Offset + Length)
                throw new EndOfStreamException();

            // If it's in the end the file, increment it
            if (AbsolutePosition == Offset + Length)
                Length += count;

            BaseStream.Position = AbsolutePosition;
            BaseStream.Write(buffer, index, count);
            Position += count;
        }

        public void WriteTo(string fileOut)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            using (var stream = new DataStream(fileOut, FileMode.Create))
                WriteTo(stream);
        }

        public void WriteTo(DataStream stream)
        {
            long currPos = Position;
            Seek(0, SeekMode.Start);

            WriteTo(stream, Length);

            Seek(currPos, SeekMode.Start);
        }

        public void WriteTo(DataStream stream, long count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BaseStream.Position = AbsolutePosition;

            const int BufferSize = 5 * 1024;
            byte[] buffer = new byte[BufferSize];

            int written = 0;
            int bytesToRead = 0;
            do {
                if (written + BufferSize > count)
                    bytesToRead = (int)(count - written);
                else
                    bytesToRead = BufferSize;

                written += Read(buffer, 0, bytesToRead);
                stream.Write(buffer, 0, bytesToRead);
            } while (written != count);
        }

        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            Disposed = true;
            Instances[BaseStream] -= 1;

            if (freeManagedResourcesAlso && Instances[BaseStream] == 0) {
                BaseStream.Dispose();
            }
        }
    }
}
