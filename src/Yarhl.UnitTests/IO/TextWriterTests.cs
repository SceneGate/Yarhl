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

namespace Yarhl.UnitTests.IO
{
    using System;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class TextWriterTests
    {
        DataStream stream;

        [SetUp]
        public void SetUp()
        {
            stream = new DataStream();
        }

        [TearDown]
        public void TearDown()
        {
            stream.Dispose();
        }

        [Test]
        public void DefaultPropertyValues()
        {
            var writer = new TextWriter(stream);
            Assert.AreSame(stream, writer.Stream);
            Assert.AreSame(Encoding.UTF8, writer.Encoding);
            Assert.IsFalse(writer.AutoPreamble);
            Assert.AreEqual("\n", writer.NewLine);
        }

        [Test]
        public void ConstructorWithEncoding()
        {
            var writer = new TextWriter(stream, Encoding.ASCII);
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
            var writer = new TextWriter(stream);
            writer.NewLine = "a";
            Assert.AreEqual("a", writer.NewLine);

            writer.AutoPreamble = true;
            Assert.IsTrue(writer.AutoPreamble);
        }

        [Test]
        public void WritePreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.WritePreamble();
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFF));
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFE));

            writer = new TextWriter(stream, Encoding.BigEndianUnicode);
            stream.Position = 0;
            writer.WritePreamble();
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFF));

            writer = new TextWriter(stream, Encoding.UTF8);
            stream.Position = 0;
            writer.WritePreamble();
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0xEF));
            Assert.That(stream.ReadByte(), Is.EqualTo(0xBB));
            Assert.That(stream.ReadByte(), Is.EqualTo(0xBF));

            writer = new TextWriter(stream, Encoding.UTF32);
            stream.Position = 0;
            writer.WritePreamble();
            stream.Position = 0;
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFF));
            Assert.That(stream.ReadByte(), Is.EqualTo(0xFE));
            Assert.That(stream.ReadByte(), Is.EqualTo(0x00));
            Assert.That(stream.ReadByte(), Is.EqualTo(0x00));
        }

        [Test]
        public void WritePreambleWithEncodingWithoutItDoesNotThrow()
        {
            var writer = new TextWriter(stream, Encoding.ASCII);
            Assert.That(() => writer.WritePreamble(), Throws.Nothing);
            Assert.That(stream.Length, Is.EqualTo(0));
        }

        [Test]
        public void WritePreambleNotStartThrowsException()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            stream.WriteByte(0x00);

            Assert.That(
                () => writer.WritePreamble(),
                Throws.InvalidOperationException.With
                    .Message.EqualTo("Preamble can be written only in position 0."));
        }

        [Test]
        public void WriteChar()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
            writer.Write('c');

            stream.Position = 0;
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual('c', stream.ReadByte());
        }

        [Test]
        public void WriteCharWithEncoding()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.Write('c');

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('c', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteCharWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.Write('c');
            writer.Write('a');

            stream.Position = 0;
            Assert.AreEqual(6, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('c', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteCharArray()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
            writer.Write(new char[] { 'z', 'w' });

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('w', stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayWithEncoding()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.Write(new char[] { 'z', 'w' });

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('w', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteCharArrayWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.Write(new char[] { 'z', 'w' });
            writer.Write(new char[] { 'a' });

            stream.Position = 0;
            Assert.AreEqual(8, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('z', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('w', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullCharArrayThrowsException()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<ArgumentNullException>(() => writer.Write((char[])null));
        }

        [Test]
        public void WriteString()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
            writer.Write("he");

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('h', stream.ReadByte());
            Assert.AreEqual('e', stream.ReadByte());
        }

        [Test]
        public void WriteStringWithEncoding()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.Write("he");

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('h', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('e', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteStringWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.Write("he");
            writer.Write("llo");

            stream.Position = 0;
            Assert.AreEqual(12, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('h', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('e', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('l', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('l', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('o', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullStringThrowsException()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<ArgumentNullException>(() => writer.Write((string)null));
        }

        [Test]
        public void WriteStringFormat()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
            writer.Write("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
        }

        [Test]
        public void WriteStringFormatEscaping()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
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
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.Write("a{0}", 3);

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteStringFormatWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.Write("a{0}", 3);
            writer.Write("b{0}", 0);

            stream.Position = 0;
            Assert.AreEqual(10, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('b', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('0', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteNullStringFormatThrowsException()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<ArgumentNullException>(() => writer.Write(null, "a"));
            Assert.Throws<ArgumentNullException>(() => writer.Write("a", null));
        }

        [Test]
        public void WriteStringFormatWithLessFormatsThrows()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<FormatException>(() => writer.Write("a{0}{1}", 3));
            Assert.Throws<FormatException>(() => writer.Write("a{1}", 3));
            Assert.Throws<FormatException>(() => writer.Write("a{0}", new object[0]));
        }

        [Test]
        public void WriteStringFormatWithMoreArgsDoesNotThrow()
        {
            var writer = new TextWriter(stream);
            Assert.DoesNotThrow(() => writer.Write("a{0}", 3, 4, 5));
        }

        [Test]
        public void WriteLineOnly()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(1, stream.Length);
            Assert.AreEqual('\n', stream.ReadByte());
        }

        [Test]
        public void WriteLineOnlyWithEncoding()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(2, stream.Length);
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineOnlyWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.WriteLine();
            writer.WriteLine();

            stream.Position = 0;
            Assert.AreEqual(6, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineOnlyWithNewLine()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
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
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
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
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
            writer.WriteLine("a");

            stream.Position = 0;
            Assert.AreEqual(4, stream.Length);
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineStringWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.WriteLine("a");
            writer.WriteLine("b");

            stream.Position = 0;
            Assert.AreEqual(10, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('b', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineNullStringThrowsException()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine(null));
        }

        [Test]
        public void WriteLineStringFormat()
        {
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
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
            var writer = new TextWriter(stream);
            writer.AutoPreamble = false;
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
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = false;
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
        public void WriteLineStringFormatWithPreamble()
        {
            var writer = new TextWriter(stream, Encoding.Unicode);
            writer.AutoPreamble = true;
            writer.WriteLine("a{0}", 3);
            writer.WriteLine("b{0}", 0);

            stream.Position = 0;
            Assert.AreEqual(14, stream.Length);
            Assert.AreEqual(0xFF, stream.ReadByte());
            Assert.AreEqual(0xFE, stream.ReadByte());
            Assert.AreEqual('a', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('3', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('b', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('0', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
            Assert.AreEqual('\n', stream.ReadByte());
            Assert.AreEqual('\0', stream.ReadByte());
        }

        [Test]
        public void WriteLineNullStringFormatThrowsException()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine(null, "a"));
            Assert.Throws<ArgumentNullException>(() => writer.WriteLine("a", null));
        }

        [Test]
        public void WriteLineStringFormatWithLessFormatsThrows()
        {
            var writer = new TextWriter(stream);
            Assert.Throws<FormatException>(() => writer.WriteLine("a{0}{1}", 3));
            Assert.Throws<FormatException>(() => writer.WriteLine("a{1}", 3));
            Assert.Throws<FormatException>(() => writer.WriteLine("a{0}", new object[0]));
        }

        [Test]
        public void WriteLineStringFormatWithMoreArgsDoesNotThrow()
        {
            var writer = new TextWriter(stream);
            Assert.DoesNotThrow(() => writer.WriteLine("a{0}", 3, 4, 5));
        }
    }
}
