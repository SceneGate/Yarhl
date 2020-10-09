// Copyright (c) 2019 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.UnitTests.IO
{
    using System;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class DataWriterTests
    {
        [Test]
        public void ConstructorSetProperties()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreSame(stream, writer.Stream);
        }

        [Test]
        public void EndiannesProperty()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreEqual(EndiannessMode.LittleEndian, writer.Endianness);
            writer.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(EndiannessMode.BigEndian, writer.Endianness);
        }

        [Test]
        public void EncodingProperty()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.AreSame(Encoding.UTF8, writer.DefaultEncoding);
            writer.DefaultEncoding = Encoding.GetEncoding(932);
            Assert.AreSame(Encoding.GetEncoding(932), writer.DefaultEncoding);
        }

        [Test]
        public void WriteByte()
        {
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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
            using DataStream stream = new DataStream();
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

        [Test]
        public void WriteSingleLittle()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(3.14f);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(4));
            Assert.That(stream.ReadByte, Is.EqualTo(0xC3));
            Assert.That(stream.ReadByte, Is.EqualTo(0xF5));
            Assert.That(stream.ReadByte, Is.EqualTo(0x48));
            Assert.That(stream.ReadByte, Is.EqualTo(0x40));
        }

        [Test]
        public void WriteSingleBig()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(-3.14f);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(4));
            Assert.That(stream.ReadByte, Is.EqualTo(0xC0));
            Assert.That(stream.ReadByte, Is.EqualTo(0x48));
            Assert.That(stream.ReadByte, Is.EqualTo(0xF5));
            Assert.That(stream.ReadByte, Is.EqualTo(0xC3));
        }

        [Test]
        public void WriteSingleNegativeZero()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(-0.0f);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(4));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x80));
        }

        [Test]
        public void WriteSinglePositiveZero()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(+0.0f);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(4));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
            Assert.That(stream.ReadByte, Is.EqualTo(0x00));
        }

        [Test]
        public void WriteDoubleLittle()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.LittleEndian;

            writer.Write(-3.14d);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(8));
            Assert.That(stream.ReadByte, Is.EqualTo(0x1F));
            Assert.That(stream.ReadByte, Is.EqualTo(0x85));
            Assert.That(stream.ReadByte, Is.EqualTo(0xEB));
            Assert.That(stream.ReadByte, Is.EqualTo(0x51));
            Assert.That(stream.ReadByte, Is.EqualTo(0xB8));
            Assert.That(stream.ReadByte, Is.EqualTo(0x1E));
            Assert.That(stream.ReadByte, Is.EqualTo(0x09));
            Assert.That(stream.ReadByte, Is.EqualTo(0xC0));
        }

        [Test]
        public void WriteDoubleBig()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            writer.Endianness = EndiannessMode.BigEndian;

            writer.Write(3.14d);

            stream.Position = 0;
            Assert.That(stream.Length, Is.EqualTo(8));
            Assert.That(stream.ReadByte, Is.EqualTo(0x40));
            Assert.That(stream.ReadByte, Is.EqualTo(0x09));
            Assert.That(stream.ReadByte, Is.EqualTo(0x1E));
            Assert.That(stream.ReadByte, Is.EqualTo(0xB8));
            Assert.That(stream.ReadByte, Is.EqualTo(0x51));
            Assert.That(stream.ReadByte, Is.EqualTo(0xEB));
            Assert.That(stream.ReadByte, Is.EqualTo(0x85));
            Assert.That(stream.ReadByte, Is.EqualTo(0x1F));
        }

        [Test]
        public void WriteByteArray()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            byte[] buffer = { 0xCA, 0xFE, 0xBE, 0xBE };
            writer.Write(buffer);

            Assert.AreEqual(4, stream.Length);
            stream.Position = 0;
            Assert.AreEqual(buffer[0], stream.ReadByte());
            Assert.AreEqual(buffer[1], stream.ReadByte());
            Assert.AreEqual(buffer[2], stream.ReadByte());
            Assert.AreEqual(buffer[3], stream.ReadByte());
        }

        [Test]
        public void WriteNullByteArrayThrowsException()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            byte[] buffer = null;
            Assert.Throws<ArgumentNullException>(() => writer.Write(buffer));
        }

        [Test]
        public void WriteCharWithDefaultEncoding()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write('あ');

            // UTF-8
            stream.Position = 0;
            Assert.AreEqual(3, stream.Length);
            Assert.AreEqual(0xE3, stream.ReadByte());
            Assert.AreEqual(0x81, stream.ReadByte());
            Assert.AreEqual(0x82, stream.ReadByte());
        }

        [Test]
        public void WriteCharWithSpecificEncoding()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write('あ', Encoding.GetEncoding(932));

            // SHIFT-JIS
            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual(0x82, stream.ReadByte());
            Assert.AreEqual(0xA0, stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayWithDefaultEncoding()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            char[] chars = { 'あ', 'ア' };
            writer.Write(chars);

            // UTF-8
            stream.Position = 0;
            Assert.AreEqual(6, stream.Length);
            Assert.AreEqual(0xE3, stream.ReadByte());
            Assert.AreEqual(0x81, stream.ReadByte());
            Assert.AreEqual(0x82, stream.ReadByte());
            Assert.AreEqual(0xE3, stream.ReadByte());
            Assert.AreEqual(0x82, stream.ReadByte());
            Assert.AreEqual(0xA2, stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayWithSpecificEncoding()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            char[] chars = { 'あ', 'ア' };
            writer.Write(chars, Encoding.GetEncoding(932));

            // SHIFT-JIS
            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual(0x82, stream.ReadByte());
            Assert.AreEqual(0xA0, stream.ReadByte());
            Assert.AreEqual(0x83, stream.ReadByte());
            Assert.AreEqual(0x41, stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayNullThrowsException()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            char[] chars = null;
            Assert.Throws<ArgumentNullException>(() => writer.Write(chars));
        }

        [Test]
        public void WriteTextAndNoNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, false);

            Assert.AreEqual(6, stream.Length);
            byte[] expected = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            byte[] actual = new byte[6];
            stream.Position = 0;
            stream.Read(actual, 0, 6);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextWithEncodingAndNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, true, Encoding.GetEncoding("utf-16"));

            byte[] expected = { 0x42, 0x30, 0xA2, 0x30, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextWithMaxSizeAndNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, true, maxSize: 4);

            byte[] expected = { 0xE3, 0x81, 0x82, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextWithInvalidArgumentsThrowsException()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = null;
            Assert.Throws<ArgumentNullException>(() => writer.Write(text));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => writer.Write(string.Empty, maxSize: -2));
        }

        [Test]
        public void WriteTextFixedSize()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, 10);

            byte[] expected = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00, 0x00, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextAvoidDuplicateNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア\0";
            writer.Write(text);

            byte[] expected = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextFixedSizeTruncating()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, 4, true, Encoding.GetEncoding("utf-16"));

            byte[] expected = { 0x42, 0x30, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextFixedSizeWithEncodingAndNoNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, 4, false, Encoding.GetEncoding("utf-16"));

            byte[] expected = { 0x42, 0x30, 0xA2, 0x30 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextFixedSizeWithInvalidArgumentsThrowsException()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = null;
            Assert.Throws<ArgumentNullException>(() => writer.Write(text, 5));
        }

        [Test]
        public void WriteTextAndSize()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, typeof(ushort));

            Assert.AreEqual(8, stream.Length);
            byte[] expected = { 0x06, 0x00, 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextAndSizeWithEncodingAndNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, typeof(byte), true, Encoding.GetEncoding("utf-16"));

            byte[] expected = { 0x06, 0x42, 0x30, 0xA2, 0x30, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextAndSizeWithAvoidDuplicateNullTerminators()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア\0";
            writer.Write(text, typeof(byte), true);

            byte[] expected = { 0x07, 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextAndSizeWithMaxSizeAndNullTerminator()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string text = "あア";
            writer.Write(text, typeof(int), true, maxSize: 3);

            byte[] expected = { 0x03, 0x00, 0x00, 0x00, 0xE3, 0x81, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);
            byte[] actual = new byte[expected.Length];
            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTextAndSizeWithInvalidArgumentsThrowsException()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            string nullText = null;
            Type nullType = null;
            Assert.Throws<ArgumentNullException>(
                () => writer.Write(nullText, typeof(short)));
            Assert.Throws<ArgumentNullException>(
                () => writer.Write(string.Empty, nullType));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => writer.Write(string.Empty, typeof(short), maxSize: -2));
        }

        [Test]
        public void WriteObjects()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.WriteOfType(typeof(long), "3");
            writer.WriteOfType(typeof(ulong), 2);
            writer.WriteOfType(typeof(int), 1);
            writer.WriteOfType(typeof(uint), 4);
            writer.WriteOfType(typeof(short), 5);
            writer.WriteOfType(typeof(ushort), 6);
            writer.WriteOfType(typeof(byte), 7);
            writer.WriteOfType(typeof(sbyte), 8);
            writer.WriteOfType(typeof(string), 9);
            writer.WriteOfType(typeof(char), 'a');
            writer.WriteOfType(typeof(float), 11);
            writer.WriteOfType(typeof(double), 12);

            byte[] expected = {
                0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x00, 0x00,
                0x05, 0x00,
                0x06, 0x00,
                0x07,
                0x08,
                0x39, 0x00,
                0x61,
                0x00, 0x00, 0x30, 0x41,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x40,
            };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteGenericObject()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.WriteOfType<long>(3);
            writer.WriteOfType<ulong>(2);
            writer.WriteOfType<int>(1);
            writer.WriteOfType<uint>(4);
            writer.WriteOfType<short>(5);
            writer.WriteOfType<ushort>(6);
            writer.WriteOfType<byte>(7);
            writer.WriteOfType<sbyte>(8);
            writer.WriteOfType<string>("9");
            writer.WriteOfType<char>('a');
            writer.WriteOfType<float>(11);
            writer.WriteOfType<double>(12);

            byte[] expected = {
                0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x01, 0x00, 0x00, 0x00,
                0x04, 0x00, 0x00, 0x00,
                0x05, 0x00,
                0x06, 0x00,
                0x07,
                0x08,
                0x39, 0x00,
                0x61,
                0x00, 0x00, 0x30, 0x41,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x40,
            };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteObjectsInvalidConversion()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            Assert.Throws<InvalidCastException>(() => writer.WriteOfType(typeof(DateTime), 1));
        }

        [Test]
        public void WriteObjectsUnsupportedType()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            Assert.Throws<FormatException>(() => writer.WriteOfType(typeof(bool), true));
        }

        [Test]
        public void WriteObjectsInvalidArguments()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            Type nullType = null;
            string nullValue = null;
            Assert.Throws<ArgumentNullException>(() => writer.WriteOfType(nullType, 1));
            Assert.Throws<ArgumentNullException>(() => writer.WriteOfType(typeof(string), nullValue));
        }

        [Test]
        public void WriteTimesLessBufferSize()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.WriteTimes(0xFF, 3);

            byte[] expected = { 0xFF, 0xFF, 0xFF };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteTimesMoreThanOneBuffer()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.WriteTimes(0xFF, (5 * 1024) + 8);

            byte[] expected = new byte[(5 * 1024) + 8];
            for (int i = 0; i < expected.Length; i++)
                expected[i] = 0xFF;

            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteNegativeTimes()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteTimes(0xFF, -2));
        }

        [Test]
        public void WriteUntilLength()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.WriteUntilLength(0xAF, 7);

            byte[] expected = { 0xAF, 0xAF, 0xAF, 0xAF, 0xAF, 0xAF, 0xAF };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteUntilLengthLessThanStream()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(0xFFFFFFFF);
            writer.WriteUntilLength(0xAF, 3);

            byte[] expected = { 0xFF, 0xFF, 0xFF, 0xFF };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteUntilLengthMoveToEnd()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(0xFFFFFFFF);
            stream.Position = 2;
            writer.WriteUntilLength(0xAF, 6);

            byte[] expected = { 0xFF, 0xFF, 0xFF, 0xFF, 0xAF, 0xAF };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WriteUntilLengthInvalidArguments()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.WriteUntilLength(0x00, -2));
        }

        [Test]
        public void WritePadding()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write((ushort)0xCAFE);
            writer.WritePadding(0xFF, 4);

            byte[] expected = { 0xFE, 0xCA, 0xFF, 0xFF };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WritePaddingWhenNoNeed()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(0xCAFEu);
            writer.WritePadding(0xFF, 4);

            byte[] expected = { 0xFE, 0xCA, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void WritePaddingInvalidArguments()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);
            Assert.Throws<ArgumentOutOfRangeException>(() => writer.WritePadding(0x00, -2));
        }

        [Test]
        public void WritePaddingLessEqualOneDoesNothing()
        {
            using DataStream stream = new DataStream();
            DataWriter writer = new DataWriter(stream);

            writer.Write(0xCAFEu);
            writer.WritePadding(0xFF, 0);

            byte[] expected = { 0xFE, 0xCA, 0x00, 0x00 };
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            byte[] actual = new byte[expected.Length];
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));

            writer.WritePadding(0xFF, 1);
            Assert.AreEqual(expected.Length, stream.Length);

            stream.Position = 0;
            stream.Read(actual, 0, expected.Length);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}
