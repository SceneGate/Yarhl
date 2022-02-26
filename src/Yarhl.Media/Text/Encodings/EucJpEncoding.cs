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
    using System.IO;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// EUC-JP encoding.
    /// Implemented standard from: https://encoding.spec.whatwg.org/.
    /// </summary>
    public sealed class EucJpEncoding : Encoding
    {
        static readonly Table TableJis212 =
            Table.FromResource($"{typeof(EucJpEncoding).Namespace}.index-jis0212.txt");

        static readonly Table TableJis208 =
            Table.FromResource($"{typeof(EucJpEncoding).Namespace}.index-jis0208.txt");

        /// <summary>
        /// Initializes a new instance of the <see cref="EucJpEncoding"/> class.
        /// </summary>
        public EucJpEncoding()
        {
            DecoderFallback = new DecoderExceptionFallback();
            EncoderFallback = new EncoderExceptionFallback();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EucJpEncoding"/> class.
        /// </summary>
        /// <param name="decFallback">Decoder fallback.</param>
        /// <param name="encFallback">Encoder fallback.</param>
        public EucJpEncoding(DecoderFallback decFallback, EncoderFallback encFallback)
        {
            DecoderFallback = decFallback ?? throw new ArgumentNullException(nameof(decFallback));
            EncoderFallback = encFallback ?? throw new ArgumentNullException(nameof(encFallback));
        }

        /// <summary>
        /// Gets the decoder fallback.
        /// </summary>
        /// <value>The decoder fallback.</value>
        public new DecoderFallback DecoderFallback {
            get;
        }

        /// <summary>
        /// Gets the encoder fallback.
        /// </summary>
        /// <value>The encoder fallback.</value>
        public new EncoderFallback EncoderFallback {
            get;
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
        /// <param name="byteIndex">Index in the byte array.</param>
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

        static bool IsInRange(int val, int min, int max)
        {
            return val >= min && val <= max;
        }

        /// <summary>
        /// Internal text encoder.
        /// </summary>
        /// <param name="text">Text to encode.</param>
        /// <param name="encodedByte">Callback with the byte encoded.</param>
        void EncodeText(string text, Action<Stream, byte> encodedByte)
        {
            using (var stream = new MemoryStream(UTF32.GetBytes(text))) {
                EncodeText(stream, encodedByte);
            }
        }

        /// <summary>
        /// Internal text encoder.
        /// </summary>
        /// <param name="stream">Text to encode.</param>
        /// <param name="encodedByte">Callback with the byte encoded.</param>
        void EncodeText(Stream stream, Action<Stream, byte> encodedByte)
        {
            // 1
            while (stream.Position < stream.Length) {
                byte[] buffer = new byte[4];
                _ = stream.Read(buffer, 0, 4);
                int codePoint = BitConverter.ToInt32(buffer, 0);

                // 6
                if (codePoint == 0x2212)
                    codePoint = 0xFF0D;

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
                } else if (TableJis208.CodePoint2Index.ContainsKey(codePoint)) {
                    // 7
                    int pointer = TableJis208.CodePoint2Index[codePoint];
                    encodedByte(stream, (byte)((pointer / 94) + 0xA1)); // 9, 11
                    encodedByte(stream, (byte)((pointer % 94) + 0xA1)); // 10, 11
                } else if (TableJis212.CodePoint2Index.ContainsKey(codePoint)) {
                    // 8, Fixed, not in the specs
                    int pointer212 = TableJis212.CodePoint2Index[codePoint];
                    encodedByte(stream, 0x8F);
                    encodedByte(stream, (byte)((pointer212 / 94) + 0xA1));
                    encodedByte(stream, (byte)((pointer212 % 94) + 0xA1));
                } else {
                    EncodeInvalidChar(codePoint, stream, encodedByte);
                }
            }
        }

        void EncodeInvalidChar(int codePoint, Stream stream, Action<Stream, byte> encodedByte)
        {
            var fallback = EncoderFallback.CreateFallbackBuffer();
            string ch = char.ConvertFromUtf32(codePoint);
            if (ch.Length == 1) {
                _ = fallback.Fallback(ch[0], 0);
            } else {
                _ = fallback.Fallback(ch[0], ch[1], 0);
            }

            while (fallback.Remaining > 0) {
                encodedByte(stream, (byte)fallback.GetNextChar());
            }
        }

        /// <summary>
        /// Internal text decoder.
        /// </summary>
        /// <param name="stream">Stream to decode.</param>
        /// <param name="decodedText">Callback with decoded text.</param>
        void DecodeText(Stream stream, Action<Stream, string> decodedText)
        {
            byte lead = 0;
            IDictionary<int, int> codePointTable = TableJis208.Index2CodePoint;
            while (stream.Position < stream.Length) {
                byte current = (byte)stream.ReadByte();
                if (lead == 0x8E && IsInRange(current, 0xA1, 0xDF)) {
                    // 3
                    lead = 0;
                    decodedText(stream, char.ConvertFromUtf32(0xFF61 - 0xA1 + current));
                } else if (lead == 0x8F && IsInRange(current, 0xA1, 0xFE)) {
                    // 4
                    codePointTable = TableJis212.Index2CodePoint;
                    lead = current;
                } else if (IsInRange(lead, 0xA1, 0xFE) && IsInRange(current, 0xA1, 0xFE)) {
                    // 5
                    int tblIdx = ((lead - 0xA1) * 94) + current - 0xA1;
                    int codePoint = codePointTable[tblIdx];
                    decodedText(stream, char.ConvertFromUtf32(codePoint));

                    lead = 0x00;
                    codePointTable = TableJis208.Index2CodePoint;
                } else if (lead != 0x00) {
                    DecodeInvalidBytes(stream, decodedText, lead, current);
                } else if (current <= 0x7F) {
                    // 6
                    decodedText(stream, char.ConvertFromUtf32(current));
                } else if (IsInRange(current, 0x8E, 0x8F) || IsInRange(current, 0xA1, 0xFE)) {
                    // 7
                    lead = current;
                } else {
                    // 8
                    DecodeInvalidBytes(stream, decodedText, current);
                }
            }

            // 1
            if (lead != 0x00)
                DecodeInvalidBytes(stream, decodedText, lead);
        }

        void DecodeInvalidBytes(Stream stream, Action<Stream, string> decodedText, params byte[] data)
        {
            DecoderFallbackBuffer fallback = DecoderFallback.CreateFallbackBuffer();
            bool result = fallback.Fallback(data, 0);
            while (result && fallback.Remaining > 0) {
                decodedText(stream, $"{fallback.GetNextChar()}");
            }
        }

        sealed class Table
        {
            private Table()
            {
                Index2CodePoint = new Dictionary<int, int>();
                CodePoint2Index = new Dictionary<int, int>();
            }

            public IDictionary<int, int> Index2CodePoint {
                get;
                private set;
            }

            public IDictionary<int, int> CodePoint2Index {
                get;
                private set;
            }

            public static Table FromResource(string path)
            {
                Table table = new Table();

                Stream? stream = null;
                try {
                    // Cannot be null as this is a private call with known files
                    // If the files are not included in the build, tests will fail.
                    #pragma warning disable SA1009 // False positive due to nullable
                    stream = Assembly.GetExecutingAssembly()
                        .GetManifestResourceStream(path)!;
                    #pragma warning restore SA1009

                    using (var reader = new StreamReader(stream)) {
                        stream = null;  // Avoid disposing twice

                        while (!reader.EndOfStream) {
                            string? line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                                continue;
                            if (line[0] == '#')
                                continue;

                            string[] fields = line.Split('\t');

                            string indexText = fields[0].TrimStart(' ');
                            int index = System.Convert.ToInt32(indexText, 10);

                            string codeText = fields[1][2..];
                            int codePoint = System.Convert.ToInt32(codeText, 16);

                            table.Index2CodePoint[index] = codePoint;
                            table.CodePoint2Index[codePoint] = index;
                        }
                    }
                } finally {
                    stream?.Dispose();
                }

                return table;
            }
        }
    }
}
