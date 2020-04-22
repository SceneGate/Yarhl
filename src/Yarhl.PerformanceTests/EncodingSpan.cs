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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using BenchmarkDotNet.Attributes;
    using Yarhl.IO;
    using Yarhl.Media.Text.Encodings;

    [MemoryDiagnoser]
    [CsvExporter]
    public class EncodingSpan
    {
        string text;
        static Dictionary<int, int> codeToUnicode;
        static Dictionary<int, int> unicodeToCode;

        static EncodingSpan()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LoadTable();
        }

        [Params(10, 100, 1_000, 100_000, 10 * 1024 * 1024)]
        // [Params(1000)]
        public int TextLength { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            GenerateText();
        }

        void GenerateText()
        {
            var fullText = File.ReadAllText("japanese.txt");
            if (TextLength <= fullText.Length) {
                text = fullText.Substring(0, TextLength);
            } else {
                text = string.Create(TextLength, fullText, (chars, fullText) =>
                {
                    for (int i = 0; i < chars.Length; i++) {
                        chars[i] = fullText[i % fullText.Length];
                    }
                });
            }
        }

        static void LoadTable()
        {
            codeToUnicode = new Dictionary<int, int>();
            unicodeToCode = new Dictionary<int, int>();

            using var stream = DataStreamFactory.FromFile("index-jis0208.txt", FileOpenMode.Read);
            var reader = new Yarhl.IO.TextReader(stream);

            while (!stream.EndOfStream) {
                if (reader.Peek() == '#') {
                    reader.ReadLine();
                    continue;
                } else if (reader.PeekLine() == string.Empty) {
                    reader.ReadLine();
                    continue;
                }

                string codeText = reader.ReadToToken("\t").TrimStart(' '); // index
                int code = int.Parse(codeText);

                string unicodeHex = reader.ReadToToken("\t").Substring(2);
                int unicode = int.Parse(unicodeHex, NumberStyles.HexNumber);

                codeToUnicode[code] = unicode;
                unicodeToCode[unicode] = code;
                reader.ReadLine();
            }
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
            EncodingDotNet encoding = new EncodingDotNet(codeToUnicode, unicodeToCode);
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        [Benchmark]
        public string SjisCustomFromYarhlEncoding()
        {
            EncodingYarhl encoding = new EncodingYarhl(codeToUnicode, unicodeToCode);
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        [Benchmark]
        public string SjisCustomFromYarhlEncoding2()
        {
            EncodingYarhl2 encoding = new EncodingYarhl2(codeToUnicode, unicodeToCode);
            byte[] encoded = encoding.GetBytes(text);
            return encoding.GetString(encoded);
        }

        private sealed class EncodingDotNet : Encoding
        {
            readonly Dictionary<int, int> codeToUnicode;
            readonly Dictionary<int, int> unicodeToCode;

            public EncodingDotNet(Dictionary<int, int> codeToUnicode, Dictionary<int, int> unicodeToCode)
            {
                this.codeToUnicode = codeToUnicode;
                this.unicodeToCode = unicodeToCode;
            }

            public override int GetByteCount(char[] chars, int index, int count)
            {
                int bytes = 0;
                for (int i = 0; i < count; i++) {
                    ushort codePoint = chars[index + i];

                    if (codePoint == 0x00A5) {
                        bytes++;
                    } else if (codePoint == 0x203E) {
                        bytes++;
                    } else if (codePoint < 0x80) {
                        bytes++;
                    } else if (0xFF61 <= codePoint && codePoint <= 0xFF9F) {
                        bytes++;
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.ContainsKey(codePoint)) {
                            throw new FormatException();
                        }

                        bytes += 2;
                    }
                }

                return bytes;
            }

            public override int GetCharCount(byte[] bytes, int index, int count)
            {
                int chars = 0;
                byte lead = 0x00;
                for (int i = 0; i < count; i++) {
                    byte current = bytes[index + i];

                    if (lead != 0x00) {
                        int offset = (current < 0x7F) ? 0x40 : 0x41;
                        int leadOffset = (lead < 0xA0) ? 0x81 : 0xC1;

                        bool inRange1 = 0x40 <= current && current <= 0x7E;
                        bool inRange2 = 0x80 <= current && current <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            throw new FormatException();
                        }

                        int pointer = (lead - leadOffset) * 188 + current - offset;
                        if (8836 <= pointer && pointer <= 10715) {
                            chars++;
                        } else {
                            if (!codeToUnicode.ContainsKey(pointer)) {
                                throw new FormatException();
                            }

                            chars++;
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        chars++; // yen
                    } else if (current == 0x7E) {
                        chars++; // overline
                    } else if (current < 0x80) {
                        chars++;
                    } else if (0xA1 <= current && current <= 0xDF) {
                        chars++;
                    } else if ((0x81 <= current && current <= 0x9F) ||
                            (0xE0 <= current && current <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }
                }

                // 1.
                if (lead != 0x00) {
                    throw new FormatException();
                }

                return chars;
            }

            public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
            {
                int startIndex = byteIndex;
                for (int i = 0; i < charCount; i++) {
                    ushort codePoint = chars[charIndex + i];

                    if (codePoint == 0x00A5) {
                        bytes[byteIndex++] = 0x5C;
                    } else if (codePoint == 0x203E) {
                        bytes[byteIndex++] = 0x7E;
                    } else if (codePoint < 0x80) {
                        bytes[byteIndex++] = (byte)codePoint;
                    } else if (0xFF61 <= codePoint && codePoint <= 0xFF9F) {
                        bytes[byteIndex++] = (byte)(codePoint - 0xFF61 + 0xA1);
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.TryGetValue(codePoint, out int code)) {
                            throw new FormatException();
                        }

                        int lead = code / 188;
                        int leadOffset = (lead < 0x1F) ? 0x81 : 0xC1;
                        int trail = code % 188;
                        int offset = (trail < 0x3F) ? 0x40 : 0x41;
                        bytes[byteIndex++] = (byte)(lead + leadOffset);
                        bytes[byteIndex++] = (byte)(trail + offset);
                    }
                }

                return byteIndex - startIndex;
            }

            public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
            {
                int startIndex = charIndex;

                byte lead = 0x00;
                for (int i = 0; i < byteCount; i++) {
                    int codePoint = -1;
                    byte current = bytes[byteIndex + i];

                    if (lead != 0x00) {
                        int offset = (current < 0x7F) ? 0x40 : 0x41;
                        int leadOffset = (lead < 0xA0) ? 0x81 : 0xC1;

                        bool inRange1 = 0x40 <= current && current <= 0x7E;
                        bool inRange2 = 0x80 <= current && current <= 0xFC;
                        int pointer = (inRange1 || inRange2)
                            ? (lead - leadOffset) * 188 + current - offset
                            : -1;

                        if (8836 <= pointer && pointer <= 10715) {
                            codePoint = 0xE000 - 8836 + pointer;
                        } else {
                            if (!codeToUnicode.TryGetValue(pointer, out codePoint)) {
                                throw new FormatException();
                            }
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        codePoint = 0x00A5; // yen
                    } else if (current == 0x7E) {
                        codePoint = 0x203E; // overline
                    } else if (current < 0x80) {
                        codePoint = current;
                    } else if (0xA1 <= current && current <= 0xDF) {
                        codePoint = 0xFF61 - 0xA1 + current;
                    } else if ((0x81 <= current && current <= 0x9F) ||
                            (0xE0 <= current && current <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }

                    if (codePoint != -1) {
                        chars[charIndex++] = (char)codePoint;
                    }
                }

                // 1.
                if (lead != 0x00) {
                    throw new FormatException();
                }

                // 2.
                return charIndex - startIndex;
            }

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;
        }

        private sealed class EncodingYarhl : PropietarySpanEncoding
        {
            readonly Dictionary<int, int> codeToUnicode;
            readonly Dictionary<int, int> unicodeToCode;

            public EncodingYarhl(Dictionary<int, int> codeToUnicode, Dictionary<int, int> unicodeToCode)
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
                this.codeToUnicode = codeToUnicode;
                this.unicodeToCode = unicodeToCode;
            }

            public override string EncodingName => "ascii-yarhl";

            public override int GetByteCount(ReadOnlySpan<char> chars)
            {
                int bytes = 0;
                int count = chars.Length;
                for (int i = 0; i < count; i++) {
                    ushort codePoint = chars[i];

                    if (codePoint == 0x00A5) {
                        bytes++;
                    } else if (codePoint == 0x203E) {
                        bytes++;
                    } else if (codePoint < 0x80) {
                        bytes++;
                    } else if (0xFF61 <= codePoint && codePoint <= 0xFF9F) {
                        bytes++;
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.ContainsKey(codePoint)) {
                            throw new FormatException();
                        }

                        bytes += 2;
                    }
                }

                return bytes;
            }

            public override int GetCharCount(ReadOnlySpan<byte> bytes)
            {
                int chars = 0;
                byte lead = 0x00;
                int count = bytes.Length;
                for (int i = 0; i < count; i++) {
                    byte current = bytes[i];

                    if (lead != 0x00) {
                        int offset = (current < 0x7F) ? 0x40 : 0x41;
                        int leadOffset = (lead < 0xA0) ? 0x81 : 0xC1;

                        bool inRange1 = 0x40 <= current && current <= 0x7E;
                        bool inRange2 = 0x80 <= current && current <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            throw new FormatException();
                        }

                        int pointer = (lead - leadOffset) * 188 + current - offset;
                        if (8836 <= pointer && pointer <= 10715) {
                            chars++;
                        } else {
                            if (!codeToUnicode.ContainsKey(pointer)) {
                                throw new FormatException();
                            }

                            chars++;
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        chars++; // yen
                    } else if (current == 0x7E) {
                        chars++; // overline
                    } else if (current < 0x80) {
                        chars++;
                    } else if (0xA1 <= current && current <= 0xDF) {
                        chars++;
                    } else if ((0x81 <= current && current <= 0x9F) ||
                            (0xE0 <= current && current <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }
                }

                // 1.
                if (lead != 0x00) {
                    throw new FormatException();
                }

                return chars;
            }

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;

            public override int GetChars(ReadOnlySpan<byte> buffer, Span<char> text)
            {
                int pos = 0;
                byte lead = 0x00;
                int count = buffer.Length;
                for (int i = 0; i < count; i++) {
                    int codePoint = -1;
                    byte current = buffer[i];

                    if (lead != 0x00) {
                        int offset = (current < 0x7F) ? 0x40 : 0x41;
                        int leadOffset = (lead < 0xA0) ? 0x81 : 0xC1;

                        bool inRange1 = 0x40 <= current && current <= 0x7E;
                        bool inRange2 = 0x80 <= current && current <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            throw new FormatException();
                        }

                        int pointer = (lead - leadOffset) * 188 + current - offset;
                        if (8836 <= pointer && pointer <= 10715) {
                            codePoint = 0xE000 - 8836 + pointer;
                        } else {
                            if (!codeToUnicode.TryGetValue(pointer, out codePoint)) {
                                throw new FormatException();
                            }
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        codePoint = 0x00A5; // yen
                    } else if (current == 0x7E) {
                        codePoint = 0x203E; // overline
                    } else if (current < 0x80) {
                        codePoint = current;
                    } else if (0xA1 <= current && current <= 0xDF) {
                        codePoint = 0xFF61 - 0xA1 + current;
                    } else if ((0x81 <= current && current <= 0x9F) ||
                            (0xE0 <= current && current <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }

                    if (codePoint != -1) {
                        text[pos++] = (char)codePoint;
                    }
                }

                // 1.
                if (lead != 0x00) {
                    throw new FormatException();
                }

                // 2.
                return pos;
            }

            public override int GetBytes(ReadOnlySpan<char> text, Span<byte> buffer)
            {
                int pos = 0;
                int count = text.Length;
                for (int i = 0; i < count; i++) {
                    ushort codePoint = text[i];

                    if (codePoint == 0x00A5) {
                        buffer[pos++] = 0x5C;
                    } else if (codePoint == 0x203E) {
                        buffer[pos++] = 0x7E;
                    } else if (codePoint < 0x80) {
                        buffer[pos++] = (byte)codePoint;
                    } else if (0xFF61 <= codePoint && codePoint <= 0xFF9F) {
                        buffer[pos++] = (byte)(codePoint - 0xFF61 + 0xA1);
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.TryGetValue(codePoint, out int code)) {
                            throw new FormatException();
                        }

                        int lead = code / 188;
                        int leadOffset = (lead < 0x1F) ? 0x81 : 0xC1;
                        int trail = code % 188;
                        int offset = (trail < 0x3F) ? 0x40 : 0x41;
                        buffer[pos++] = (byte)(lead + leadOffset);
                        buffer[pos++] = (byte)(trail + offset);
                    }
                }

                return pos;
            }
        }

        private sealed class EncodingYarhl2 : PropietarySpanEncodingWithoutCount
        {
            readonly Dictionary<int, int> codeToUnicode;
            readonly Dictionary<int, int> unicodeToCode;

            public EncodingYarhl2(Dictionary<int, int> codeToUnicode, Dictionary<int, int> unicodeToCode)
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
                this.codeToUnicode = codeToUnicode;
                this.unicodeToCode = unicodeToCode;
            }

            public override string EncodingName => "ascii-yarhl2";

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;

            protected override void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer)
            {
                byte lead = 0x00;
                int count = bytes.Length;
                for (int i = 0; i < count; i++) {
                    int codePoint = -1;
                    byte current = bytes[i];

                    if (lead != 0x00) {
                        int offset = (current < 0x7F) ? 0x40 : 0x41;
                        int leadOffset = (lead < 0xA0) ? 0x81 : 0xC1;

                        bool inRange1 = 0x40 <= current && current <= 0x7E;
                        bool inRange2 = 0x80 <= current && current <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            throw new FormatException();
                        }

                        int pointer = (lead - leadOffset) * 188 + current - offset;
                        if (8836 <= pointer && pointer <= 10715) {
                            codePoint = 0xE000 - 8836 + pointer;
                        } else {
                            if (!codeToUnicode.TryGetValue(pointer, out codePoint)) {
                                throw new FormatException();
                            }
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        codePoint = 0x00A5; // yen
                    } else if (current == 0x7E) {
                        codePoint = 0x203E; // overline
                    } else if (current < 0x80) {
                        codePoint = current;
                    } else if (0xA1 <= current && current <= 0xDF) {
                        codePoint = 0xFF61 - 0xA1 + current;
                    } else if ((0x81 <= current && current <= 0x9F) ||
                            (0xE0 <= current && current <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }

                    if (codePoint != -1) {
                        buffer.Write((char)codePoint);
                    }
                }

                // 1.
                if (lead != 0x00) {
                    throw new FormatException();
                }
            }

            protected override void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer)
            {
                int count = chars.Length;
                for (int i = 0; i < count; i++) {
                    ushort codePoint = chars[i];

                    if (codePoint == 0x00A5) {
                        buffer.Write(0x5C);
                    } else if (codePoint == 0x203E) {
                        buffer.Write(0x7E);
                    } else if (codePoint < 0x80) {
                        buffer.Write((byte)codePoint);
                    } else if (0xFF61 <= codePoint && codePoint <= 0xFF9F) {
                        buffer.Write((byte)(codePoint - 0xFF61 + 0xA1));
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.TryGetValue(codePoint, out int code)) {
                            throw new FormatException();
                        }

                        int lead = code / 188;
                        int leadOffset = (lead < 0x1F) ? 0x81 : 0xC1;
                        int trail = code % 188;
                        int offset = (trail < 0x3F) ? 0x40 : 0x41;
                        buffer.Write((byte)(lead + leadOffset));
                        buffer.Write((byte)(trail + offset));
                    }
                }
            }
        }
    }
}
