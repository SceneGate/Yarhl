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

    public class EncodingSpan
    {
        [Params("Hello world!")]
        public string Text { get; set; }

        [Benchmark]
        public string EncodeDotNet()
        {
            return Run(Encoding.ASCII);
        }

        [Benchmark]
        public string EncodeDotNetSimplify()
        {
            return Run(new EncodingDotNet());
        }

        [Benchmark]
        public string EncodeYarhl()
        {
            return Run(new EncodingYarhl());
        }

        private string Run(Encoding encoding)
        {
            byte[] encoded = encoding.GetBytes(Text);
            return encoding.GetString(encoded);
        }

        private sealed class EncodingDotNet : Encoding
        {
            public override int GetByteCount(char[] chars, int index, int count)
            {
                return count;
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                for (int i = 0; i < charCount; i++) {
                    bytes[byteIndex++] = (byte)chars[charIndex + i];
                }

                return charCount;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                return count;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                for (int i = 0; i < byteCount; i++) {
                    chars[charIndex++] = (char)bytes[byteIndex + i];
                }

                return byteCount;
            }

            public override int GetMaxByteCount(int charCount) => charCount;

            public override int GetMaxCharCount(int byteCount) => byteCount;
        }

        private sealed class EncodingYarhl : PropietarySpanEncoding
        {
            public EncodingYarhl()
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
            }

            public override string EncodingName => throw new NotImplementedException();

            public override bool IsSingleByte => throw new NotImplementedException();

            public override int GetMaxByteCount(int charCount) => charCount;

            public override int GetMaxCharCount(int byteCount) => byteCount;

            protected override void Decode(ReadOnlySpan<byte> buffer, Action<char> writeFcn)
            {
                foreach (var b in buffer) {
                    writeFcn((char)b);
                }
            }

            protected override void Encode(ReadOnlySpan<char> text, Action<byte> writeFcn)
            {
                foreach (var ch in text) {
                    writeFcn((byte)ch);
                }
            }
        }
    }
}
