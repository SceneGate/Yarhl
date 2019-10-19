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
    using System.Linq;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.Media.Text.Encodings;

    [TestFixture]
    public class EscapeOutRangeEncodingTests
    {
        [Test]
        public void DecoderFallbackBufferGetsBuffer()
        {
            var buffer = new EscapeOutRangeEncoding.EscapeOutRangeDecoderFallbackBuffer();
            byte[] invalidBuffer = { 0xE2, 0x81 };
            Assert.True(buffer.Fallback(invalidBuffer, 0));

            string replacement = "[@!!E281]";
            for (int i = 0; i < replacement.Length; i++) {
                Assert.AreEqual(replacement.Length - i, buffer.Remaining);
                Assert.AreEqual(replacement[i], buffer.GetNextChar());
            }

            Assert.AreEqual(0, buffer.Remaining);
            Assert.AreEqual('\0', buffer.GetNextChar());
        }

        [Test]
        public void DecoderFallbackBufferResets()
        {
            var buffer = new EscapeOutRangeEncoding.EscapeOutRangeDecoderFallbackBuffer();
            byte[] invalidBuffer = { 0xE2, 0x81 };
            Assert.True(buffer.Fallback(invalidBuffer, 0));

            Assert.AreEqual(9, buffer.Remaining);
            Assert.AreEqual('[', buffer.GetNextChar());
            Assert.AreEqual(8, buffer.Remaining);
            Assert.AreEqual('@', buffer.GetNextChar());
            Assert.AreEqual(7, buffer.Remaining);

            buffer.Reset();
            Assert.AreEqual(0, buffer.Remaining);
            Assert.AreEqual('\0', buffer.GetNextChar());
        }

        [Test]
        public void DecoderFallbackBufferMoveBack()
        {
            var buffer = new EscapeOutRangeEncoding.EscapeOutRangeDecoderFallbackBuffer();
            byte[] invalidBuffer = { 0xE2, 0x81 };
            Assert.True(buffer.Fallback(invalidBuffer, 0));

            Assert.AreEqual(9, buffer.Remaining);
            Assert.AreEqual('[', buffer.GetNextChar());
            Assert.AreEqual(8, buffer.Remaining);
            Assert.AreEqual('@', buffer.GetNextChar());
            Assert.AreEqual(7, buffer.Remaining);

            Assert.IsTrue(buffer.MovePrevious());
            Assert.AreEqual(8, buffer.Remaining);
            Assert.AreEqual('@', buffer.GetNextChar());

            Assert.IsTrue(buffer.MovePrevious());
            Assert.AreEqual(8, buffer.Remaining);
            Assert.IsTrue(buffer.MovePrevious());
            Assert.AreEqual(9, buffer.Remaining);
            Assert.IsFalse(buffer.MovePrevious());
            Assert.AreEqual(9, buffer.Remaining);
            Assert.AreEqual('[', buffer.GetNextChar());
        }

        [Test]
        public void ReplaceInvalidUtf8Symbols()
        {
            Encoding encoding = Encoding.GetEncoding(
                "utf-8",
                new EncoderExceptionFallback(),
                new EscapeOutRangeEncoding.EscapeOutRangeDecoderFallback());

            byte[] invalidBuffer = { 0xE2, 0x81, 0xE3, 0xE4 };
            string output = encoding.GetString(invalidBuffer);
            Assert.AreEqual("[@!!E281][@!!E3][@!!E4]", output);
        }

        [Test]
        public void DoNotChangeValidUtf8Symbols()
        {
            Encoding encoding = Encoding.GetEncoding(
                "utf-8",
                new EncoderExceptionFallback(),
                new EscapeOutRangeEncoding.EscapeOutRangeDecoderFallback());

            byte[] validBuffer = { 0xE3, 0x81, 0x82, 0xE3, 0xE3, 0x81, 0x82 };
            string output = encoding.GetString(validBuffer);
            Assert.AreEqual("あ[@!!E3]あ", output);
        }

        [Test]
        public void EncodingReplaceInvalidUtf8Symbols()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            byte[] invalidBuffer = { 0xE2, 0x81, 0xE3, 0xE4 };
            string output = encoding.GetString(invalidBuffer);
            Assert.AreEqual(output.Length, encoding.GetCharCount(invalidBuffer));
            Assert.Less(output.Length, encoding.GetMaxCharCount(4));
            Assert.AreEqual("[@!!E281][@!!E3][@!!E4]", output);
        }

        [Test]
        public void EncodingThrowExceptionIfNullEncoding()
        {
            Assert.Throws<ArgumentNullException>(() => new EscapeOutRangeEncoding((Encoding)null));
        }

        [Test]
        public void EncodingThrowExceptionIfNullStringEncoding()
        {
            Assert.Throws<ArgumentNullException>(() => new EscapeOutRangeEncoding((string)null));
        }

        [Test]
        public void EncodingDoesNotChangeValidUtf8Symbols()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            byte[] validBuffer = { 0xE3, 0x81, 0x82, 0xE3, 0xE3, 0x81, 0x82 };
            string output = encoding.GetString(validBuffer);
            Assert.AreEqual(output.Length, encoding.GetCharCount(validBuffer));
            Assert.Less(output.Length, encoding.GetMaxCharCount(3));
            Assert.AreEqual("あ[@!!E3]あ", output);
        }

        [Test]
        public void EncodingEncodesValidUtf8Symbols()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            string input = "[@!!E281][@!!E3][@!!E4]";
            byte[] output = encoding.GetBytes(input);
            Assert.AreEqual(output.Length, encoding.GetByteCount(input));
            Assert.Less(output.Length, encoding.GetMaxByteCount(4));
            Assert.IsTrue(output.SequenceEqual(new byte[] { 0xE2, 0x81, 0xE3, 0xE4 }));
        }

        [Test]
        public void EncodingEncodesInvalidUtf8Symbols()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            string input = "あ[@!!E3]あ";
            byte[] output = encoding.GetBytes(input);
            Assert.AreEqual(output.Length, encoding.GetByteCount(input));
            Assert.Less(output.Length, encoding.GetMaxByteCount(3));
            Assert.IsTrue(output.SequenceEqual(new byte[] { 0xE3, 0x81, 0x82, 0xE3, 0xE3, 0x81, 0x82 }));
        }

        [Test]
        public void EncodingEncodesThrowExceptionIfMissingEndToken()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            string input = "あ[@!!E3";
            Assert.Throws<EncoderFallbackException>(() => encoding.GetBytes(input));
        }

        [Test]
        public void EncodingEncodesThrowExceptionIfInvalidHexNumber()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            string input = "あ[@!!Q3]あ";
            Assert.Throws<FormatException>(() => encoding.GetBytes(input));
        }

        [Test]
        public void EncodingThrowsWithInvalidArguments()
        {
            Encoding encoding = new EscapeOutRangeEncoding("utf-8");
            char[] input = "test".ToCharArray();
            byte[] output = new byte[10];

            Assert.Throws<ArgumentNullException>(
                () => encoding.GetBytes((char[])null, 0, 0, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, -1, 1, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, 10, 0, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, 0, -1, output, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, 2, 4, output, 0));
            Assert.Throws<ArgumentNullException>(
                () => encoding.GetBytes(input, 0, 4, null, 0));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, 0, 4, output, -1));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => encoding.GetBytes(input, 0, 4, output, 15));
        }
    }
}
