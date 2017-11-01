//
// EucJpEncoding.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Yarhl.IO.Encodings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// EUC-JP encoding.
    /// Implemented standard from: https://encoding.spec.whatwg.org/
    /// </summary>
    public sealed class EucJpEncoding : Encoding
    {
        static Dictionary<int, int> idx2CodePointJs212;
        static Dictionary<int, int> idx2CodePointJs208;
        static Dictionary<int, int> codePoint2IdxJs212;
        static Dictionary<int, int> codePoint2IdxJs208;
        readonly DecoderFallback decoderFallback;
        readonly EncoderFallback encoderFallback;

        static EucJpEncoding()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            idx2CodePointJs208 = new Dictionary<int, int>();
            codePoint2IdxJs208 = new Dictionary<int, int>();
            FillCodecTable(
                assembly.GetManifestResourceStream("Yarhl.IO.Encodings.index-jis0208.txt"),
                idx2CodePointJs208,
                codePoint2IdxJs208);
            
            idx2CodePointJs212 = new Dictionary<int, int>();
            codePoint2IdxJs212 = new Dictionary<int, int>();
            FillCodecTable(
                assembly.GetManifestResourceStream("Yarhl.IO.Encodings.index-jis0212.txt"),
                idx2CodePointJs212,
                codePoint2IdxJs212);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EucJpEncoding"/> class.
        /// </summary>
        public EucJpEncoding()
        {
            decoderFallback = new DecoderExceptionFallback();
            encoderFallback = new EncoderExceptionFallback();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EucJpEncoding"/> class.
        /// </summary>
        /// <param name="decFallback">Decoder fallback.</param>
        /// <param name="encFallback">Encoder fallback.</param>
        public EucJpEncoding(DecoderFallback decFallback, EncoderFallback encFallback)
        {
            if (decFallback == null)
                throw new ArgumentNullException(nameof(decFallback));
            if (encFallback == null)
                throw new ArgumentNullException(nameof(encFallback));

            decoderFallback = decFallback;
            encoderFallback = encFallback;
        }

        /// <summary>
        /// Gets the decoder fallback.
        /// </summary>
        /// <value>The decoder fallback.</value>
        public new DecoderFallback DecoderFallback {
            get { return decoderFallback; }
        }

        /// <summary>
        /// Gets the encoder fallback.
        /// </summary>
        /// <value>The encoder fallback.</value>
        public new EncoderFallback EncoderFallback {
            get { return encoderFallback; }
        }

        /// <summary>
        /// Gets the byte count.
        /// </summary>
        /// <returns>The byte count.</returns>
        /// <param name="chars">Chars to convert.</param>
        /// <param name="index">Index of the char array.</param>
        /// <param name="count">Count in the char array.</param>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (count < 0 || count > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index < 0 || index + count > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int length = 0;
            string text = new string(chars, index, count);
            EncodeText(text, (str, b) => length++);
            return length;
        }

        /// <summary>
        /// Gets the encoded bytes.
        /// </summary>
        /// <returns>The encoded bytes.</returns>
        /// <param name="chars">Chars to convert.</param>
        /// <param name="charIndex">Index in the char array.</param>
        /// <param name="charCount">Number of chars to convert.</param>
        /// <param name="bytes">Output byte array.</param>
        /// <param name="byteIndex">Indes in the byte array.</param>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (charCount < 0 || charCount > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charCount));
            if (charIndex < 0 || charIndex + charCount > chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (byteIndex < 0 || byteIndex >= bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex));

            using (var stream = new MemoryStream(bytes, byteIndex, bytes.Length)) {
                string text = new string(chars, charIndex, charCount);
                EncodeText(text, (str, b) => stream.WriteByte(b));
                return (int)stream.Length;
            }
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
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (count < 0 || count > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (index < 0 || index + count > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            int chars = 0;
            using (MemoryStream stream = new MemoryStream(bytes, index, count))
                DecodeText(stream, (str, ch) => chars += ch.Length);
            return chars;
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
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));
            if (byteCount < 0 || byteCount > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteCount));
            if (byteIndex < 0 || byteIndex + byteCount > bytes.Length)
                throw new ArgumentOutOfRangeException(nameof(byteIndex));

            if (chars == null)
                throw new ArgumentNullException(nameof(chars));
            if (charIndex < 0 || charIndex >= chars.Length)
                throw new ArgumentOutOfRangeException(nameof(charIndex));

            StringBuilder text = new StringBuilder();
            using (MemoryStream stream = new MemoryStream(bytes, byteIndex, byteCount))
                DecodeText(stream, (str, ch) => text.Append(ch));

            text.CopyTo(0, chars, charIndex, text.Length);
            return text.Length;
        }

        /// <summary>
        /// Gets the max byte count.
        /// </summary>
        /// <returns>The max byte count.</returns>
        /// <param name="charCount">Char count.</param>
        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0)
                throw new ArgumentOutOfRangeException(nameof(charCount));

            return charCount * 3;
        }

        /// <summary>
        /// Gets the max char count.
        /// </summary>
        /// <returns>The max char count.</returns>
        /// <param name="byteCount">Byte count.</param>
        public override int GetMaxCharCount(int byteCount)
        {
            if (byteCount < 0)
                throw new ArgumentOutOfRangeException(nameof(byteCount));

            return byteCount;
        }

        /// <summary>
        /// Internal text encoder.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <param name="encodedByte">Callback with the byte encoded.</param>
        void EncodeText(string text, Action<Stream, byte> encodedByte)
        {
            MemoryStream stream = new MemoryStream(UTF32.GetBytes(text));

            // 1
            while (stream.Position < stream.Length) {
                byte[] buffer = new byte[4];
                stream.Read(buffer, 0, 4);
                int codePoint = BitConverter.ToInt32(buffer, 0);

                if (codePoint <= 0x7F) {
                    // 2
                    encodedByte(stream, (byte)(codePoint & 0xFF));
                } else if (codePoint == 0xA5) {
                    // 3
                    encodedByte(stream, 0x5C);
                } else if (codePoint == 0x203E) {
                    // 4
                    encodedByte(stream, 0x7E);
                } else if (IsInRange(codePoint, 0xFF61, 0xFF9F)) {
                    // 5
                    encodedByte(stream, 0x8E);
                    encodedByte(stream, (byte)(codePoint - 0xFF61 + 0xA1));
                } else {
                    // 6
                    if (codePoint == 0x2212)
                        codePoint = 0xFF0D;

                    // 8
                    if (!codePoint2IdxJs208.ContainsKey(codePoint)) {
                        if (codePoint2IdxJs212.ContainsKey(codePoint)) {
                            // Fixed, not in the specs
                            int pointer212 = codePoint2IdxJs212[codePoint];
                            encodedByte(stream, 0x8F);
                            encodedByte(stream, (byte)((pointer212 / 94) + 0xA1));
                            encodedByte(stream, (byte)((pointer212 % 94) + 0xA1));
                        } else {
                            var fallback = encoderFallback.CreateFallbackBuffer();
                            string ch = char.ConvertFromUtf32(codePoint);
                            if (ch.Length == 1)
                                fallback.Fallback(ch[0], 0);
                            else
                                fallback.Fallback(ch[0], ch[1], 0);

                            while (fallback.Remaining > 0)
                                encodedByte(stream, (byte)fallback.GetNextChar());
                        }
                    } else {
                        // 7
                        int pointer = codePoint2IdxJs208[codePoint];
                        encodedByte(stream, (byte)((pointer / 94) + 0xA1)); // 9, 11
                        encodedByte(stream, (byte)((pointer % 94) + 0xA1)); // 10, 11
                    }
                }
            }
        }

        /// <summary>
        /// Internal text decoder.
        /// </summary>
        /// <param name="stream">Stream to decode.</param>
        /// <param name="decodedText">Callback with decoded text.</param>
        void DecodeText(Stream stream, Action<Stream, string> decodedText)
        {
            DecoderFallbackBuffer fallback = decoderFallback.CreateFallbackBuffer();

            byte lead = 0;
            bool jis0212 = false;
            while (stream.Position < stream.Length) {
                byte current = (byte)stream.ReadByte();
                if (lead == 0x8E && IsInRange(current, 0xA1, 0xDF)) {
                    // 3
                    lead = 0;
                    decodedText(stream, char.ConvertFromUtf32(0xFF61 - 0xA1 + current));
                } else if (lead == 0x8F && IsInRange(current, 0xA1, 0xFE)) {
                    // 4
                    jis0212 = true;
                    lead = current;
                } else if (lead != 0x00) {
                    // 5
                    if (IsInRange(lead, 0xA1, 0xFE) && IsInRange(current, 0xA1, 0xFE)) {
                        int tblIdx = ((lead - 0xA1) * 94) + current - 0xA1;
                        int codePoint = jis0212 ? idx2CodePointJs212[tblIdx] : idx2CodePointJs208[tblIdx];
                        decodedText(stream, char.ConvertFromUtf32(codePoint));

                        lead = 0x00;
                        jis0212 = false;
                    } else {
                        bool result = fallback.Fallback(new byte[] { lead, current }, 0);
                        while (result && fallback.Remaining > 0)
                            decodedText(stream, fallback.GetNextChar().ToString());
                    }
                } else if (current <= 0x7F) {
                    // 6
                    decodedText(stream, char.ConvertFromUtf32(current));
                } else if (IsInRange(current, 0x8E, 0x8F) || IsInRange(current, 0xA1, 0xFE)) {
                    // 7
                    lead = current;
                } else {
                    // 8
                    bool result = fallback.Fallback(new byte[] { current }, 0);
                    while (result && fallback.Remaining > 0)
                        decodedText(stream, fallback.GetNextChar().ToString());
                }
            }

            // 1
            if (lead != 0x00) {
                bool result = fallback.Fallback(new byte[] { lead }, 0);
                while (result && fallback.Remaining > 0)
                    decodedText(stream, fallback.GetNextChar().ToString());
            }
        }

        static void FillCodecTable(
            Stream file,
            IDictionary<int, int> idx2CodePoint,
            IDictionary<int, int> codePoint2Idx)
        {
            using (StreamReader reader = new StreamReader(file)) {
                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line))
                        continue;
                    if (line[0] == '#')
                        continue;

                    string[] fields = line.Split('\t');
                    int index = System.Convert.ToInt32(fields[0].TrimStart(' '), 10);
                    int codePoint = System.Convert.ToInt32(fields[1].Substring(2), 16);

                    idx2CodePoint[index] = codePoint;
                    codePoint2Idx[codePoint] = index;
                }
            }
        }

        static bool IsInRange(int val, int min, int max)
        {
            return val >= min && val <= max;
        }
    }
}