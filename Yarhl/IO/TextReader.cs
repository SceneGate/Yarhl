//
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
    using System.Text;

    /// <summary>
    /// Text reader for DataStreams.
    /// </summary>
    public class TextReader
    {
        readonly DataReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <remarks>The default encoding is UTF-8.</remarks>
        public TextReader(DataStream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReader"/> class.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="encoding">Encoding to use.</param>
        public TextReader(DataStream stream, Encoding encoding)
        {
            reader = new DataReader(stream);
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            NewLine = Environment.NewLine;
            AutoNewLine = true;
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <value>The stream.</value>
        public DataStream Stream {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the new line.
        /// </summary>
        /// <value>The new line.</value>
        /// <remarks>The default value is OS-dependant.</remarks>
        public string NewLine {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether reads any kind of
        /// NewLine format (\r\n or \n). If true, this ignore the
        /// <cref href="NewLine" /> field.
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
            return reader.ReadChar(Encoding);
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

            return reader.ReadChars(count, Encoding);
        }

        /// <summary>
        /// Reads a string until a string / token is found.
        /// </summary>
        /// <returns>The read string.</returns>
        /// <param name="token">Token to find.</param>
        public string ReadToToken(string token)
        {
            // If starting is EOF, then return null
            if (Stream.EndOfStream)
                return null;

            string text = string.Empty;
            bool found = false;
            while (!found) {
                if (Stream.EndOfStream)
                    break;

                text += reader.ReadChar(Encoding);
                found = text.EndsWith(token, StringComparison.InvariantCulture);
            }

            return found ? text.Substring(0, text.Length - token.Length) : text;
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
            return reader.ReadString(
                (int)(Stream.Length - Stream.Position),
                Encoding);
        }

        /// <summary>
        /// Read the next char without changing the position.
        /// </summary>
        /// <returns>The next char.</returns>
        public char Peek()
        {
            Stream.PushCurrentPosition();
            char ch = Read();
            Stream.PopPosition();
            return ch;
        }

        /// <summary>
        /// Read the next count of chars without changing the position.
        /// </summary>
        /// <returns>The next chars.</returns>
        /// <param name="count">Number of chars to read.</param>
        public char[] Peek(int count)
        {
            Stream.PushCurrentPosition();
            char[] chars = Read(count);
            Stream.PopPosition();
            return chars;
        }

        /// <summary>
        /// Read until a string / token is found without changing the position.
        /// </summary>
        /// <returns>The next chars.</returns>
        /// <param name="token">Token to find.</param>
        public string PeekToToken(string token)
        {
            Stream.PushCurrentPosition();
            string content = ReadToToken(token);
            Stream.PopPosition();
            return content;
        }

        /// <summary>
        /// Read the next line without changing the position.
        /// </summary>
        /// <returns>The next line.</returns>
        public string PeekLine()
        {
            Stream.PushCurrentPosition();
            string line = ReadLine();
            Stream.PopPosition();
            return line;
        }
    }
}
