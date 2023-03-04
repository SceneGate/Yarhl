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
namespace Yarhl.PerformanceTests.Encodings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        private static Dictionary<int, int> baseCodeToUnicode;
        private static Dictionary<int, int> baseUnicodeToCode;
        private Encoding encoding;
        private string text;
        private byte[] encodedText;

        static EncodingSpan()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            LoadTable();
        }

        [Params(280, 5 * 1024 * 1024)]
        public int TextLength { get; set; }

        [ParamsAllValues]
        public EncodingKind Kind { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            string fullText = File.ReadAllText(Path.Combine("Encodings", "japanese.txt"));
            if (TextLength <= fullText.Length) {
                text = fullText[..TextLength];
            } else {
                text = string.Create(
                    TextLength,
                    fullText,
                    (chars, ctxtText) =>
                    {
                        for (int i = 0; i < chars.Length; i++) {
                            chars[i] = ctxtText[i % ctxtText.Length];
                        }
                    });
            }

            encodedText = Encoding.GetEncoding("shift-jis").GetBytes(text);

            encoding = Kind switch {
                EncodingKind.DotNet => Encoding.GetEncoding("shift-jis"),
                EncodingKind.Manual => new CustomSjisEncoding(baseCodeToUnicode, baseUnicodeToCode),
                EncodingKind.Yarhl => new CustomSjisYarhlEncoding(baseCodeToUnicode, baseUnicodeToCode),
                _ => throw new NotImplementedException(),
            };
        }

        [Benchmark]
        public int CountChars() => encoding.GetCharCount(encodedText);

        [Benchmark]
        public int CountBytes() => encoding.GetByteCount(text);

        [Benchmark]
        public int Encode()
        {
            byte[] buffer = new byte[encodedText.Length];
            return encoding.GetBytes(text, 0, text.Length, buffer, 0);
        }

        [Benchmark]
        public int Decode()
        {
            char[] buffer = new char[text.Length];
            return encoding.GetChars(encodedText, 0, encodedText.Length, buffer, 0);
        }

        [Benchmark]
        public string DecodeString() => encoding.GetString(encodedText);

        private static void LoadTable()
        {
            baseCodeToUnicode = new Dictionary<int, int>();
            baseUnicodeToCode = new Dictionary<int, int>();

            using var stream = DataStreamFactory.FromFile(Path.Combine("Encodings", "index-jis0208.txt"), FileOpenMode.Read);
            var reader = new TextDataReader(stream);

            while (!stream.EndOfStream) {
                if (reader.Peek() == '#' || reader.PeekLine().Length == 0) {
                    _ = reader.ReadLine();
                    continue;
                }

                string codeText = reader.ReadToToken("\t").TrimStart(' '); // index
                int code = int.Parse(codeText);

                string unicodeHex = reader.ReadToToken("\t")[2..];
                int unicode = int.Parse(unicodeHex, NumberStyles.HexNumber);

                baseCodeToUnicode[code] = unicode;
                baseUnicodeToCode[unicode] = code;
                _ = reader.ReadLine();
            }
        }

        private sealed class CustomSjisEncoding : Encoding
        {
            private readonly Dictionary<int, int> codeToUnicode;
            private readonly Dictionary<int, int> unicodeToCode;

            public CustomSjisEncoding(Dictionary<int, int> codeToUnicode, Dictionary<int, int> unicodeToCode)
            {
                this.codeToUnicode = codeToUnicode;
                this.unicodeToCode = unicodeToCode;
            }

            public override int GetMaxByteCount(int charCount) => charCount * 2;

            public override int GetMaxCharCount(int byteCount) => byteCount;

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
                    } else if (codePoint is >= 0xFF61 and <= 0xFF9F) {
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

                        bool inRange1 = current is >= 0x40 and <= 0x7E;
                        bool inRange2 = current is >= 0x80 and <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            throw new FormatException();
                        }

                        int pointer = ((lead - leadOffset) * 188) + current - offset;
                        if (pointer is >= 8836 and <= 10715) {
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
                    } else if (current is >= 0xA1 and <= 0xDF) {
                        chars++;
                    } else if (current is(>= 0x81 and <= 0x9F) or(>= 0xE0 and <= 0xFC)) {
                        lead = current;
                    } else {
                        throw new FormatException();
                    }
                }

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
                    } else if (codePoint is >= 0xFF61 and <= 0xFF9F) {
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

                        bool inRange1 = current is >= 0x40 and <= 0x7E;
                        bool inRange2 = current is >= 0x80 and <= 0xFC;
                        int pointer = (inRange1 || inRange2)
                            ? ((lead - leadOffset) * 188) + current - offset
                            : -1;

                        if (pointer is >= 8836 and <= 10715) {
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
                    } else if (current is >= 0xA1 and <= 0xDF) {
                        codePoint = 0xFF61 - 0xA1 + current;
                    } else if (current is(>= 0x81 and <= 0x9F) or(>= 0xE0 and <= 0xFC)) {
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
        }

        private sealed class CustomSjisYarhlEncoding : SimpleSpanEncoding
        {
            private readonly Dictionary<int, int> codeToUnicode;
            private readonly Dictionary<int, int> unicodeToCode;

            public CustomSjisYarhlEncoding(Dictionary<int, int> codeToUnicode, Dictionary<int, int> unicodeToCode)
                : base(0, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback)
            {
                this.codeToUnicode = codeToUnicode;
                this.unicodeToCode = unicodeToCode;
            }

            public override string EncodingName => "sjis-yarhl";

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

                        bool inRange1 = current is >= 0x40 and <= 0x7E;
                        bool inRange2 = current is >= 0x80 and <= 0xFC;
                        if (!inRange1 && !inRange2) {
                            DecodeUnknownBytes(buffer, i, current);
                        }

                        int pointer = ((lead - leadOffset) * 188) + current - offset;
                        if (pointer is 8836 and <= 10715) {
                            codePoint = 0xE000 - 8836 + pointer;
                        } else {
                            if (!codeToUnicode.TryGetValue(pointer, out codePoint)) {
                                DecodeUnknownBytes(buffer, i, current);
                            }
                        }

                        lead = 0x00;
                    } else if (current == 0x5C) {
                        codePoint = 0x00A5; // yen
                    } else if (current == 0x7E) {
                        codePoint = 0x203E; // overline
                    } else if (current < 0x80) {
                        codePoint = current;
                    } else if (current is >= 0xA1 and <= 0xDF) {
                        codePoint = 0xFF61 - 0xA1 + current;
                    } else if (current is(>= 0x81 and <= 0x9F) or(>= 0xE0 and <= 0xFC)) {
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
                    DecodeUnknownBytes(buffer, count - 2, lead);
                }
            }

            protected override void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer, bool isFallbackText = false)
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
                    } else if (codePoint is >= 0xFF61 and <= 0xFF9F) {
                        buffer.Write((byte)(codePoint - 0xFF61 + 0xA1));
                    } else {
                        if (codePoint == 0x2212) {
                            codePoint = 0xFF0D;
                        }

                        if (!unicodeToCode.TryGetValue(codePoint, out int code)) {
                            EncodeUnknownChar(buffer, codePoint, i, isFallbackText);
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

        [SuppressMessage("", "SA1201", Justification = "Internal test enumeration")]
        public enum EncodingKind
        {
            DotNet,
            Manual,
            Yarhl,
        }
    }
}
