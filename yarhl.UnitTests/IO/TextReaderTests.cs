﻿//
// TextReaderTests.cs
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
    public class TextReaderTests
    {
        DataStream stream;
        TextReader reader;

        [SetUp]
        public void SetUp()
        {
            stream = new DataStream();
            reader = new TextReader(stream);
        }

        [TearDown]
        public void TearDown()
        {
            stream.Dispose();
        }

        [Test]
        public void PropertyValuesWithConstructorOneArgument()
        {
            Assert.AreSame(stream, reader.Stream);
            Assert.AreSame(Encoding.UTF8, reader.Encoding);
            Assert.AreEqual("\n", reader.NewLine);
        }

        [Test]
        public void PropertyValuesWithConstructorEncoding()
        {
            reader = new TextReader(stream, Encoding.ASCII);
            Assert.AreSame(stream, reader.Stream);
            Assert.AreSame(Encoding.ASCII, reader.Encoding);
            Assert.AreEqual("\n", reader.NewLine);
        }

        [Test]
        public void TestConstructorWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TextReader(null));

            Assert.Throws<ArgumentNullException>(() => new TextReader(null, Encoding.ASCII));
            Assert.Throws<ArgumentNullException>(() => new TextReader(stream, null));
        }

        [Test]
        public void PropertyActuallyChangesValues()
        {
            reader.Encoding = Encoding.BigEndianUnicode;
            Assert.AreSame(Encoding.BigEndianUnicode, reader.Encoding);

            reader.NewLine = "a";
            Assert.AreEqual("a", reader.NewLine);
        }

        [Test]
        public void ReadChar()
        {
            stream.WriteByte(0x61);
            stream.WriteByte(0x30);
            stream.Position = 0;

            Assert.AreEqual('a', reader.Read());
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void ReadCharWithEncoding()
        {
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.Encoding = Encoding.GetEncoding("utf-16");
            Assert.AreEqual('a', reader.Read());
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadEndOfStreamThrowsException()
        {
            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Read());
        }

        [Test]
        public void ReadArrayChars()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var chars = reader.Read(2);

            Assert.AreEqual('1', chars[0]);
            Assert.AreEqual('9', chars[1]);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadArrayWithEncoding()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.Encoding = Encoding.GetEncoding("utf-16");
            var chars = reader.Read(2);

            Assert.AreEqual('1', chars[0]);
            Assert.AreEqual('9', chars[1]);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadArrayThrowsExceptionIfNegative()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Read(-3));
        }

        [Test]
        public void ReadArrayThrowsEOF()
        {
            stream.WriteByte(0x31);
            stream.Position = 0;

            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Read(3));
        }

        [Test]
        public void ReadToCharDoesNotIncludeStopChar()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            string text = reader.ReadToToken("9");

            Assert.AreEqual("1", text);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadToCharDoesNotThrowsEOFIfCharNotFound()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.Position = 0;

            string text = string.Empty;
            Assert.DoesNotThrow(() => text = reader.ReadToToken("5"));

            Assert.AreEqual("19", text);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadToCharWithEncoding()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.Encoding = Encoding.GetEncoding("utf-16");
            var text = reader.ReadToToken("9");

            Assert.AreEqual("1", text);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadToPartialToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            string text = reader.ReadToToken("5.");

            Assert.AreEqual("195", text);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadToToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            string text = reader.ReadToToken("95");

            Assert.AreEqual("1", text);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadToTokenWhenEOFReturnsNull()
        {
            Assert.IsNull(reader.ReadToToken("3"));
        }

        [Test]
        public void ReadUnixLine()
        {
            stream.WriteByte(0x35);
            stream.WriteByte(0x39);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x31);
            stream.Position = 0;

            var line = reader.ReadLine();

            Assert.AreEqual("59", line);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadWindowsLineWithEncoding()
        {
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x0D);
            stream.WriteByte(0x00);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x00);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.NewLine = "\r\n";
            reader.Encoding = Encoding.GetEncoding("utf-16");
            var line = reader.ReadLine();

            Assert.AreEqual("51", line);
            Assert.AreEqual(8, stream.Position);
        }

        [Test]
        public void ReadLineWithoutNewLineDoesNotThrow()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.Position = 0;

            var line = string.Empty;
            Assert.DoesNotThrow(() => line = reader.ReadLine());
            Assert.AreEqual("19", line);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadLineWhenEOFReturnsNull()
        {
            Assert.IsNull(reader.ReadLine());
        }

        [Test]
        public void ReadToEnd()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var text = reader.ReadToEnd();

            Assert.AreEqual("15", text);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadToEndWithEncoding()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.Encoding = Encoding.GetEncoding("utf-16");
            var text = reader.ReadToEnd();

            Assert.AreEqual("15", text);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadToEndStartingAtSomePos()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x35);
            stream.Position = 1;

            var text = reader.ReadToEnd();

            Assert.AreEqual("5", text);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadToEndIfEmpty()
        {
            Assert.AreEqual(string.Empty, reader.ReadToEnd());
        }

        [Test]
        public void PeekChar()
        {
            stream.WriteByte(0x41);
            stream.WriteByte(0x42);
            stream.Position = 0;

            char ch = reader.Peek();

            Assert.AreEqual('A', ch);
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekCharWithEncoding()
        {
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.Position = 0;

            reader.Encoding = Encoding.GetEncoding("utf-16");
            Assert.AreEqual('a', reader.Peek());
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekEndOfStreamThrowsException()
        {
            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Peek());
        }

        [Test]
        public void PeekArrayChars()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var chars = reader.Peek(2);

            Assert.AreEqual('1', chars[0]);
            Assert.AreEqual('9', chars[1]);
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekToToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            string text = reader.PeekToToken("5");

            Assert.AreEqual("19", text);
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekLine()
        {
            stream.WriteByte(0x35);
            stream.WriteByte(0x31);
            stream.WriteByte(0x0D);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x31);
            stream.Position = 0;

            reader.NewLine = "\r\n";
            var line = reader.PeekLine();

            Assert.AreEqual("51", line);
            Assert.AreEqual(0, stream.Position);
        }
    }
}
