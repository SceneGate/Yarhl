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
    /// Custom text encoding based on Span types simple to implement.
    /// </summary>
    public abstract class SimpleSpanEncoding : Encoding
    {
                /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSpanEncoding" /> class.
        /// </summary>
        /// <param name="codePage">The code page of the encoding.</param>
        /// <remarks>
        /// The encoder and decoder fallback throw exceptions.
        /// </remarks>
        protected SimpleSpanEncoding(int codePage)
            : base(codePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSpanEncoding" /> class.
        /// </summary>
        /// <param name="codePage">The code page of the encoding.</param>
        /// <param name="encoderFallback">The encoder fallback.</param>
        /// <param name="decoderFallback">The decoder fallback.</param>
        protected SimpleSpanEncoding(int codePage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
            : base(codePage, encoderFallback, decoderFallback)
        {
        }

        /// <summary>
        /// Gets the human-readable description of the current encoding.
        /// </summary>
        public abstract override string EncodingName { get; }

        /// <summary>
        /// Gets a value indicating whether the current encoding uses single-byte code points.
        /// </summary>
        public override bool IsSingleByte => GetMaxByteCount(1) == 1;

        /// <summary>
        /// Gets the Windows operating system code page that most closely corresponds to the current encoding.
        /// This property is not in used anymore. By default is `0`.
        /// </summary>
        public override int WindowsCodePage => 0;

        /// <summary>
        /// Gets a value indicating whether the current encoding can be used by mail and news clients for displaying content.
        /// As it's a custom encoding and the other side may not have it, by default is `false`.
        /// </summary>
        public override bool IsMailNewsDisplay => false;

        /// <summary>
        /// Gets a value indicating whether the current encoding can be used by mail and news clients for saving content.
        /// As it's a custom encoding and the other side may not have it, by default is `false`.
        /// </summary>
        public override bool IsMailNewsSave => false;

        /// <summary>
        /// Gets a name for the current encoding that can be used with mail agent header tags.
        /// </summary>
        public override string HeaderName => EncodingName;

        /// <summary>
        /// Gets a name for the current encoding that can be used with mail agent body tags.
        /// </summary>
        public override string BodyName => EncodingName;

        /// <summary>
        /// Gets a value indicating whether the current encoding can be used by browser clients for displaying content.
        /// As it's a custom encoding and the other side may not have it, by default is `false`.
        /// </summary>
        public override bool IsBrowserDisplay => false;

        /// <summary>
        /// Gets a value indicating whether the current encoding can be used by browser clients for saving content.
        /// As it's a custom encoding and the other side may not have it, by default is `false`.
        /// </summary>
        public override bool IsBrowserSave => false;

        /// <summary>
        /// Gets the name registered with the Internet Assigned Numbers Authority (IANA) for the current encoding.
        /// As it's a custom encoding, no name is registered in IANA, return empty string.
        /// </summary>
        public override string WebName => string.Empty;

        /// <inheritdoc/>
        public override int GetByteCount(string s) => GetByteCount(s.AsSpan());

        /// <inheritdoc/>
        public override int GetByteCount(char[] chars) => GetByteCount(chars.AsSpan());

        /// <inheritdoc/>
        public override int GetByteCount(char[] chars, int index, int count) =>
            GetByteCount(chars.AsSpan(index, count));

        /// <inheritdoc/>
        public override int GetByteCount(ReadOnlySpan<char> chars)
        {
            var buffer = new SpanStream<byte>(Span<byte>.Empty);
            Encode(chars, buffer);
            return buffer.Length;
        }

        /// <inheritdoc/>
        public override byte[] GetBytes(string s) => GetBytes(s.AsSpan());

        /// <inheritdoc/>
        public override byte[] GetBytes(char[] chars) => GetBytes(chars.AsSpan());

        /// <inheritdoc/>
        public override byte[] GetBytes(char[] chars, int index, int count) =>
            GetBytes(chars.AsSpan(index, count));

        /// <summary>
        /// Encodes the characters.
        /// </summary>
        /// <param name="chars">The characters to encode.</param>
        /// <returns>The output encoded data.</returns>
        public byte[] GetBytes(ReadOnlySpan<char> chars)
        {
            int length = GetByteCount(chars);
            byte[] buffer = new byte[length];
            _ = GetBytes(chars, buffer);

            return buffer;
        }

        /// <inheritdoc/>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(chars.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));

        /// <inheritdoc/>
        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex) =>
            GetBytes(s.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));

        /// <inheritdoc/>
        public override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            var buffer = new SpanStream<byte>(bytes);
            Encode(chars, buffer);

            return buffer.Length;
        }

        /// <inheritdoc/>
        public override int GetCharCount(byte[] bytes) => GetCharCount(bytes.AsSpan());

        /// <inheritdoc/>
        public override int GetCharCount(byte[] bytes, int index, int count) =>
            GetCharCount(bytes.AsSpan(index, count));

        /// <inheritdoc/>
        public override int GetCharCount(ReadOnlySpan<byte> bytes)
        {
            var buffer = new SpanStream<char>(Span<char>.Empty);
            Decode(bytes, buffer);
            return buffer.Length;
        }

        /// <inheritdoc/>
        public override char[] GetChars(byte[] bytes) => GetChars(bytes.AsSpan());

        /// <inheritdoc/>
        public override char[] GetChars(byte[] bytes, int index, int count) => GetChars(bytes.AsSpan(index, count));

        /// <summary>
        /// Decodes the bytes.
        /// </summary>
        /// <param name="bytes">The data to decode.</param>
        /// <returns>The decoded characters.</returns>
        public char[] GetChars(ReadOnlySpan<byte> bytes)
        {
            int length = GetCharCount(bytes);
            char[] buffer = new char[length];
            _ = GetChars(bytes, buffer.AsSpan());
            return buffer;
        }

        /// <inheritdoc/>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) =>
            GetChars(bytes.AsSpan(byteIndex, byteCount), chars.AsSpan(charIndex));

        /// <inheritdoc/>
        public override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            var stream = new SpanStream<char>(chars);
            Decode(bytes, stream);

            return stream.Length;
        }

        /// <inheritdoc/>
        public override string GetString(byte[] bytes) => GetString(bytes, 0, bytes.Length);

        /// <inheritdoc/>
        public override string GetString(byte[] bytes, int index, int count)
        {
            int length = GetCharCount(bytes.AsSpan(index, count));
            return string.Create(length, ValueTuple.Create(this, bytes, index, count), (chars, state) =>
            {
                var (encoding, stateBytes, stateIndex, stateCount) = state;
                _ = encoding.GetChars(stateBytes.AsSpan(stateIndex, stateCount), chars);
            });
        }

        /// <summary>
        /// Encodes the characters into the stream.
        /// </summary>
        /// <param name="chars">The characters to encode.</param>
        /// <param name="buffer">The output byte stream.</param>
        protected abstract void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer);

        /// <summary>
        /// Decodes the bytes into a char stream.
        /// </summary>
        /// <param name="bytes">The bytes to decode.</param>
        /// <param name="buffer">The output char stream.</param>
        protected abstract void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer);
    }
}
