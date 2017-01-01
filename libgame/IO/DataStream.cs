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

        public DataStream(Stream stream, long offset, long length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!Instances.ContainsKey(stream))
                Instances.Add(stream, 1);
            else
                Instances[stream] += 1;

            BaseStream = stream;
            Length = (length != -1) ? length : stream.Length;
            Offset = offset;
            Position = Offset;
        }

        public DataStream(string filePath, FileMode mode, FileAccess access)
            : this(new FileStream(filePath, mode, access), 0, -1)
        {
        }

        public DataStream(string filePath, FileMode mode, FileAccess access, FileShare share)
            : this(new FileStream(filePath, mode, access, share), 0, -1)
        {
        }

        public DataStream(DataStream stream, long offset, long length)
            : this(stream?.BaseStream, offset + stream.Offset, length)
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

        public long RelativePosition {
            get { return Position - Offset; }
        }

        public long Length {
            get;
            private set;
        }

        public Stream BaseStream {
            get;
            private set;
        }

        public bool EOF {
            get {
                return Position >= Offset + Length;
            }
        }

        public static bool Compare(DataStream ds1, DataStream ds2)
        {
            if (ds1 == null)
                throw new ArgumentNullException(nameof(ds1));
            if (ds2 == null)
                throw new ArgumentNullException(nameof(ds2));

            if (ds1.Length != ds2.Length)
                return false;

            ds1.Seek(0, SeekMode.Origin);
            ds2.Seek(0, SeekMode.Origin);

            while (!ds1.EOF) {
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
            case SeekMode.Origin:
                Position = Offset + shift;
                break;
            case SeekMode.End:
                Position = Offset + Length - shift;
                break;
            case SeekMode.Absolute:
                Position = shift;
                break;
            }

            if (Position < Offset)
                Position = Offset;
            if (Position > Offset + Length)
                Position = Offset + Length;

            BaseStream.Position = Position;
        }

        public void SetLength(long length)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (length > BaseStream.Length)
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length is bigger than BaseStream");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length can not be negative");

            Length = length;

            if (RelativePosition > Length)
                Seek(0, SeekMode.End);
        }

        public byte ReadByte()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (Position >= Offset + Length)
                throw new EndOfStreamException();

            BaseStream.Position = Position++;
            return (byte)BaseStream.ReadByte();
        }

        public int Read(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            if (Position > Offset + Length + count)
                throw new EndOfStreamException();

            BaseStream.Position = Position;
            int read = BaseStream.Read(buffer, index, count);
            Position += count;

            return read;
        }

        public void WriteByte(byte val)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (Position > Offset + Length)
                throw new EndOfStreamException();

            if (Position == Offset + Length)
                Length++;

            BaseStream.Position = Position++;
            BaseStream.WriteByte(val);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // If we're trying to write out the stream
            if (Position > Offset + Length)
                throw new EndOfStreamException();

            // If it's in the end the file, increment it
            if (Position == Offset + Length)
                Length += count;

            BaseStream.Position = Position;
            BaseStream.Write(buffer, index, count);
            Position += count;
        }

        public void WriteTimes(byte val, long times)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            const int BufferSize = 5 * 1024;
            byte[] buffer = new byte[BufferSize];
            for (int i = 0; i < BufferSize; i++)
                buffer[i] = val;

            int written = 0;
            int bytesToWrite = 0;
            do {
                if (written + BufferSize > times)
                    bytesToWrite = (int)(times - written);
                else
                    bytesToWrite = BufferSize;

                written += bytesToWrite;
                Write(buffer, 0, bytesToWrite);
            } while (written != times);
        }

        public void WriteUntilLength(byte val, long length)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            long times = length - Length;
            Seek(0, SeekMode.End);
            WriteTimes(val, times);
        }

        public void WritePadding(byte val, int padding)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            int times = (int)(padding - (Position % padding));
            if (times != padding)    // Else it's already padded
                WriteTimes(val, times);
        }

        public void WriteTo(string fileOut)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            using (DataStream stream = new DataStream(fileOut, FileMode.Create, FileAccess.Write))
                WriteTo(stream);
        }

        public void WriteTo(DataStream stream)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            WriteTo(stream, Length);
        }

        public void WriteTo(DataStream stream, long count)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataStream));

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            long currPos = Position;
            Seek(0, SeekMode.Origin);
            BaseStream.Position = Position;

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

            Seek(currPos, SeekMode.Absolute);
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
