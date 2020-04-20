// Copyright (c) 2020 SceneGate

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
namespace Yarhl.PerformanceTests
{
    using System;
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using Yarhl.Media.Text.Encodings;

    [MemoryDiagnoser]
    [CsvExporter]
    public class EncodingSpan
    {
        string text;

        static EncodingSpan()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        [Params(10, 100, 1_000, 100_000, 10 * 1024 * 1024)]
        public int TextLength { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            text = new string('a', TextLength);
        }

        [Benchmark]
        public string Sjis()
        {
            Encoding encoding = Encoding.GetEncoding("shift-jis");
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        [Benchmark(Baseline = true)]
        public string SjisCustomFromNetEncoding()
        {
            EncodingDotNet encoding = new EncodingDotNet();
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        [Benchmark]
        public string SjisCustomFromYarhlEncoding()
        {
            EncodingYarhl encoding = new EncodingYarhl();
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        [Benchmark]
        public string SjisCustomFromYarhlEncoding2()
        {
            EncodingYarhl2 encoding = new EncodingYarhl2();
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        private sealed class EncodingDotNet : Encoding
        {
            public override int GetByteCount(char[] chars, int index, int count)
            {
                int bytes = 0;
                for (int i = 0; i < count; i++) {
                    char ch = chars[index + i];
                    bytes += (ch <= 0x7F || (ch >= 0xA1 && ch <= 0xDF)) ? 1 : 2;
                }

                return bytes;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                int chars = 0;
                int i = 0;
                while (i < count) {
                    byte b = bytes[index + i];
                    chars++;
                    i += (b <= 0x7F || (b >= 0xA1 && b <= 0xDF)) ? 1 : 2;
                }

                return chars;
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                for (int i = 0; i < charCount; i++) {
                    bytes[byteIndex + i] = (byte)chars[charIndex + i];
                }

                return charCount;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                for (int i = 0; i < byteCount; i++) {
                    chars[charIndex + i] = (char)bytes[byteIndex + i];
                }

                return byteCount;
            }

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;
        }

        private sealed class EncodingYarhl : PropietarySpanEncoding
        {
            public EncodingYarhl()
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
            }

            public override string EncodingName => "ascii-yarhl";

            public override int GetByteCount(ReadOnlySpan<char> chars)
            {
                int bytes = 0;
                int length = chars.Length;
                for (int i = 0; i < length; i++) {
                    char ch = chars[i];
                    bytes += (ch <= 0x7F || (ch >= 0xA1 && ch <= 0xDF)) ? 1 : 2;
                }

                return bytes;
            }

            public override int GetCharCount(ReadOnlySpan<byte> bytes)
            {
                int chars = 0;
                int i = 0;
                int length = bytes.Length;
                while (i < length) {
                    byte b = bytes[i];
                    chars++;
                    i += (b <= 0x7F || (b >= 0xA1 && b <= 0xDF)) ? 1 : 2;
                }

                return chars;
            }

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;

            public override int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
            {
                int length = buffer.Length;
                for (int i = 0; i < length; i++) {
                    text[i] = (char)buffer[i];
                }

                return length;
            }

            public override int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
            {
                int length = text.Length;
                for (int i = 0; i < length; i++) {
                    buffer[i] = (byte)text[i];
                }

                return length;
            }
        }

        private sealed class EncodingYarhl2 : PropietarySpanEncodingWithoutCount
        {
            public EncodingYarhl2()
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
            }

            public override string EncodingName => "ascii-yarhl2";

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;

            protected override void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer)
            {
                int length = bytes.Length;
                for (int i = 0; i < length; i++) {
                    buffer.Write((char)bytes[i]);
                }
            }

            protected override void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer)
            {
                int length = chars.Length;
                for (int i = 0; i < length; i++) {
                    buffer.Write((byte)chars[i]);
                }
            }
        }
    }
}
