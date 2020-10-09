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
    using System.IO;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class DataReaderTests
    {
        DataStream stream;
        DataReader reader;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

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
            Assert.AreEqual("utf-8", reader.DefaultEncoding.BodyName);
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
        public void ReadByteThrowExceptionWhenEof()
        {
            Assert.Throws<EndOfStreamException>(() => reader.ReadByte());
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
        public void ReadInt24LE()
        {
            byte[] buffer = { 0xAF, 0xFE, 0xCA };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.AreEqual(0xCAFEAF, reader.ReadInt24());
        }

        [Test]
        public void ReadInt24BE()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xAF };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.AreEqual(0xCAFEAF, reader.ReadInt24());
        }

        [Test]
        public void ReadInt24InvalidEndianness()
        {
            byte[] buffer = { 0xCA, 0xFE, 0xAF };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.AreEqual(-1, reader.ReadInt24());
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
        public void ReadSingleLE()
        {
            byte[] buffer = { 0xC3, 0xF5, 0x48, 0x40 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.That(reader.ReadSingle(), Is.EqualTo(3.14f));
        }

        [Test]
        public void ReadSingleBE()
        {
            byte[] buffer = { 0xC0, 0x48, 0xF5, 0xC3 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.That(reader.ReadSingle(), Is.EqualTo(-3.14f));
        }

        [Test]
        public void ReadSinglePositiveZero()
        {
            byte[] buffer = { 0x00, 0x00, 0x00, 0x00 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.LittleEndian;
            Assert.That(reader.ReadSingle(), Is.EqualTo(+0.0f));
        }

        [Test]
        public void ReadSingleNegativeZero()
        {
            byte[] buffer = { 0x00, 0x00, 0x00, 0x80 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.LittleEndian;
            Assert.That(reader.ReadSingle(), Is.EqualTo(-0.0f));
        }

        [Test]
        public void ReadSingleInvalidEndianness()
        {
            byte[] buffer = { 0x40, 0x48, 0xF5, 0xC3 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.That(reader.ReadSingle(), Is.EqualTo(float.NaN));
        }

        [Test]
        public void ReadDoubleLE()
        {
            byte[] buffer = { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0xC0 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            Assert.That(reader.ReadDouble, Is.EqualTo(-3.14d));
        }

        [Test]
        public void ReadDoubleBE()
        {
            byte[] buffer = { 0x40, 0x09, 0x1E, 0xB8, 0x51, 0xEB, 0x85, 0x1F };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = EndiannessMode.BigEndian;
            Assert.That(reader.ReadDouble, Is.EqualTo(3.14d));
        }

        [Test]
        public void ReadDoubleInvalidEndianness()
        {
            byte[] buffer = { 0x40, 0x09, 0x1E, 0xB8, 0x51, 0xEB, 0x85, 0x1F };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;
            reader.Endianness = (EndiannessMode)0x100;
            Assert.That(reader.ReadDouble, Is.EqualTo(double.NaN));
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
        public void ReadByteArrayThrowExceptionWhenEof()
        {
            stream.WriteByte(0xFF);
            stream.Position = 0;
            Assert.Throws<EndOfStreamException>(() => reader.ReadBytes(2));
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
        public void ReadCharArrayThrowsExceptionForInvalidSymbols()
        {
            byte[] buffer = { 0xE3, 0x81, 0x42, 0x42, 0x42 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.Throws<DecoderFallbackException>(() => reader.ReadChars(1));
        }

        [Test]
        public void ReadCharArrayUtf8ThrowsExWhenExpectedLengthIsBiggerThanStream()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x81 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;

            // In the case of UTF-8 is a DecoderFallback
            Assert.Throws<DecoderFallbackException>(() => reader.ReadChars(2));
        }

        [Test]
        public void ReadCharArrayUtf16ThrowsExWhenExpectedLengthIsBiggerThanStream()
        {
            byte[] buffer = { 0x01, 0xD8, 0x37 };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;

            // In the case of UTF-16 it's an out of range
            Assert.Throws<ArgumentOutOfRangeException>(
                () => reader.ReadChars(2, Encoding.Unicode));

            // or it may return the special unknown char U+FFFD
            buffer = new byte[] { 0x01, 0xD8, 0x37, 0xDC };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;

            char[] text = null;
            Assert.That(
                () => text = reader.ReadChars(1, Encoding.Unicode),
                Throws.Nothing);
            Assert.That(text, Is.EquivalentTo(new[] { '\uFFFD' }));
        }

        [Test]
        public void ReadCharArrayWithGarbageTrail()
        {
            byte[] buffer = { 0x30, 0x31, 0x00, 0x6D, 0x61, 0x74, 0x31, 0xD4 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            char[] text = reader.ReadChars(3);
            Assert.That(text, Is.EquivalentTo(new[] { '0', '1', '\0' }));
            Assert.That(stream.Position, Is.EqualTo(0x03));
        }

        [Test]
        public void ReadCharArrayWithHalfEncodedTail()
        {
            byte[] buffer = {
                0x30, 0x00, 0x31, 0x00, 0x61, 0x00, 0xe6, 0xbc, 0x00, 0x00,
                0xa2, 0xe5, // half encoded, missing second utf-16 code unit
            };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            char[] text = reader.ReadChars(5, Encoding.Unicode);

            Assert.That(text, Is.EquivalentTo(new[] { '0', '1', 'a', '볦', '\0' }));
            Assert.That(stream.Position, Is.EqualTo(10));
        }

        [Test]
        public void ReadCharArrayWithMultipleCodeUnitChars()
        {
            byte[] buffer = {
                0x01, 0xd8, 0x37, 0xdc, 0x00, 0x00,
                0xa2, 0xe5, // half encoded, missing second utf-16 code unit
            };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            char[] text = reader.ReadChars(3, Encoding.Unicode);

            Assert.That(text, Is.EquivalentTo(new[] { '\uD801', '\uDC37', '\0' }));
            Assert.That(stream.Position, Is.EqualTo(6));
        }

        [Test]
        public void ReadString()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            string actual = reader.ReadString();
            Assert.AreEqual("あア", actual);
            Assert.IsFalse(actual.Contains("\0"));
        }

        [Test]
        public void ReadStringWithEncoding()
        {
            byte[] buffer = { 0x82, 0xA0, 0x83, 0x41, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual("あア", reader.ReadString(Encoding.GetEncoding(932)));
        }

        [Test]
        public void ReadStringThrowsExceptionIfNullCharNotPresent()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.Throws<EndOfStreamException>(() => reader.ReadString());
        }

        [Test]
        public void ReadToToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            string text = reader.ReadStringToToken("95");

            Assert.AreEqual("1", text);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadToPartialToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            Assert.That(
                () => reader.ReadStringToToken("5."),
                Throws.InstanceOf<EndOfStreamException>());
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadToTokenWhenEOFThrowsException()
        {
            Assert.That(
                () => reader.ReadStringToToken("3"),
                Throws.InstanceOf<EndOfStreamException>());
        }

        [Test]
        public void ReadToTokenNullOrEmptyToken()
        {
            Assert.That(() => reader.ReadStringToToken(null), Throws.ArgumentNullException);
            Assert.That(() => reader.ReadStringToToken(string.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void ReadToTokenWithGarbageTrail()
        {
            byte[] buffer = { 0x30, 0x31, 0x00, 0x6D, 0x61, 0x74, 0x31, 0xD4 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            string text = reader.ReadStringToToken("\0");
            Assert.That(text, Is.EqualTo("01"));
            Assert.That(stream.Position, Is.EqualTo(0x03));
        }

        [Test]
        public void ReadToTokenWithHalfEncodedTail()
        {
            byte[] buffer = {
                0x30, 0x00, 0x31, 0x00, 0x61, 0x00, 0xe6, 0xbc, 0x00, 0x00,
                0xa2, 0xe5, // half encoded, missing second utf-16 code unit
            };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            string text = reader.ReadStringToToken("\0", Encoding.Unicode);

            Assert.That(text, Is.EqualTo("01a볦"));
            Assert.That(stream.Position, Is.EqualTo(10));
        }

        [Test]
        public void ReadToTokenWithMultipleCodeUnitChars()
        {
            byte[] buffer = {
                0x01, 0xd8, 0x37, 0xdc, 0x00, 0x00,
                0xa2, 0xe5, // half encoded, missing second utf-16 code unit
            };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            string text = reader.ReadStringToToken("\0", Encoding.Unicode);

            Assert.That(text, Is.EqualTo("\uD801\uDC37"));
            Assert.That(stream.Position, Is.EqualTo(6));
        }

        [Test]
        public void ReadToTokenWithCodeUnitFalsePositive()
        {
            // It may happen in some-encodings with variable length size
            // that single-bytes code unit match a byte of other code units.
            // It doesn't happen in UTF-16 since it was well-designed :D
            byte[] buffer = {
                0x82, 0x50, 0x41, 0x40, 0x00,
            };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            string text = reader.ReadStringToToken("@", Encoding.GetEncoding("shift-jis"));

            Assert.That(text, Is.EqualTo("１A"));
            Assert.That(stream.Position, Is.EqualTo(4));
        }

        [Test]
        public void ReadToTokenMultipleBuffers()
        {
            for (int i = 0; i < 150; i++)
                stream.WriteByte(0x30);

            stream.WriteByte(0x39);
            stream.WriteByte(0x38);
            stream.Position = 0;

            string text = reader.ReadStringToToken("9");

            Assert.That(text, Is.EqualTo(new string('0', 150)));
            Assert.That(stream.Position, Is.EqualTo(151));
        }

        [Test]
        public void ReadToTokenHalfEncodedBetweenBuffers()
        {
            // first buffer
            for (int i = 0; i < 127; i++)
                stream.WriteByte(0x30);
            stream.WriteByte(0xe6);

            // second buffer
            stream.WriteByte(0xbc);
            stream.WriteByte(0xa2);
            stream.WriteByte(0x08);
            stream.WriteByte(0x30);
            stream.Position = 0;

            string text = reader.ReadStringToToken("\x08");

            Assert.That(text, Is.EqualTo(new string('0', 127) + '漢'));
            Assert.That(stream.Position, Is.EqualTo(131));
        }

        [Test]
        public void ReadToTokenHalfEncodedTokenBetweenBuffers()
        {
            // first buffer
            for (int i = 0; i < 127; i++)
                stream.WriteByte(0x30);
            stream.WriteByte(0xe6);

            // second buffer
            stream.WriteByte(0xbc);
            stream.WriteByte(0xa2);
            stream.WriteByte(0x30);
            stream.Position = 0;

            string text = reader.ReadStringToToken("漢");

            Assert.That(text, Is.EqualTo(new string('0', 127)));
            Assert.That(stream.Position, Is.EqualTo(130));
        }

        [Test]
        public void ReadToTokenThrowsExceptionForInvalidSymbols()
        {
            byte[] buffer = { 0xE3, 0x81, 0x42, 0x42, 0x42, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.Throws<DecoderFallbackException>(() => reader.ReadStringToToken("\0"));
        }

        [Test]
        public void ReadFixedString()
        {
            byte[] buffer = { 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual("あ", reader.ReadString(3));
        }

        [Test]
        public void ReadFixedStringWithEncoding()
        {
            byte[] buffer = { 0x82, 0xA0, 0x83, 0x41 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual("あ", reader.ReadString(2, Encoding.GetEncoding(932)));
        }

        [Test]
        public void ReadStringWithType()
        {
            byte[] buffer = { 0x03, 0x00, 0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual("あ", reader.ReadString(typeof(short)));
        }

        [Test]
        public void ReadStringWithTypeAndEncoding()
        {
            byte[] buffer = { 0x02, 0x82, 0xA0, 0x83, 0x41 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            Assert.AreEqual("あ", reader.ReadString(typeof(byte), Encoding.GetEncoding(932)));
        }

        [Test]
        public void ReadByType()
        {
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
            stream.Write(expected, 0, expected.Length);

            stream.Position = 0;
            dynamic longValue = reader.ReadByType(typeof(long));
            Assert.IsInstanceOf<long>(longValue);
            Assert.AreEqual(3, longValue);

            dynamic ulongValue = reader.ReadByType(typeof(ulong));
            Assert.IsInstanceOf<ulong>(ulongValue);
            Assert.AreEqual(2, ulongValue);

            dynamic intValue = reader.ReadByType(typeof(int));
            Assert.IsInstanceOf<int>(intValue);
            Assert.AreEqual(1, intValue);

            dynamic uintValue = reader.ReadByType(typeof(uint));
            Assert.IsInstanceOf<uint>(uintValue);
            Assert.AreEqual(4, uintValue);

            dynamic shortValue = reader.ReadByType(typeof(short));
            Assert.IsInstanceOf<short>(shortValue);
            Assert.AreEqual(5, shortValue);

            dynamic ushortValue = reader.ReadByType(typeof(ushort));
            Assert.IsInstanceOf<ushort>(ushortValue);
            Assert.AreEqual(6, ushortValue);

            dynamic byteValue = reader.ReadByType(typeof(byte));
            Assert.IsInstanceOf<byte>(byteValue);
            Assert.AreEqual(7, byteValue);

            dynamic sbyteValue = reader.ReadByType(typeof(sbyte));
            Assert.IsInstanceOf<sbyte>(sbyteValue);
            Assert.AreEqual(8, sbyteValue);

            dynamic stringValue = reader.ReadByType(typeof(string));
            Assert.IsInstanceOf<string>(stringValue);
            Assert.AreEqual("9", stringValue);

            dynamic charValue = reader.ReadByType(typeof(char));
            Assert.IsInstanceOf<char>(charValue);
            Assert.AreEqual('a', charValue);

            dynamic floatValue = reader.ReadByType(typeof(float));
            Assert.IsInstanceOf<float>(floatValue);
            Assert.AreEqual(11, floatValue);

            dynamic doubleValue = reader.ReadByType(typeof(double));
            Assert.IsInstanceOf<double>(doubleValue);
            Assert.AreEqual(12, doubleValue);
        }

        [Test]
        public void ReadByTypeThrowExceptionForUnsupportedFormat()
        {
            byte[] expected = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            stream.Write(expected, 0, expected.Length);

            stream.Position = 0;
            Assert.Throws<FormatException>(() => reader.ReadByType(typeof(DateTime)));
        }

        [Test]
        public void ReadByGeneric()
        {
            byte[] expected = { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            stream.Write(expected, 0, expected.Length);

            stream.Position = 0;
            dynamic longValue = reader.Read<long>();
            Assert.IsInstanceOf<long>(longValue);
            Assert.AreEqual(3, longValue);
        }

        [Test]
        public void SkipPadding()
        {
            byte[] buffer = { 0xCA, 0xFE, 0x00, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            reader.ReadUInt16();
            reader.SkipPadding(4);

            Assert.AreEqual(buffer.Length, stream.Position);
        }

        [Test]
        public void SkipPaddingWhenNoNeed()
        {
            byte[] buffer = { 0xCA, 0xFE, 0x00, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            reader.ReadUInt32();
            reader.SkipPadding(4);

            Assert.AreEqual(buffer.Length, stream.Position);
        }

        [Test]
        public void SkipPaddingInvalidArguments()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.SkipPadding(-2));
        }

        [Test]
        public void SkipPaddingLessEqualOneDoesNothing()
        {
            byte[] buffer = { 0xCA, 0xFE, 0x00, 0x00 };
            stream.Write(buffer, 0, buffer.Length);

            stream.Position = 0;
            reader.ReadUInt32();

            reader.SkipPadding(0);
            Assert.AreEqual(buffer.Length, stream.Position);

            reader.SkipPadding(1);
            Assert.AreEqual(buffer.Length, stream.Position);
        }
    }
}
