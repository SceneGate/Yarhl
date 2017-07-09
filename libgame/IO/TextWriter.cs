//
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

namespace Libgame.IO
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Text writer for DataStreams.
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
            NewLine = Environment.NewLine;
            writer = new DataWriter(stream);
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
        /// Gets or sets the new line character.
        /// </summary>
        /// <value>The new line character.</value>
        /// <remarks>The default value is OS-dependant.</remarks>
        public string NewLine {
            get;
            set;
        }

        /// <summary>
        /// Write the specified char.
        /// </summary>
        /// <param name="ch">Char to write.</param>
        public void Write(char ch)
        {
            writer.Write(ch, Encoding);
        }

        /// <summary>
        /// Write the specified chars.
        /// </summary>
        /// <param name="chars">Chars to write.</param>
        public void Write(char[] chars)
        {
            if (chars == null)
                throw new ArgumentNullException(nameof(chars));

            writer.Write(chars, Encoding);
        }

        /// <summary>
        /// Write the specified text without including a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void Write(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            writer.Write(text, false, Encoding);
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

            string text = string.Format(CultureInfo.InvariantCulture, format, args);
            writer.Write(text, false, Encoding);
        }

        /// <summary>
        /// Writes a new line.
        /// </summary>
        public void WriteLine()
        {
            writer.Write(NewLine, false, Encoding);
        }

        /// <summary>
        /// Writes the specified text and add a new line.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void WriteLine(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            writer.Write(text + NewLine, false, Encoding);
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

            string text = string.Format(CultureInfo.InvariantCulture, format, args);
            writer.Write(text + NewLine, false, Encoding);
        }
    }
}
