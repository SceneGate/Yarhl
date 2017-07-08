// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
// namespace RTI.Connector
namespace Libgame.UnitTests.IO.Encodings
{
    using System;
    using System.Text;
    using Libgame.IO.Encodings;
    using NUnit.Framework;

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

            // We can't override original
            Encoding baseEncoding = encoder;
            Assert.IsNotInstanceOf<DecoderReplacementFallback>(baseEncoding.DecoderFallback);
            Assert.IsNotInstanceOf<EncoderReplacementFallback>(baseEncoding.EncoderFallback);
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
                () => encoder.GetByteCount(null, 0, 1));
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

        [Test]
        public void ProtectedMethodsThrowException()
        {
            var customEncoder = new ByPassEncoding();
            Assert.Throws<ArgumentNullException>(customEncoder.TestDecoderNull1);
            Assert.Throws<ArgumentNullException>(customEncoder.TestDecoderNull2);
            Assert.Throws<ArgumentNullException>(customEncoder.TestEncoderNull1);
            Assert.Throws<ArgumentNullException>(customEncoder.TestEncoderNull2);
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

        class ByPassEncoding : EucJpEncoding
        {
            public void TestDecoderNull1()
            {
                int count = 0;
                DecodeText(null, (arg1, arg2) => count++);
            }

            public void TestDecoderNull2()
            {
                var dummy = new System.IO.MemoryStream();
                DecodeText(dummy, null);
            }

            public void TestEncoderNull1()
            {
                int count = 0;
                EncodeText(null, (arg1, arg2) => count++);
            }

            public void TestEncoderNull2()
            {
                EncodeText("a", null);
            }
        }
    }
}
