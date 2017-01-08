//
// DataWriter.cs
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
    using System.Globalization;
    using System.Text;

    public class DataWriter
    {
        public DataWriter(DataStream stream)
            : this(stream, EndiannessMode.LittleEndian, Encoding.UTF8)
        {
        }

        public DataWriter(DataStream stream, EndiannessMode endiannes, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            Stream    = stream;
            Endiannes = endiannes;
            Encoding  = encoding;
        }

        public DataStream Stream {
            get;
            private set;
        }

        public EndiannessMode Endiannes {
            get;
            private set;
        }

        public Encoding Encoding {
            get;
            private set;
        }

        public void Write(byte val)
        {
            Stream.WriteByte(val);
        }

        [CLSCompliant(false)]
        public void Write(sbyte val)
        {
            Stream.WriteByte((byte)val);
        }

        public void Write(short val)
        {
            Write((ushort)val);
        }

        [CLSCompliant(false)]
        public void Write(ushort val)
        {
            if (Endiannes == EndiannessMode.LittleEndian) {
                Write((byte)((val >> 0) & 0xFF));
                Write((byte)((val >> 8) & 0xFF));
            } else if (Endiannes == EndiannessMode.BigEndian) {
                Write((byte)((val >> 8) & 0xFF));
                Write((byte)((val >> 0) & 0xFF));
            }
        }

        public void Write(int val)
        {
            Write((uint)val);
        }

        [CLSCompliant(false)]
        public void Write(uint val)
        {
            if (Endiannes == EndiannessMode.LittleEndian) {
                Write((ushort)((val >> 00) & 0xFFFF));
                Write((ushort)((val >> 16) & 0xFFFF));
            } else if (Endiannes == EndiannessMode.BigEndian) {
                Write((ushort)((val >> 16) & 0xFFFF));
                Write((ushort)((val >> 00) & 0xFFFF));
            }
        }

        public void Write(long val)
        {
            Write((ulong)val);
        }

        [CLSCompliant(false)]
        public void Write(ulong val)
        {
            if (Endiannes == EndiannessMode.LittleEndian) {
                Write((uint)((val >> 00) & 0xFFFFFFFF));
                Write((uint)((val >> 32) & 0xFFFFFFFF));
            } else if (Endiannes == EndiannessMode.BigEndian) {
                Write((uint)((val >> 32) & 0xFFFFFFFF));
                Write((uint)((val >> 00) & 0xFFFFFFFF));
            }
        }

        public void Write(byte[] vals)
        {
            if (vals == null)
                throw new ArgumentNullException(nameof(vals));

            Stream.Write(vals, 0, vals.Length);
        }

        public void Write(char ch)
        {
            Write(Encoding.GetBytes(new char[] { ch }));
        }

        public void Write(char[] chs)
        {
            Write(Encoding.GetBytes(chs));
        }

        public void Write(string s, int byteCount, bool nullTerminator = true)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            int maxBytes = nullTerminator ? byteCount - 1 : byteCount;
            byte[] buffer = Encoding.GetBytes(s);

            if (buffer.Length > maxBytes) {
                System.IO.File.AppendAllText(
                    "fallo bytes.txt",
                    byteCount.ToString() + ": " + s + "\r\n\r\n");

                if (nullTerminator)
                    buffer[byteCount - 1] = 0x00;    // Null terminator
            }

            Array.Resize(ref buffer, byteCount);
            Write(buffer);
        }

        public void Write(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));

            Write(Encoding.GetBytes(s));
        }

        public void Write(string s, Type sizeType)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (sizeType == null)
                throw new ArgumentNullException(nameof(sizeType));

            byte[] data = Encoding.GetBytes(s);
            Write(data.Length, sizeType);
            Write(data);
        }

        public void Write(object o, Type type)
        {
            if (o == null)
                throw new ArgumentNullException(nameof(o));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            o = Convert.ChangeType(o, type, CultureInfo.InvariantCulture);

            if (type == typeof(long))
                Write((long)o);
            else if (type == typeof(ulong))
                Write((ulong)o);
            else if (type == typeof(int))
                Write((int)o);
            else if (type == typeof(uint))
                Write((uint)o);
            else if (type == typeof(short))
                Write((short)o);
            else if (type == typeof(ushort))
                Write((ushort)o);
            else if (type == typeof(byte))
                Write((byte)o);
            else if (type == typeof(sbyte))
                Write((sbyte)o);
            else if (type == typeof(char))
                Write((char)o);
            else if (type == typeof(string))
                Write((string)o);
        }

        public void WriteTimes(byte val, long times)
        {
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
                Stream.Write(buffer, 0, bytesToWrite);
            } while (written != times);
        }

        public void WriteUntilLength(byte val, long length)
        {
            long times = length - Stream.Length;
            Stream.Seek(0, SeekMode.End);
            WriteTimes(val, times);
        }

        public void WritePadding(byte val, int padding)
        {
            // TODO: Make relative padding
            int times = (int)(padding - (Stream.AbsolutePosition % padding));
            if (times != padding)    // Else it's already padded
                WriteTimes(val, times);
        }

        public void Flush()
        {
            Stream.Flush();
        }
    }
}
