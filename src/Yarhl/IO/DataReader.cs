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
namespace Yarhl.IO
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Binary DataReader for DataStreams.
    /// </summary>
    public class DataReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Yarhl.IO.DataReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <remarks>
        /// <para>By default the endianness is LittleEndian and
        /// the encoding is UTF-8.</para>
        /// </remarks>
        public DataReader(IStream stream)
        {
            Stream = stream;
            Endianness = EndiannessMode.LittleEndian;
            DefaultEncoding = new UTF8Encoding(false, true);
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        public IStream Stream {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the endianness.
        /// </summary>
        public EndiannessMode Endianness {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default encoding.
        /// </summary>
        public Encoding DefaultEncoding {
            get;
            set;
        }

        /// <summary>
        /// Reads a 8-bit byte number.
        /// </summary>
        /// <returns>The next byte.</returns>
        public byte ReadByte()
        {
            return Stream.ReadByte();
        }

        /// <summary>
        /// Reads a signed 8-bit byte number.
        /// </summary>
        /// <returns>The next signed byte.</returns>
        [CLSCompliant(false)]
        public sbyte ReadSByte()
        {
            return (sbyte)ReadByte();
        }

        /// <summary>
        /// Reads an unsigned 16-bit number.
        /// </summary>
        /// <returns>The next 16-bit number.</returns>
        [CLSCompliant(false)]
        public ushort ReadUInt16()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return (ushort)(ReadByte() | (ReadByte() << 8));
            if (Endianness == EndiannessMode.BigEndian)
                return (ushort)((ReadByte() << 8) | ReadByte());

            return 0xFFFF;
        }

        /// <summary>
        /// Reads a signed 16-bit number.
        /// </summary>
        /// <returns>The next signed 16-bit number.</returns>
        public short ReadInt16()
        {
            return (short)ReadUInt16();
        }

        /// <summary>
        /// Reads a 24-bit number.
        /// </summary>
        /// <returns>The next 24-bit number.</returns>
        public int ReadInt24()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return ReadByte() | (ReadByte() << 8) | (ReadByte() << 16);
            if (Endianness == EndiannessMode.BigEndian)
                return (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();

            return -1;
        }

        /// <summary>
        /// Reads an unsigned 32-bit number.
        /// </summary>
        /// <returns>The next unsigned 32-bit number.</returns>
        [CLSCompliant(false)]
        public uint ReadUInt32()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return (uint)(ReadUInt16() | (ReadUInt16() << 16));
            if (Endianness == EndiannessMode.BigEndian)
                return (uint)((ReadUInt16() << 16) | ReadUInt16());

            return 0xFFFFFFFF;
        }

        /// <summary>
        /// Reads a signed 32-bit number.
        /// </summary>
        /// <returns>The next signed 32-bit number.</returns>
        public int ReadInt32()
        {
            return (int)ReadUInt32();
        }

        /// <summary>
        /// Reads an unsigned 64-bit number.
        /// </summary>
        /// <returns>The next unsigned 64-bit number.</returns>
        [CLSCompliant(false)]
        public ulong ReadUInt64()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return ReadUInt32() | ((ulong)ReadUInt32() << 32);
            if (Endianness == EndiannessMode.BigEndian)
                return ((ulong)ReadUInt32() << 32) | ReadUInt32();

            return 0xFFFFFFFFFFFFFFFF;
        }

        /// <summary>
        /// Reads a signed 64-bit number.
        /// </summary>
        /// <returns>The next signed 64-bit number.</returns>
        public long ReadInt64()
        {
            return (long)ReadUInt64();
        }

        /// <summary>
        /// Reads a 32-bits IEEE 754 single precision floating-point number.
        /// </summary>
        /// <returns>The next float number.</returns>
        public float ReadSingle()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return BitConverter.ToSingle(ReadBytes(4), 0);
            if (Endianness == EndiannessMode.BigEndian)
                return BitConverter.ToSingle(ReadBytes(4).Reverse().ToArray(), 0);

            return float.NaN;
        }

        /// <summary>
        /// Reads a 64-bits IEEE 754 double precision floating-point number.
        /// </summary>
        /// <returns>The next double number.</returns>
        public double ReadDouble()
        {
            if (Endianness == EndiannessMode.LittleEndian)
                return BitConverter.ToDouble(ReadBytes(8), 0);
            if (Endianness == EndiannessMode.BigEndian)
                return BitConverter.ToDouble(ReadBytes(8).Reverse().ToArray(), 0);

            return double.NaN;
        }

        /// <summary>
        /// Reads bytes from the stream.
        /// </summary>
        /// <returns>The bytes read.</returns>
        /// <param name="count">Number of bytes to read.</param>
        public byte[] ReadBytes(int count)
        {
            byte[] buffer = new byte[count];
            Stream.Read(buffer, 0, count);
            return buffer;
        }

        /// <summary>
        /// Reads a char.
        /// </summary>
        /// <returns>The next char.</returns>
        /// <param name="encoding">Optional encoding to use.</param>
        public char ReadChar(Encoding encoding = null)
        {
            return ReadChars(1, encoding)[0];
        }

        /// <summary>
        /// Reads an array of chars.
        /// </summary>
        /// <returns>The chars read.</returns>
        /// <param name="count">The number of chars to read.</param>
        /// <param name="encoding">Optional encoding to use.</param>
        public char[] ReadChars(int count, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;

            long startPos = Stream.Position;

            // Reads the maximum number of bytes possible to get that number of chars
            int charLength = encoding.GetMaxByteCount(count);
            if (charLength > Stream.Length - Stream.Position)
                charLength = (int)(Stream.Length - Stream.Position);

            byte[] buffer = ReadBytes(charLength);
            char[] charArray = encoding.GetChars(buffer);

            if (charArray.Length > count)
                Array.Resize(ref charArray, count);
            else if (charArray.Length < count)
                throw new EndOfStreamException();

            // Adjust position
            int actualCharLength = encoding.GetByteCount(charArray);
            Stream.Seek(startPos + actualCharLength, SeekMode.Start);

            return charArray;
        }

        /// <summary>
        /// Reads a string that ends with the null terminator.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="encoding">Optional encoding to use.</param>
        public string ReadString(Encoding encoding = null)
        {
            StringBuilder str = new StringBuilder();
            if (encoding == null)
                encoding = DefaultEncoding;

            char ch;
            do {
                if (Stream.EndOfStream)
                    throw new EndOfStreamException();

                ch = ReadChar(encoding);
                if (ch != '\0')
                    str.Append(ch);
            } while (ch != '\0');

            return str.ToString();
        }

        /// <summary>
        /// Reads a string with a constant size.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="bytesCount">Size of the string in bytes.</param>
        /// <param name="encoding">Optional encoding to use.</param>
        public string ReadString(int bytesCount, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;

            byte[] buffer = ReadBytes(bytesCount);
            return encoding.GetString(buffer);
        }

        /// <summary>
        /// Reads the size with a size field first.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="sizeType">Type of the size field.</param>
        /// <param name="encoding">Optional encoding to use.</param>
        public string ReadString(Type sizeType, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = DefaultEncoding;

            dynamic size = ReadByType(sizeType);
            size = Convert.ChangeType(size, typeof(int), CultureInfo.InvariantCulture);
            return ReadString(size, encoding);
        }

        /// <summary>
        /// Reads a field by type.
        /// </summary>
        /// <returns>The field.</returns>
        /// <param name="type">Type of the field.</param>
        public dynamic ReadByType(Type type)
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
            if (type == typeof(string))
                return ReadString();

            throw new FormatException("Unsupported type");
        }

        /// <summary>
        /// Read a field by type.
        /// </summary>
        /// <returns>The field.</returns>
        /// <typeparam name="T">The type of the field.</typeparam>
        public dynamic Read<T>()
        {
            return ReadByType(typeof(T));
        }

        /// <summary>
        /// Skip bytes to pad the position in the stream.
        /// </summary>
        /// <param name="padding">Padding value.</param>
        public void SkipPadding(int padding)
        {
            if (padding < 0)
                throw new ArgumentOutOfRangeException(nameof(padding));

            if (padding <= 1) {
                return;
            }

            long remainingBytes = Stream.Position.Pad(padding) - Stream.Position;
            if (remainingBytes > 0) {
                Stream.Seek(remainingBytes, SeekMode.Current);
            }
        }
    }
}
