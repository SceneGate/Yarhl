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
namespace Yarhl.Media.Text.Encodings
{
    using System;
    using System.Text;

    /// <summary>
    /// A propietary text encoding based on Span types.
    /// Candidate to be included in Yarhl.Media.Text
    /// </summary>
    public abstract class PropietarySpanEncoding : Encoding
    {
        public PropietarySpanEncoding(int codePage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
            : base(codePage, encoderFallback, decoderFallback)
        {
        }

        public abstract override string EncodingName { get; }

        public abstract override bool IsSingleByte { get; }

        public override string BodyName { get; } = string.Empty;

        public override bool IsMailNewsDisplay { get; } = false;

        public override bool IsBrowserDisplay { get; } = false;

        public override bool IsBrowserSave { get; } = false;

        public override bool IsMailNewsSave { get; } = false;

        public override string HeaderName { get; } = string.Empty;

        public override string WebName { get; } = string.Empty;

        public override int GetByteCount(ReadOnlySpan<char> chars)
        {
            int numBytes = 0;
            Encode(chars, _ => numBytes++);

            return numBytes;
        }

        public override int GetByteCount(string s) => GetByteCount(s.AsSpan());

        public override int GetByteCount(char[] chars) => GetByteCount(chars.AsSpan());

        public override int GetByteCount(char[] chars, int index, int count) => GetByteCount(chars.AsSpan(index, count));

        public int GetBytes(ReadOnlySpan<char> chars, byte[] bytes, int byteIndex)
        {
            int count = 0;
            Encode(chars, b => bytes[byteIndex + count++] = b);

            return count;
        }

        public override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes) =>
            GetBytes(chars, bytes.ToArray(), 0); // It's less performance that we would like but better than .NET impl.

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(chars.AsSpan(charIndex, charCount), bytes, byteIndex);

        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(s.AsSpan(charIndex, charCount), bytes, byteIndex);

        public byte[] GetBytes(ReadOnlySpan<char> chars)
        {
            int length = GetByteCount(chars);
            byte[] buffer = new byte[length];
            GetBytes(chars, buffer, 0);

            return buffer;
        }

        public override byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        public override byte[] GetBytes(char[] chars) => GetBytes(chars.AsSpan());

        public override byte[] GetBytes(char[] chars, int index, int count) => GetBytes(chars.AsSpan(index, count));

        // TODO: Continue porting to Span

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            int numChars = 0;
            Decode(bytes.AsSpan(index, count), _ => numChars++);

            return numChars;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            int startIndex = charIndex;
            Decode(bytes.AsSpan(byteIndex, byteCount), ch => chars[charIndex++] = ch);

            return charIndex - startIndex;
        }

        protected abstract void Encode(ReadOnlySpan<char> text, Action<byte> writeFcn);

        protected abstract void Decode(ReadOnlySpan<byte> buffer, Action<char> writeFcn);
    }
}