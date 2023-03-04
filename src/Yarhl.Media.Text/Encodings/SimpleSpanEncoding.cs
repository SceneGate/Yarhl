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

        /// <summary>
        /// Calculates the number of bytes produced by encoding the characters
        /// in the specified string.
        /// </summary>
        /// <param name="s">The string containing the set of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public override int GetByteCount(string s)
        {
            ArgumentNullException.ThrowIfNull(s);
            return GetByteCount(s.AsSpan());
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding all the
        /// characters in the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the characters to encode.</param>
        /// <returns>The number of bytes produced by encoding all the characters
        /// in the specified character array.</returns>
        public override int GetByteCount(char[] chars)
        {
            ArgumentNullException.ThrowIfNull(chars);
            return GetByteCount(chars.AsSpan());
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding a set of
        /// characters from the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="index">The index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified characters.</returns>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(chars);
            return GetByteCount(chars.AsSpan(index, count));
        }

        /// <summary>
        /// Calculates the number of bytes produced by encoding the characters
        /// in the specified character span.
        /// </summary>
        /// <param name="chars">The span of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified character span.</returns>
        public override int GetByteCount(ReadOnlySpan<char> chars)
        {
            var buffer = new SpanStream<byte>(Span<byte>.Empty);
            Encode(chars, buffer);
            return buffer.Length;
        }

        /// <inheritdoc/>
        public override byte[] GetBytes(string s)
        {
            ArgumentNullException.ThrowIfNull(s);
            return GetBytes(s.AsSpan());
        }

        /// <inheritdoc/>
        public override byte[] GetBytes(char[] chars)
        {
            ArgumentNullException.ThrowIfNull(chars);
            return GetBytes(chars.AsSpan());
        }

        /// <inheritdoc/>
        public override byte[] GetBytes(char[] chars, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(chars);
            return GetBytes(chars.AsSpan(index, count));
        }

        /// <summary>
        /// Encodes the characters.
        /// </summary>
        /// <param name="chars">The characters to encode.</param>
        /// <returns>The output encoded data.</returns>
        public byte[] GetBytes(ReadOnlySpan<char> chars)
        {
            // This will trigger the Encode call twice: get buffer length + encode in buffer.
            // It will hit performance but it should consume less memory than
            // using a dynamic list and copying again into an array.
            int length = GetByteCount(chars);
            byte[] buffer = new byte[length];
            _ = GetBytes(chars, buffer);

            return buffer;
        }

        /// <inheritdoc/>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            ArgumentNullException.ThrowIfNull(chars);
            ArgumentNullException.ThrowIfNull(bytes);
            return GetBytes(chars.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));
        }

        /// <inheritdoc/>
        public override int GetBytes(string s, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            ArgumentNullException.ThrowIfNull(s);
            ArgumentNullException.ThrowIfNull(bytes);
            return GetBytes(s.AsSpan(charIndex, charCount), bytes.AsSpan(byteIndex));
        }

        /// <inheritdoc/>
        public override int GetBytes(ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            var buffer = new SpanStream<byte>(bytes);
            Encode(chars, buffer);

            return buffer.Length;
        }

        /// <inheritdoc/>
        public override int GetCharCount(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            return GetCharCount(bytes.AsSpan());
        }

        /// <inheritdoc/>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            return GetCharCount(bytes.AsSpan(index, count));
        }

        /// <inheritdoc/>
        public override int GetCharCount(ReadOnlySpan<byte> bytes)
        {
            var buffer = new SpanStream<char>(Span<char>.Empty);
            Decode(bytes, buffer);
            return buffer.Length;
        }

        /// <inheritdoc/>
        public override char[] GetChars(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            return GetChars(bytes.AsSpan());
        }

        /// <inheritdoc/>
        public override char[] GetChars(byte[] bytes, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            return GetChars(bytes.AsSpan(index, count));
        }

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
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            ArgumentNullException.ThrowIfNull(chars);
            return GetChars(bytes.AsSpan(byteIndex, byteCount), chars.AsSpan(charIndex));
        }

        /// <inheritdoc/>
        public override int GetChars(ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            var stream = new SpanStream<char>(chars);
            Decode(bytes, stream);

            return stream.Length;
        }

        /// <inheritdoc/>
        public override string GetString(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            return GetString(bytes, 0, bytes.Length);
        }

        /// <inheritdoc/>
        public override string GetString(byte[] bytes, int index, int count)
        {
            ArgumentNullException.ThrowIfNull(bytes);

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
        /// <param name="isFallbackText">Value indicating if the unknown char comes from fallback text already.</param>
        protected abstract void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer, bool isFallbackText = false);

        /// <summary>
        /// Decodes the bytes into a char stream.
        /// </summary>
        /// <param name="bytes">The bytes to decode.</param>
        /// <param name="buffer">The output char stream.</param>
        protected abstract void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer);

        /// <summary>
        /// Reports an invalid code-point for this encoding and encode the fallback text.
        /// </summary>
        /// <param name="buffer">The current output byte buffer to encode the fallback text.</param>
        /// <param name="codePoint">The UTF-32 code-point that cannot be encoded.</param>
        /// <param name="index">The index where the code-point was found in the buffer.</param>
        /// <param name="isFallbackText">Value indicating if the unknown char comes from fallback text already.</param>
        /// <exception cref="FormatException">Thrown when the fallback text cannot be encoded.</exception>
        protected void EncodeUnknownChar(SpanStream<byte> buffer, int codePoint, int index, bool isFallbackText)
        {
            // Prevent infinite loops reporting encoding errors of the error text
            if (isFallbackText)
            {
                throw new FormatException(
                    "Cannot encode the character given by the encoder fallback buffer");
            }

            EncoderFallbackBuffer fallbackBuffer = EncoderFallback.CreateFallbackBuffer();

            // Report the code-point that we cannot encoding as a UTF-16 character
            // The fallback may find an alternative character to encode,
            // replace with an error char like '?' or throw an exception.
            string ch = char.ConvertFromUtf32(codePoint);
            bool encodeFallbackBuffer = (ch.Length == 1)
                ? fallbackBuffer.Fallback(ch[0], index)
                : fallbackBuffer.Fallback(ch[0], ch[1], index);

            // If provided, encoding the new character from the fallback buffer.
            if (encodeFallbackBuffer)
            {
                var bufferString = new StringBuilder();
                while (fallbackBuffer.Remaining > 0)
                {
                    _ = bufferString.Append(fallbackBuffer.GetNextChar());
                }

                // We can't use the GetChunk API since we need the full string in one call.
                ReadOnlySpan<char> errorText = bufferString.ToString().AsSpan();

                Encode(errorText, buffer, true);
            }
        }

        /// <summary>
        /// Reports invalid encoded bytes and write the fallback text to the stream depending on the fallback strategy.
        /// </summary>
        /// <param name="buffer">The output chars buffer to write the fallback text.</param>
        /// <param name="index">The index where the invalid data was found.</param>
        /// <param name="bytesUnknown">The invalid unknown bytes found.</param>
        protected void DecodeUnknownBytes(SpanStream<char> buffer, int index, params byte[] bytesUnknown)
        {
            // Report that we cannot decode these bytes
            DecoderFallbackBuffer fallbackBuffer = DecoderFallback.CreateFallbackBuffer();
            bool writeFallbackChars = fallbackBuffer.Fallback(bytesUnknown, index);

            // We got text to write as replacement for the wrong data.
            while (writeFallbackChars && fallbackBuffer.Remaining > 0) {
                buffer.Write(fallbackBuffer.GetNextChar());
            }
        }
    }
}
