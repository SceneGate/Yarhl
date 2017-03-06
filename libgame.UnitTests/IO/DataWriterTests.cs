//
// DataWriterTests.cs
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
    using System.Text;
    using Libgame.IO;
    using NUnit.Framework;

    [TestFixture]
    public class DataWriterTests
    {
        [Test]
        public void ConstructorSetProperties()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreSame(stream, writer.Stream);
        }

        [Test]
        public void EndiannesProperty()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreEqual(EndiannessMode.LittleEndian, writer.Endianness);
            writer.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(EndiannessMode.BigEndian, writer.Endianness);
        }

        [Test]
        public void DefaultEncodingProperty()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreSame(Encoding.UTF8, writer.DefaultEncoding);
            writer.DefaultEncoding = Encoding.GetEncoding(932);
            Assert.AreSame(Encoding.GetEncoding(932), writer.DefaultEncoding);
        }

        [Test]
        public void WriteByte()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            byte expected = 0xAB;
            writer.Write(expected);

            Assert.AreEqual(1, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(expected, stream.ReadByte());
        }

        [Test]
        public void WriteSByte()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            sbyte expected = -5;
            writer.Write(expected);

            Assert.AreEqual(1, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(expected, (sbyte)stream.ReadByte());
        }

        [Test]
        public void WriteUShortLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write((ushort)0xCAFE);

            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteUShortBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write((ushort)0xCAFE);

            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
        }

        [Test]
        public void WriteShortLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write((short)-28124);

            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0x24, stream.ReadByte());
            Assert.AreEqual(0x92, stream.ReadByte());
        }

        [Test]
        public void WriteShortBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write((short)-28124);

            Assert.AreEqual(2, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0x92, stream.ReadByte());
            Assert.AreEqual(0x24, stream.ReadByte());
        }

        [Test]
        public void WriteUIntLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(0xCAFEBABE);

            Assert.AreEqual(4, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xBE, stream.ReadByte());
            Assert.AreEqual(0xBA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteUIntBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(0xCAFEBABE);

            Assert.AreEqual(4, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(0xBA, stream.ReadByte());
            Assert.AreEqual(0xBE, stream.ReadByte());
        }

        [Test]
        public void WriteIntLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(-1683902195);

            Assert.AreEqual(4, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0x0D, stream.ReadByte());
            Assert.AreEqual(0xB1, stream.ReadByte());
            Assert.AreEqual(0xA1, stream.ReadByte());
            Assert.AreEqual(0x9B, stream.ReadByte());
        }

        [Test]
        public void WriteIntBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(-1683902195);

            Assert.AreEqual(4, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0x9B, stream.ReadByte());
            Assert.AreEqual(0xA1, stream.ReadByte());
            Assert.AreEqual(0xB1, stream.ReadByte());
            Assert.AreEqual(0x0D, stream.ReadByte());
        }

        [Test]
        public void WriteULongLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(0xCAFEBABE1234ACDC);

            Assert.AreEqual(8, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xDC, stream.ReadByte());
            Assert.AreEqual(0xAC, stream.ReadByte());
            Assert.AreEqual(0x34, stream.ReadByte());
            Assert.AreEqual(0x12, stream.ReadByte());
            Assert.AreEqual(0xBE, stream.ReadByte());
            Assert.AreEqual(0xBA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(0xCA, stream.ReadByte());
        }

        [Test]
        public void WriteULongBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(0xCAFEBABE1234ACDC);

            Assert.AreEqual(8, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCA, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual(0xBA, stream.ReadByte());
            Assert.AreEqual(0xBE, stream.ReadByte());
            Assert.AreEqual(0x12, stream.ReadByte());
            Assert.AreEqual(0x34, stream.ReadByte());
            Assert.AreEqual(0xAC, stream.ReadByte());
            Assert.AreEqual(0xDC, stream.ReadByte());
        }

        [Test]
        public void WriteLongLittle()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(-592582943872953006);

            Assert.AreEqual(8, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0x52, stream.ReadByte());
            Assert.AreEqual(0x55, stream.ReadByte());
            Assert.AreEqual(0x7F, stream.ReadByte());
            Assert.AreEqual(0xC2, stream.ReadByte());
            Assert.AreEqual(0xF3, stream.ReadByte());
            Assert.AreEqual(0xB8, stream.ReadByte());
            Assert.AreEqual(0xC6, stream.ReadByte());
            Assert.AreEqual(0xF7, stream.ReadByte());
        }

        [Test]
        public void WriteLongBig()
        {
            DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(-592582943872953006);

            Assert.AreEqual(8, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(0xF7, stream.ReadByte());
            Assert.AreEqual(0xC6, stream.ReadByte());
            Assert.AreEqual(0xB8, stream.ReadByte());
            Assert.AreEqual(0xF3, stream.ReadByte());
            Assert.AreEqual(0xC2, stream.ReadByte());
            Assert.AreEqual(0x7F, stream.ReadByte());
            Assert.AreEqual(0x55, stream.ReadByte());
            Assert.AreEqual(0x52, stream.ReadByte());
        }
    }
}
