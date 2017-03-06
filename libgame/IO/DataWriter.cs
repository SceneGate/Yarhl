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
        {
            Stream = stream;
            Endianness = EndiannessMode.LittleEndian;
            DefaultEncoding = Encoding.UTF8;
        }

        public DataStream Stream {
            get;
            private set;
        }

        public EndiannessMode Endianness {
            get;
            set;
        }

        public Encoding DefaultEncoding {
            get;
            set;
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
            WriteNumber((ushort)val, 16);
        }

        [CLSCompliant(false)]
        public void Write(ushort val)
        {
            WriteNumber(val, 16);
        }

        public void Write(int val)
        {
            WriteNumber((uint)val, 32);
        }

        [CLSCompliant(false)]
        public void Write(uint val)
        {
            WriteNumber(val, 32);
        }

        public void Write(long val)
        {
            WriteNumber((ulong)val, 64);
        }

        [CLSCompliant(false)]
        public void Write(ulong val)
        {
            WriteNumber(val, 64);
        }

        public void Write(byte[] vals)
        {
            if (vals == null)
                throw new ArgumentNullException(nameof(vals));

            Stream.Write(vals, 0, vals.Length);
        }

        public void Write(char ch, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;

            Write(encoding.GetBytes(new[] { ch }));
        }

        public void Write(char[] chs, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;

            Write(encoding.GetBytes(chs));
        }

        public void Write(
            string s,
            int fixedLength,
            bool nullTerminator = true,
            Encoding encoding = null)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (encoding == null)
                encoding = DefaultEncoding;

            byte[] buffer = encoding.GetBytes(s);
            Array.Resize(ref buffer, fixedLength);

            if (nullTerminator)
                buffer[fixedLength - 1] = 0x00;

            Write(buffer);
        }

        public void Write(string s, Encoding encoding = null)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (encoding == null)
                encoding = DefaultEncoding;

            Write(encoding.GetBytes(s));
        }

        public void Write(string s, Type sizeType, Encoding encoding = null)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (sizeType == null)
                throw new ArgumentNullException(nameof(sizeType));
            if (encoding == null)
                encoding = DefaultEncoding;

            byte[] data = encoding.GetBytes(s);
            Write(data.Length, sizeType);
            Write(data);
        }

        public void Write(dynamic o, Type type)
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
            int times = (int)(padding - (Stream.AbsolutePosition % padding));
            if (times != padding)    // Else it's already padded
                WriteTimes(val, times);
        }

        void WriteNumber(ulong number, byte numBytes)
        {
            byte start;
            byte end;
            int step;

            if (Endianness == EndiannessMode.LittleEndian) {
                start = 0;
                end = numBytes;
                step = 8;
            } else {
                start = (byte)(numBytes - 8);
                end = 0xF8; // When the byte var reach < 0 it overflows to 0-8=0xF8
                step = -8;
            }

            for (byte i = start; i < end; i = (byte)(i + step)) {
                byte val = (byte)((number >> i) & 0xFF);
                Stream.WriteByte(val);
            }
        }
    }
}
