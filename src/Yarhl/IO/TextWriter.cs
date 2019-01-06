// TextWriter.cs
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
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Text writer for <cref href="DataStream" />.
    /// </summary>
    public class TextWriter
    {
        readonly DataWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriter"/> class.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <remarks>The default encoding is UTF-8.</remarks>
        public TextWriter(DataStream stream)
            : this(stream, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextWriter"/> class.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="encoding">Encoding to use.</param>
        public TextWriter(DataStream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (encoding == null)
                throw new ArgumentNullException(nameof(encoding));

            Stream = stream;
            Encoding = encoding;
            NewLine = "\n";
            AutoPreamble = false;
            writer = new DataWriter(stream) {
                DefaultEncoding = Encoding,
            };
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
        /// Gets the encoding.
        /// </summary>
        /// <value>The encoding.</value>
        public Encoding Encoding {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the new line character.
        /// </summary>
        /// <value>The new line character.</value>
        /// <remarks>The default value is \n, the Unix new line.</remarks>
        public string NewLine {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether write automatically
        /// the encoding preamble.
        /// </summary>
        /// <value>
        /// True to write the preamble if the stream is empty, otherwise false.
        /// </value>
        public bool AutoPreamble {
            get;
            set;
        }

        /// <summary>
        /// Write the specified char.
        /// </summary>
        /// <param name="ch">Char to write.</param>
        public void Write(char ch)
        {
            CheckWritePreamble();
            writer.Write(ch);
        }

        /// <summary>
        /// Write the specified chars.
        /// </summary>
        /// <param name="chars">Chars to write.</param>
        public void Write(char[] chars)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));

            CheckWritePreamble();
            writer.Write(chars);
        }

        /// <summary>
        /// Write the specified text without including a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void Write(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            CheckWritePreamble();
            writer.Write(text, false);
        }

        /// <summary>
        /// Write the specified text with format.
        /// </summary>
        /// <param name="format">Format for the text.</param>
        /// <param name="args">Arguments for the format.</param>
        public void Write(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            CheckWritePreamble();
            string text = string.Format(CultureInfo.InvariantCulture, format, args);
            writer.Write(text, false);
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public void WriteLine()
        {
            CheckWritePreamble();
            writer.Write(NewLine, false);
        }

        /// <summary>
        /// Writes the specified text and add a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void WriteLine(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            CheckWritePreamble();
            writer.Write(text + NewLine, false);
        }

        /// <summary>
        /// Writes the specified text with format.
        /// </summary>
        /// <param name="format">Format for the text.</param>
        /// <param name="args">Arguments of the format.</param>
        public void WriteLine(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            CheckWritePreamble();
            string text = string.Format(CultureInfo.InvariantCulture, format, args);
            writer.Write(text + NewLine, false);
        }

        /// <summary>
        /// Write the encoding preamble.
        /// </summary>
        public void WritePreamble()
        {
            if (Stream.Position > 0) {
                throw new InvalidOperationException(
                    "Preamble can be written only in Position 0.");
            }

            writer.Write(Encoding.GetPreamble());
        }

        void CheckWritePreamble()
        {
            if (AutoPreamble && Stream.Position == 0) {
                WritePreamble();
            }
        }
    }
}
