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
    /// Custom text encoding based on Span types.
    /// </summary>
    public abstract class PropietarySpanEncoding : Encoding
    {
        public PropietarySpanEncoding(int codePage)
            : base(codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
        {
        }

        public PropietarySpanEncoding(int codePage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
            : base(codePage, encoderFallback, decoderFallback)
        {
        }

        public abstract override string EncodingName { get; }

        public override bool IsSingleByte => GetMaxByteCount(1) == 1;

        public override string BodyName { get; } = string.Empty;

        public override bool IsMailNewsDisplay { get; } = false;

        public override bool IsBrowserDisplay { get; } = false;

        public override bool IsBrowserSave { get; } = false;

        public override bool IsMailNewsSave { get; } = false;

        public override string HeaderName { get; } = string.Empty;

        public override string WebName { get; } = string.Empty;

        public override int WindowsCodePage { get; } = 0;

        public override int GetByteCount(string s) => GetByteCount(s.AsSpan());

        public override int GetByteCount(char[] chars) => GetByteCount(chars.AsSpan());

        public override int GetByteCount(char[] chars, int index, int count) => GetByteCount(chars.AsSpan(index, count));

        public int GetBytes(ReadOnlySpan<char> chars, byte[] bytes, int byteIndex) =>
            GetBytes(chars, bytes.AsSpan(byteIndex));

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(chars.AsSpan(charIndex, charCount), bytes, byteIndex);

        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(s.AsSpan(charIndex, charCount), bytes, byteIndex);

        public byte[] GetBytes(ReadOnlySpan<char> chars)
        {
            int length = GetByteCount(chars);
            byte[] buffer = new byte[length];
            GetBytes(chars, buffer);

            return buffer;
        }

        public override byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        public override byte[] GetBytes(char[] chars) => GetBytes(chars.AsSpan());

        public override byte[] GetBytes(char[] chars, int index, int count) =>
            GetBytes(chars.AsSpan(index, count));

        // TODO: Continue porting to Span

        public override string GetString(byte[] bytes)
        {
            var length = GetCharCount(bytes.AsSpan());
            return string.Create(length, ValueTuple.Create(this, bytes), (chars, state) =>
            {
                (PropietarySpanEncoding encoding, byte[] bytes) = state;
                encoding.GetChars(bytes, chars);
            });
        }

        public override int GetCharCount(byte[] bytes) =>
            GetCharCount(bytes.AsSpan());

        public override int GetCharCount(byte[] bytes, int index, int count) =>
            GetCharCount(bytes.AsSpan(index, count));

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) =>
            GetChars(bytes.AsSpan(byteIndex, byteCount), chars.AsSpan(charIndex));

        public abstract override int GetByteCount(ReadOnlySpan<char> chars);

        public abstract override int GetCharCount(ReadOnlySpan<byte> bytes);

        public abstract override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes);

        public abstract override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars);
    }
}
