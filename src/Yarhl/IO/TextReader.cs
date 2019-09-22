// TextReader.cs
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

namespace Yarhl.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Text reader for <see cref="DataStream" />.
    /// </summary>
    public class TextReader
    {
        readonly DataReader reader;
        string newLine;

        static TextReader()
        {
            // Make sure that the shift-jis encoding is initialized in
            // .NET Core.
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <remarks><para>The default encoding is UTF-8.</para></remarks>
        public TextReader(DataStream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="encoding">Encoding to use.</param>
        public TextReader(DataStream stream, string encoding)
            : this(stream, Encoding.GetEncoding(encoding))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="encoding">Encoding to use.</param>
        public TextReader(DataStream stream, Encoding encoding)
        {
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            NewLine = Environment.NewLine;
            AutoNewLine = true;

            reader = new DataReader(stream) {
                DefaultEncoding = Encoding,
            };
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        public DataStream Stream {
            get;
            private set;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        public Encoding Encoding {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the new line and set to false AutoNewLine.
        /// </summary>
        /// <value>The new line.</value>
        /// <remarks><para>The default value is OS-dependant.</para></remarks>
        public string NewLine {
            get {
                return newLine;
            }

            set {
                newLine = value;
                AutoNewLine = false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether reads any kind of
        /// NewLine format (\r\n or \n). If true, this ignore the
        /// NewLine field.
        /// </summary>
        /// <returns>If true, it will treat new lines any matching of \r\n or
        /// \n. Otherwhise false.
        /// </returns>
        public bool AutoNewLine {
            get;
            set;
        }

        /// <summary>
        /// Read a char from the stream.
        /// </summary>
        /// <returns>The read char.</returns>
        public char Read()
        {
            SkipPreamble();
            return reader.ReadChar();
        }

        /// <summary>
        /// Read the specified number of chars.
        /// </summary>
        /// <returns>The read chars.</returns>
        /// <param name="count">Chars to read.</param>
        public char[] Read(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            SkipPreamble();
            return reader.ReadChars(count);
        }

        /// <summary>
        /// Reads a string until a string / token is found.
        /// </summary>
        /// <returns>The read string.</returns>
        /// <param name="token">Token to find.</param>
        public string ReadToToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            // If starting is EOF, then return null
            if (Stream.EndOfStream) {
                return null;
            }

            SkipPreamble();

            long startPos = Stream.Position;
            long streamLength = Stream.Length;

            // Gather bytes from buffer to buffer into a list and try to
            // convert to find the token. This approach is faster since we
            // read blocks and it avoids issues with half-encoded chars.
            const int BufferSize = 128;
            byte[] buffer = new byte[BufferSize];

            List<byte> textBuffer = new List<byte>();
            string text = string.Empty;
            int matchIndex = -1;

            while (matchIndex == -1) {
                if (Stream.EndOfStream) {
                    break;
                }

                // Read buffer size if possible, otherwise remaining bytes
                long currentPosition = Stream.Position;
                int size = currentPosition + BufferSize <= streamLength ?
                    BufferSize :
                    (int)(streamLength - currentPosition);

                int read = Stream.Read(buffer, 0, size);
                textBuffer.AddRange(buffer.Take(read));

                text = Encoding.GetString(textBuffer.ToArray());
                matchIndex = text.IndexOf(token, StringComparison.Ordinal);
            }

            if (matchIndex != -1) {
                // We skip the bytes of the token too
                string fullResult = text.Substring(0, matchIndex + token.Length);
                Stream.Seek(startPos + Encoding.GetByteCount(fullResult), SeekMode.Start);

                // Result without token
                text = text.Substring(0, matchIndex);
            }

            return text;
        }

        /// <summary>
        /// Reads a line.
        /// </summary>
        /// <returns>The line.</returns>
        public string ReadLine()
        {
            string line;

            // We search for \n new lines.
            if (AutoNewLine) {
                line = ReadToToken("\n");

                // In the case of Windows, the last char will be \r. We remove it.
                if (!string.IsNullOrEmpty(line) && line[line.Length - 1] == '\r')
                    line = line.Remove(line.Length - 1, 1);
            } else {
                line = ReadToToken(NewLine);
            }

            return line;
        }

        /// <summary>
        /// Reads to the end.
        /// </summary>
        /// <returns>The string.</returns>
        public string ReadToEnd()
        {
            SkipPreamble();
            return reader.ReadString((int)(Stream.Length - Stream.Position));
        }

        /// <summary>
        /// Read the next char without changing the position.
        /// </summary>
        /// <returns>The next char.</returns>
        public char Peek()
        {
            long startPos = Stream.Position;
            char ch = Read();
            Stream.Seek(startPos, SeekMode.Start);
            return ch;
        }

        /// <summary>
        /// Read the next count of chars without changing the position.
        /// </summary>
        /// <returns>The next chars.</returns>
        /// <param name="count">Number of chars to read.</param>
        public char[] Peek(int count)
        {
            long startPos = Stream.Position;
            char[] chars = Read(count);
            Stream.Seek(startPos, SeekMode.Start);
            return chars;
        }

        /// <summary>
        /// Read until a string / token is found without changing the position.
        /// </summary>
        /// <returns>The next chars.</returns>
        /// <param name="token">Token to find.</param>
        public string PeekToToken(string token)
        {
            long startPos = Stream.Position;
            string content = ReadToToken(token);
            Stream.Seek(startPos, SeekMode.Start);
            return content;
        }

        /// <summary>
        /// Read the next line without changing the position.
        /// </summary>
        /// <returns>The next line.</returns>
        public string PeekLine()
        {
            long startPos = Stream.Position;
            string line = ReadLine();
            Stream.Seek(startPos, SeekMode.Start);
            return line;
        }

        void SkipPreamble()
        {
            // Preambles can only be at the beginning of the stream.
            if (Stream.Position > 0) {
                return;
            }

            byte[] preamble = Encoding.GetPreamble();
            if (Stream.Length < preamble.Length) {
                return;
            }

            bool match = true;
            for (int i = 0; i < preamble.Length && match; i++) {
                match = preamble[i] == reader.ReadByte();
            }

            // If it didn't fully match it wasn't a preamble, returns to 0
            if (!match) {
                Stream.Seek(0, SeekMode.Start);
            }
        }
    }
}
