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
namespace Yarhl.Media.Text.Encodings
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Provides an encoding for encode and decode symbols out of range of any encoding.
    /// </summary>
    public class EscapeOutRangeEncoding : Encoding
    {
        const char Replacement = 'x';
        readonly Encoding baseEncoding;
        readonly byte[] tokenStart;
        readonly byte[] tokenEnd;
        readonly byte[] replacement;

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeOutRangeEncoding"/> class.
        /// </summary>
        /// <param name="baseEncodingName">Base encoding name.</param>
        public EscapeOutRangeEncoding(string baseEncodingName)
            : this(GetEncoding(baseEncodingName, new EncoderExceptionFallback(), new EscapeOutRangeDecoderFallback()))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeOutRangeEncoding"/> class.
        /// </summary>
        /// <param name="encoding">Base encoding.</param>
        /// <remarks>
        /// <para>For correct usage, make sure that your encoding uses the
        /// <see cref="EscapeOutRangeDecoderFallback"/> as the decoder fallback.</para>
        /// </remarks>
        public EscapeOutRangeEncoding(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            baseEncoding = encoding;
            tokenStart = baseEncoding.GetBytes(TokenStart);
            tokenEnd = baseEncoding.GetBytes(TokenEnd);
            replacement = baseEncoding.GetBytes(Replacement.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Gets the start of the token for invalid symbols.
        /// </summary>
        /// <value>The token start.</value>
        public static string TokenStart => "[@!!";

        /// <summary>
        /// Gets the end of the token for invalid symbols.
        /// </summary>
        /// <value>The token end.</value>
        public static string TokenEnd => "]";

        /// <summary>
        /// Gets the byte count.
        /// </summary>
        /// <returns>The byte count.</returns>
        /// <param name="chars">Chars to convert.</param>
        /// <param name="index">Index of the char array.</param>
        /// <param name="count">Count in the char array.</param>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            return baseEncoding.GetByteCount(UnescapeText(new string(chars, index, count)));
        }

        /// <summary>
        /// Gets the char count.
        /// </summary>
        /// <returns>The char count.</returns>
        /// <param name="bytes">Bytes to convert.</param>
        /// <param name="index">Index of the byte array.</param>
        /// <param name="count">Count of the byte array.</param>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return baseEncoding.GetCharCount(bytes, index, count);
        }

        /// <summary>
        /// Gets the encoded bytes.
        /// </summary>
        /// <returns>The encoded bytes.</returns>
        /// <param name="chars">Chars to convert.</param>
        /// <param name="charIndex">Index in the char array.</param>
        /// <param name="charCount">Number of chars to convert.</param>
        /// <param name="bytes">Output byte array.</param>
        /// <param name="byteIndex">Index in the byte array.</param>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (charIndex < 0 || charIndex > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex));
            if (charCount < 0 || charIndex + charCount > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charCount));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (byteIndex < 0 || byteIndex >= bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex));

            int startIdx = byteIndex;

            // Gets the decoded bytes
            List<byte> symbols = new List<byte>();
            int symbolIdx = 0;
            string transformed = UnescapeText(new string(chars, charIndex, charCount), symbols);
            byte[] buffer = baseEncoding.GetBytes(transformed);

            // Now replace the bytes for the token with the actual value
            // In this way we avoid the encoder to process the invalid symbols
            int pos = 0;
            while (pos < buffer.Length) {
                if (!MatchSequence(buffer, pos, tokenStart)) {
                    bytes[byteIndex++] = buffer[pos++];
                    continue;
                }

                // Remove the token start
                pos += tokenStart.Length;

                // For each char, take the next symbol
                while (!MatchSequence(buffer, pos, tokenEnd)) {
                    pos += replacement.Length;
                    bytes[byteIndex++] = symbols[symbolIdx++];
                }

                // Remove the token end
                pos += tokenEnd.Length;
            }

            return byteIndex - startIdx;
        }

        /// <summary>
        /// Gets the decoded chars.
        /// </summary>
        /// <returns>The decoded chars.</returns>
        /// <param name="bytes">Encoded bytes.</param>
        /// <param name="byteIndex">Index in the encoded bytes.</param>
        /// <param name="byteCount">Number of bytes to decoded.</param>
        /// <param name="chars">Output char array.</param>
        /// <param name="charIndex">Index in the char array.</param>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            // It will call our fallback in case of invalid chars
            // The fallback will encode the wrong byte with a special tag.
            return baseEncoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }

        /// <summary>
        /// Gets the max byte count.
        /// </summary>
        /// <returns>The max byte count.</returns>
        /// <param name="charCount">Char count.</param>
        public override int GetMaxByteCount(int charCount)
        {
            return baseEncoding.GetMaxByteCount(charCount);
        }

        /// <summary>
        /// Gets the max char count.
        /// </summary>
        /// <returns>The max char count.</returns>
        /// <param name="byteCount">Byte count.</param>
        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount * baseEncoding.DecoderFallback.MaxCharCount;
        }

        static bool MatchSequence(IList<byte> buffer, int index, params byte[] sequence)
        {
            if (index + sequence.Length > buffer.Count) {
                return false;
            }

            for (int i = 0; i < sequence.Length; i++) {
                if (buffer[index + i] != sequence[i]) {
                    return false;
                }
            }

            return true;
        }

        static string UnescapeText(string text, ICollection<byte>? symbols = null)
        {
            StringBuilder transformed = new StringBuilder(text);
            StringComparison culture = StringComparison.Ordinal;

            int tokenIdx = -1;
            while ((tokenIdx = text.IndexOf(TokenStart, tokenIdx + 1, culture)) != -1) {
                int endTokenIdx = text.IndexOf(TokenEnd, tokenIdx, culture);
                if (endTokenIdx == -1)
                    throw new EncoderFallbackException("Missing end token");

                int tokenLength = endTokenIdx - tokenIdx - TokenStart.Length - TokenEnd.Length + 1;
                string token = text.Substring(tokenIdx + TokenStart.Length, tokenLength);

                for (int tokenPos = 0; symbols != null && tokenPos < tokenLength; tokenPos += 2)
                    symbols.Add(System.Convert.ToByte(token.Substring(tokenPos, 2), 16));

                // Replace the token for a dummy char repeated the number of chars.
                // We don't want to use the original character since we won't be able to
                // call the default encoding later
                string replace = new string(Replacement, tokenLength / 2);
                if (symbols != null)
                    replace = TokenStart + replace + TokenEnd;

                transformed.Replace(TokenStart + token + TokenEnd, replace);
            }

            return transformed.ToString();
        }

        /// <summary>
        /// Provides an encoding fallback mechanism for symbols out of the range
        /// of an encoding.
        /// </summary>
        internal sealed class EscapeOutRangeDecoderFallback : DecoderFallback
        {
            /// <summary>
            /// Gets the number of chars for the invalid symbols.
            /// </summary>
            /// <value>The max char count.</value>
            public override int MaxCharCount => EscapeOutRangeEncoding.TokenStart.Length +
                                                2 + // byte in hexadecimal
                                                EscapeOutRangeEncoding.TokenEnd.Length;

            /// <summary>
            /// Creates the fallback buffer.
            /// </summary>
            /// <returns>The fallback buffer.</returns>
            public override DecoderFallbackBuffer CreateFallbackBuffer()
            {
                return new EscapeOutRangeDecoderFallbackBuffer();
            }
        }

        /// <summary>
        /// Provides a substitute string for invalid symbols of an encoding.
        /// </summary>
        internal sealed class EscapeOutRangeDecoderFallbackBuffer : DecoderFallbackBuffer
        {
            string replacement = string.Empty;
            int currentPos;

            /// <summary>
            /// Gets the number of remaining chars in the replacement string.
            /// </summary>
            /// <value>The number of remaining chars.</value>
            public override int Remaining => replacement.Length - currentPos;

            /// <summary>
            /// Creates the fallback for the specified buffer.
            /// </summary>
            /// <returns>Returns <see langword="true" />.</returns>
            /// <param name="bytesUnknown">Unknown bytes to replace.</param>
            /// <param name="index">Index in the external replacement string.</param>
            public override bool Fallback(byte[] bytesUnknown, int index)
            {
                replacement =
                    EscapeOutRangeEncoding.TokenStart +
                    BitConverter.ToString(bytesUnknown).Replace("-", string.Empty) +
                    EscapeOutRangeEncoding.TokenEnd;
                currentPos = 0;
                return true;
            }

            /// <summary>
            /// Gets the next char of the buffer.
            /// </summary>
            /// <returns>The next char.</returns>
            public override char GetNextChar()
            {
                return currentPos == replacement.Length ? '\0' : replacement[currentPos++];
            }

            /// <summary>
            /// Moves to the previous position in the buffer.
            /// </summary>
            /// <returns>Returns <see langword="true" /> if it was able to move back.</returns>
            public override bool MovePrevious()
            {
                if (currentPos == 0)
                    return false;

                currentPos--;
                return true;
            }

            /// <summary>
            /// Reset this instance.
            /// </summary>
            public override void Reset()
            {
                base.Reset();
                currentPos = 0;
                replacement = string.Empty;
            }
        }
    }
}
