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

namespace Libgame.UnitTests.IO
{
    using System;
    using System.Text;
    using Libgame.IO;
    using NUnit.Framework;

    [TestFixture]
    public class TextWriterTests
    {
        DataStream stream;
        TextWriter writer;

        [SetUp]
        public void SetUp()
        {
            stream = new DataStream();
            writer = new TextWriter(stream);
        }

        [TearDown]
        public void TearDown()
        {
            stream.Dispose();
        }

        [Test]
        public void DefaultPropertyValues()
        {
            Assert.AreSame(stream, writer.Stream);
            Assert.AreSame(Encoding.UTF8, writer.Encoding);
            Assert.AreEqual(Environment.NewLine, writer.NewLine);
        }

        [Test]
        public void ConstructorWithEncoding()
        {
            writer = new TextWriter(stream, Encoding.ASCII);
            Assert.AreSame(Encoding.ASCII, writer.Encoding);
        }

        [Test]
        public void WrongArgsInConstructorThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => new TextWriter(null));
            Assert.Throws<ArgumentNullException>(
                () => new TextWriter(null, Encoding.ASCII));
            Assert.Throws<ArgumentNullException>(
                () => new TextWriter(stream, null));
        }

        [Test]
        public void PropertiesChangeValues()
        {
            writer.Encoding = Encoding.ASCII;
            Assert.AreSame(Encoding.ASCII, writer.Encoding);

            writer.NewLine = "a";
            Assert.AreEqual("a", writer.NewLine);
        }

        [Test]
        public void WriteChar()
        {
            writer.Write('c');

            stream.Position = 0;
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual('c', stream.ReadByte());
        }

        [Test]
        public void WriteCharWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.Write('c');

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('c', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteCharArray()
        {
            writer.Write(new char[] { 'z', 'w' });

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('w', stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.Write(new char[] { 'z', 'w' });

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('w', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullCharArrayThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => writer.Write((char[])null));
        }

        [Test]
        public void WriteString()
        {
            writer.Write("he");

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('h', stream.ReadByte());
            Assert.AreEqual('e', stream.ReadByte());
        }

        [Test]
        public void WriteStringWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.Write("he");

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('h', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('e', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullStringThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => writer.Write((string)null));
        }

        [Test]
        public void WriteStringFormat()
        {
            writer.Write("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
        }

        [Test]
        public void WriteStringFormatEscaping()
        {
            writer.Write("a{{0}}", 3);

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('{', stream.ReadByte());
            Assert.AreEqual('0', stream.ReadByte());
            Assert.AreEqual('}', stream.ReadByte());
        }

        [Test]
        public void WriteStringFormatWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.Write("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullStringFormatThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => writer.Write(null, "a"));
            Assert.Throws<ArgumentNullException>(() => writer.Write("a", null));
        }

        [Test]
        public void WriteStringFormatWithLessFormatsThrows()
        {
            Assert.Throws<FormatException>(() => writer.Write("a{0}{1}", 3));
            Assert.Throws<FormatException>(() => writer.Write("a{1}", 3));
            Assert.Throws<FormatException>(() => writer.Write("a{0}", new object[0]));
        }

        [Test]
        public void WriteStringFormatWithMoreArgsDoesNotThrow()
        {
            Assert.DoesNotThrow(() => writer.Write("a{0}", 3, 4, 5));
        }

        [Test]
        public void WriteLineOnly()
        {
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual('\n', stream.ReadByte());
        }

        [Test]
        public void WriteLineOnlyWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineOnlyWithNewLine()
        {
            writer.NewLine = "NL";
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('N', stream.ReadByte());
            Assert.AreEqual('L', stream.ReadByte());
        }

        [Test]
        public void WriteLineString()
        {
            writer.WriteLine("za");

            stream.Position = 0;
            Assert.AreEqual(3, stream.Length);
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
        }

        [Test]
        public void WriteLineStringWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.WriteLine("a");

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineNullStringThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine(null));
        }

        [Test]
        public void WriteLineStringFormat()
        {
            writer.WriteLine("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(3, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
        }

        [Test]
        public void WriteLineStringFormatEscaping()
        {
            writer.WriteLine("a{{0}}", 3);

            stream.Position = 0;
            Assert.AreEqual(5, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('{', stream.ReadByte());
            Assert.AreEqual('0', stream.ReadByte());
            Assert.AreEqual('}', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
        }

        [Test]
        public void WriteLineStringFormatWithEncoding()
        {
            writer.Encoding = Encoding.GetEncoding("utf-16");
            writer.WriteLine("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(6, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineNullStringFormatThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine(null, "a"));
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine("a", null));
        }

        [Test]
        public void WriteLineStringFormatWithLessFormatsThrows()
        {
            Assert.Throws<FormatException>(() => writer.WriteLine("a{0}{1}", 3));
            Assert.Throws<FormatException>(() => writer.WriteLine("a{1}", 3));
            Assert.Throws<FormatException>(() => writer.WriteLine("a{0}", new object[0]));
        }

        [Test]
        public void WriteLineStringFormatWithMoreArgsDoesNotThrow()
        {
            Assert.DoesNotThrow(() => writer.WriteLine("a{0}", 3, 4, 5));
        }
    }
}
