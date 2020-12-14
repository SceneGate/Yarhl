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
namespace Yarhl.UnitTests.Media.Text.Encodings
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.Media.Text.Encodings;

    [TestFixture]
    public class EucJpEncodingTests
    {
        EucJpEncoding encoder;

        [SetUp]
        public void SetUp()
        {
            encoder = new EucJpEncoding();
        }

        [Test]
        public void DefaultConstructorSetToErrorFallback()
        {
            Assert.IsInstanceOf<DecoderExceptionFallback>(encoder.DecoderFallback);
            Assert.IsInstanceOf<EncoderExceptionFallback>(encoder.EncoderFallback);
        }

        [Test]
        public void ConstructorSetFallbacks()
        {
            encoder = new EucJpEncoding(
                new DecoderReplacementFallback("@"),
                new EncoderReplacementFallback("@"));
            Assert.IsInstanceOf<DecoderReplacementFallback>(encoder.DecoderFallback);
            Assert.IsInstanceOf<EncoderReplacementFallback>(encoder.EncoderFallback);
        }

        [Test]
        public void ConstructorChecksArguments()
        {
            Assert.Throws<ArgumentNullException>(
                () => new EucJpEncoding(null, new EncoderExceptionFallback()));
            Assert.Throws<ArgumentNullException>(
                () => new EucJpEncoding(new DecoderExceptionFallback(), null));
        }

        [Test]
        public void TestTextAscii()
        {
            byte[] encoded = { 0x30, 0x33, 0x41, 0x62 };
            string decoded = "03Ab";
            TestEncodingText(decoded, encoded);
        }

        [Test]
        public void TestTextJis208()
        {
            byte[] encoded = { 0xA4, 0xE9, 0xD0, 0xA5 };
            string decoded = "ら丱";
            TestEncodingText(decoded, encoded);
        }

        [Test]
        public void TestTextJis212()
        {
            byte[] encoded = { 0x8F, 0xB0, 0xCE, 0x8F, 0xB0, 0xA4 };
            string decoded = "仵丌";
            TestEncodingText(decoded, encoded);
        }

        [Test]
        public void TestTextHalfWidthKana()
        {
            byte[] encoded = { 0x8E, 0xAA, 0x8E, 0xDA };
            string decoded = "ｪﾚ";
            TestEncodingText(decoded, encoded);
        }

        [Test]
        public void TestTextMix()
        {
            byte[] encoded = {
                0x30, 0x33, 0x41, 0x62,
                0xA4, 0xE9, 0xD0, 0xA5,
                0x8F, 0xB0, 0xCE, 0x8F, 0xB0, 0xA4,
                0x30, 0x33, 0x41, 0x62,
                0x8F, 0xB0, 0xCE, 0x8F, 0xB0, 0xA4,
                0xA4, 0xE9, 0xD0, 0xA5,
                0x30, 0x33, 0x41, 0x62,
            };
            string decoded = "03Abら丱仵丌03Ab仵丌ら丱03Ab";
            TestEncodingText(decoded, encoded);
        }

        [Test]
        public void TestSpecialChars()
        {
            byte[] encoded = { 0x5C, 0x7E, 0xA1, 0xDD };
            string decoded = "¥‾−";
            TestEncodeText(decoded, encoded);
        }

        [Test]
        public void DecodeTextArgumentErrors()
        {
            byte[] buffer = new byte[4];
            char[] output = new char[4];

            Assert.Throws<ArgumentNullException>(
                () => encoder.GetChars(null, 0, 1, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetChars(buffer, -1, 1, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetChars(buffer, 0, 5, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetChars(buffer, 3, -1, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetChars(buffer, 3, 2, output, 0));
            Assert.Throws<ArgumentNullException>(
                () => encoder.GetChars(buffer, 0, 1, null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetChars(buffer, 0, 1, output, 5));

            Assert.Throws<ArgumentNullException>(
                () => encoder.GetCharCount(null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetCharCount(buffer, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetCharCount(buffer, 5, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetCharCount(buffer, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetCharCount(buffer, 0, 5));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetMaxCharCount(-1));
        }

        [Test]
        public void EncodeTextArgumentErrors()
        {
            byte[] buffer = new byte[4];
            char[] input = new char[4];

            Assert.Throws<ArgumentNullException>(
                () => encoder.GetBytes((char[])null, 0, 1, buffer, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetBytes(input, -1, 1, buffer, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetBytes(input, 0, 5, buffer, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetBytes(input, 3, 2, buffer, 0));
            Assert.Throws<ArgumentNullException>(
                () => encoder.GetBytes(input, 0, 1, null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetBytes(input, 0, 1, buffer, -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetBytes(input, 0, 1, buffer, 5));

            Assert.Throws<ArgumentNullException>(
                () => encoder.GetByteCount((char[])null, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetByteCount(input, -1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetByteCount(input, 5, 1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetByteCount(input, 0, -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetByteCount(input, 0, 5));

            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoder.GetMaxByteCount(-1));
        }

        [Test]
        public void DefaultDecoderFallbackThrowsException()
        {
            byte[] encoded = { 0x80, 0xA1, 0x83 };
            Assert.Throws<DecoderFallbackException>(
                () => encoder.GetString(encoded));
        }

        [Test]
        public void CustomDecoderFallback()
        {
            byte[] encoded = { 0x80, 0xA1, 0x83 };
            encoder = new EucJpEncoding(
                new DecoderReplacementFallback("?!"),
                new EncoderExceptionFallback());
            Assert.AreEqual("?!?!?!", encoder.GetString(encoded));
        }

        [Test]
        public void DefaultEncoderFallbackThrowsException()
        {
            string decoded = "€\ud801\udc37";
            Assert.Throws<EncoderFallbackException>(
                () => encoder.GetBytes(decoded));
        }

        [Test]
        public void CustomEncoderFallback()
        {
            string decoded = "€\ud801\udc37";
            encoder = new EucJpEncoding(
                new DecoderExceptionFallback(),
                new EncoderReplacementFallback("?!"));

            byte[] expected = { 0x3F, 0x21, 0x3F, 0x21, 0x3F, 0x21 };
            Assert.AreEqual(expected, encoder.GetBytes(decoded));
        }

        void TestEncodingText(string decoded, byte[] encoded)
        {
            Assert.AreEqual(decoded, encoder.GetString(encoded));
            Assert.AreEqual(encoded, encoder.GetBytes(decoded));
            Assert.AreEqual(decoded.Length, encoder.GetCharCount(encoded));
            Assert.AreEqual(encoded.Length, encoder.GetByteCount(decoded));
            Assert.LessOrEqual(decoded.Length, encoder.GetMaxCharCount(encoded.Length));
            Assert.LessOrEqual(encoded.Length, encoder.GetMaxByteCount(decoded.Length));
        }

        void TestEncodeText(string decoded, byte[] encoded)
        {
            Assert.AreEqual(encoded, encoder.GetBytes(decoded));
            Assert.AreEqual(encoded.Length, encoder.GetByteCount(decoded));
            Assert.LessOrEqual(encoded.Length, encoder.GetMaxByteCount(decoded.Length));
        }
    }
}
