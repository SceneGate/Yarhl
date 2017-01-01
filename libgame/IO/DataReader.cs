//
// DataReader.cs
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

    public class DataReader
    {
        public DataReader(DataStream stream)
            : this(stream, EndiannessMode.LittleEndian, Encoding.UTF8)
        {
        }

        public DataReader(DataStream stream, EndiannessMode endiannes, Encoding encoding)
        {
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

        public byte ReadByte()
        {
            return Stream.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return (sbyte)Stream.ReadByte();
        }

        public ushort ReadUInt16()
        {
            if (Endiannes == EndiannessMode.LittleEndian)
                return (ushort)((ReadByte() << 0) | (ReadByte() << 8));
            if (Endiannes == EndiannessMode.BigEndian)
                return (ushort)((ReadByte() << 8) | (ReadByte() << 0));

            return 0xFFFF;
        }

        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        public uint ReadUInt32()
        {
            if (Endiannes == EndiannessMode.LittleEndian)
                return (uint)((ReadUInt16() << 00) | (ReadUInt16() << 16));
            if (Endiannes == EndiannessMode.BigEndian)
                return (uint)((ReadUInt16() << 16) | (ReadUInt16() << 00));

            return 0xFFFFFFFF;
        }

        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            if (Endiannes == EndiannessMode.LittleEndian)
                return (ReadUInt32() << 00) | (ReadUInt32() << 32);
            if (Endiannes == EndiannessMode.BigEndian)
                return (ReadUInt32() << 32) | (ReadUInt32() << 00);

            return 0xFFFFFFFFFFFFFFFF;
        }

        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        public byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            Stream.Read(buffer, 0, count);
            return buffer;
        }

        public char ReadChar()
        {
            return ReadChars(1)[0];
        }

        public char[] ReadChars(int count)
        {
            long pos1 = Stream.Position;
            int charLength = Encoding.GetMaxByteCount(count);
            byte[] buffer = ReadBytes(charLength);

            char[] charArray = Encoding.GetChars(buffer);
            Array.Resize(ref charArray, count);    // In case we get more chars than asked

            // Adjust position
            charLength = Encoding.GetByteCount(charArray);
            Stream.Seek(pos1 + charLength, SeekMode.Absolute);

            return charArray;
        }

        /// <summary>
        /// Read until 0x00 byte or EOF reached
        /// </summary>
        /// <returns>The string.</returns>
        public string ReadString()
        {
            StringBuilder str = new StringBuilder();

            int maxBytes = Encoding.GetMaxByteCount(1);
            char ch;

            do {
                long bytesLeft = Stream.Length - Stream.RelativePosition;
                int bytesToRead = (bytesLeft < maxBytes) ? (int)bytesLeft : maxBytes;

                byte[] data = ReadBytes(bytesToRead);
                string decodedString = Encoding.GetString(data);
                if (decodedString.Length == 0)
                    break;

                ch = decodedString[0];
                int bytesRead = Encoding.GetByteCount(ch.ToString());
                int bytesNotRead = bytesToRead - bytesRead;
                Stream.Seek(-bytesNotRead, SeekMode.Current);

                if (ch != '\0')
                    str.Append(ch);
            } while (ch != '\0' && !Stream.EOF);

            return str.ToString();
        }

        public string ReadString(int bytesCount)
        {
            byte[] buffer = ReadBytes(bytesCount);
            string s = Encoding.GetString(buffer);
            s = s.Replace("\0", string.Empty);
            return s;
        }

        public string ReadString(Type sizeType)
        {
            object size = Read(sizeType);
            size = Convert.ChangeType(size, typeof(int), CultureInfo.InvariantCulture);
            return ReadString((int)size);
        }

        public object Read(Type type)
        {
            if (type == typeof(long))
                return ReadInt64();
            if (type == typeof(ulong))
                return ReadUInt64();
            if (type == typeof(int))
                return ReadInt32();
            if (type == typeof(uint))
                return ReadUInt32();
            if (type == typeof(short))
                return ReadInt16();
            if (type == typeof(ushort))
                return ReadUInt16();
            if (type == typeof(byte))
                return ReadByte();
            if (type == typeof(sbyte))
                return ReadSByte();
            if (type == typeof(char))
                return ReadChar();

            return null;
        }
    }
}
