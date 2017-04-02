//
// DataReaderTests.cs
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
namespace Libgame.UnitTests.IO
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Libgame.IO;

    [TestFixture]
    public class DataReaderTests
    {
        DataStream stream;
        DataReader reader;

        [SetUp]
        public void SetUp()
        {
            stream = new DataStream();
            reader = new DataReader(stream);
        }

        [TearDown]
        public void TearDown()
        {
            stream.Dispose();
        }

        [Test]
        public void ConstructorSetProperties()
        {
            Assert.AreSame(stream, reader.Stream);
        }

        [Test]
        public void EndiannesProperty()
        {
            Assert.AreEqual(EndiannessMode.LittleEndian, reader.Endianness);
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(EndiannessMode.BigEndian, reader.Endianness);
        }

        [Test]
        public void EncodingProperty()
        {
            Assert.AreSame(Encoding.UTF8, reader.DefaultEncoding);
            reader.DefaultEncoding = Encoding.GetEncoding(932);
            Assert.AreSame(Encoding.GetEncoding(932), reader.DefaultEncoding);
        }

        [Test]
        public void ReadByte()
        {
            stream.WriteByte(0xAF);
            stream.Position = 0;
            Assert.AreEqual(0xAF, reader.ReadByte());
        }

        [Test]
        public void ReadSByte()
        {
            stream.WriteByte(0x81);
            stream.Position = 0;
            Assert.AreEqual(-127, reader.ReadSByte());
        }

        [Test]
        public void ReadUInt16LE()
        {
            byte[] buffer = { 0xCA, 0xFE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(0xFECA, reader.ReadUInt16());
        }

        [Test]
        public void ReadUInt16BE()
        {
            byte[] buffer = { 0xCA, 0xFE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(0xCAFE, reader.ReadUInt16());
        }

        [Test]
        public void ReadUInt16InvalidEndianness()
        {
            byte[] buffer = { 0xCA, 0xFE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(0xFFFF, reader.ReadUInt16());
        }

        [Test]
        public void ReadInt16LE()
        {
            byte[] buffer = { 0x24, 0x92 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(-28124, reader.ReadInt16());
        }

        [Test]
        public void ReadInt16BE()
        {
            byte[] buffer = { 0x92, 0x24 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(-28124, reader.ReadInt16());
        }

        [Test]
        public void ReadInt16InvalidEndianness()
        {
            byte[] buffer = { 0x92, 0x24 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(-1, reader.ReadInt16());
        }

        [Test]
        public void ReadUInt32LE()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xBA, 0xBE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(0xBEBAFECA, reader.ReadUInt32());
        }

        [Test]
        public void ReadUInt32BE()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xBA, 0xBE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(0xCAFEBABE, reader.ReadUInt32());
        }

        [Test]
        public void ReadUInt32InvalidEndianness()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xBA, 0xBE };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(0xFFFFFFFF, reader.ReadUInt32());
        }

        [Test]
        public void ReadInt32LE()
        {
            byte[] buffer = { 0x0D, 0xB1, 0xA1, 0x9B };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(-1683902195, reader.ReadInt32());
        }

        [Test]
        public void ReadInt32BE()
        {
            byte[] buffer = { 0x9B, 0xA1, 0xB1, 0x0D };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(-1683902195, reader.ReadInt32());
        }

        [Test]
        public void ReadInt32InvalidEndianness()
        {
            byte[] buffer = { 0x9B, 0xA1, 0xB1, 0x0D };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(-1, reader.ReadInt32());
        }

        [Test]
        public void ReadUInt64LE()
        {
            byte[] buffer = { 0xDC, 0xAC, 0x34, 0x12, 0xBE, 0xBA, 0xFE, 0xCA };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCAFEBABE1234ACDC, reader.ReadUInt64());
        }

        [Test]
        public void ReadUInt64BE()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xBA, 0xBE, 0x12, 0x34, 0xAC, 0xDC };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(0xCAFEBABE1234ACDC, reader.ReadUInt64());
        }

        [Test]
        public void ReadUInt64InvalidEndianness()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xBA, 0xBE, 0x12, 0x34, 0xAC, 0xDC };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(0xFFFFFFFFFFFFFFFF, reader.ReadUInt64());
        }

        [Test]
        public void ReadInt64LE()
        {
            byte[] buffer = { 0x52, 0x55, 0x7F, 0xC2, 0xF3, 0xB8, 0xC6, 0xF7 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(-592582943872953006, reader.ReadInt64());
        }

        [Test]
        public void ReadInt164BE()
        {
            byte[] buffer = { 0xF7, 0xC6, 0xB8, 0xF3, 0xC2, 0x7F, 0x55, 0x52 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(-592582943872953006, reader.ReadInt64());
        }

        [Test]
        public void ReadInt164InvalidEndianness()
        {
            byte[] buffer = { 0xF7, 0xC6, 0xB8, 0xF3, 0xC2, 0x7F, 0x55, 0x52 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(-1, reader.ReadInt64());
        }

        [Test]
        public void ReadByteArray()
        {
            byte[] buffer = { 0xF7, 0xC6, 0xB8, 0xF3, 0xC2, 0x7F, 0x55, 0x52 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            byte[] expected = reader.ReadBytes(buffer.Length);
            Assert.IsTrue(buffer.SequenceEqual(expected));
        }

        [Test]
        public void ReadChar()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual('あ', reader.ReadChar());
        }

        [Test]
        public void ReadCharWithEncoding()
        {
            byte[] buffer = { 0x82, 0xA0 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual('あ', reader.ReadChar(Encoding.GetEncoding(932)));
        }

        [Test]
        public void ReadCharArray()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual(new char[] { 'あ', 'ア' }, reader.ReadChars(2));
        }

        [Test]
        public void ReadCharArrayWithEncoding()
        {
            byte[] buffer = { 0x82, 0xA0, 0x83, 0x41 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual(
                    new char[] { 'あ', 'ア' },
                    reader.ReadChars(2, Encoding.GetEncoding(932)));
        }

        [Test]
        public void ReadCharArrayAdjustPosition()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            reader.ReadChars(1);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadCharArrayIfNotEnoughBytesThrowException()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.Throws<EndOfStreamException>(() => reader.ReadChars(2));
        }

        [Test]
        public void ReadCharArrayThrowsEofWhenExpectedLengthIsBiggerThanStream()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x81 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.Throws<EndOfStreamException>(() => reader.ReadChars(2));
        }
    }
}
