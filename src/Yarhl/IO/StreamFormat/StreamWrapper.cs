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

    internal class StreamWrapper : IStream
    {
        public StreamWrapper(Stream stream)
        {
            BaseStream = stream;
        }

        protected StreamWrapper()
        {
        }

        public Stream BaseStream {
            get;
            protected set;
        }

        public long Position {
            get;
            set;
        }

        public virtual long Length => BaseStream.Length;

        public bool Disposed {
            get;
            private set;
        }

        public virtual void SetLength(long length)
        {
            BaseStream.SetLength(length);
        }

        public virtual int Read(byte[] buffer, int index, int count)
        {
            BaseStream.Position = Position;
            Position += count;
            return BaseStream.Read(buffer, index, count);
        }

        public virtual byte ReadByte()
        {
            BaseStream.Position = Position;
            Position++;
            return (byte)BaseStream.ReadByte();
        }

        public virtual void Write(byte[] buffer, int index, int count)
        {
            BaseStream.Position = Position;
            BaseStream.Write(buffer, index, count);
            Position += count;
        }

        public virtual void WriteByte(byte data)
        {
            BaseStream.Position = Position;
            BaseStream.WriteByte(data);
            Position++;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

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
