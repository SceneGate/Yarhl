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
namespace Yarhl.UnitTests.IO
{
    using System;
    using System.IO;
    using System.Text;
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class TextDataReaderTests
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
        public void PropertyValuesWithConstructorOneArgument()
        {
            var reader = new TextDataReader(stream);
            Assert.AreSame(stream, reader.Stream);
            Assert.AreSame(Encoding.UTF8, reader.Encoding);
            Assert.AreEqual(Environment.NewLine, reader.NewLine);
            Assert.IsTrue(reader.AutoNewLine);
        }

        [Test]
        public void PropertyValuesWithConstructorEncoding()
        {
            var reader = new TextDataReader(stream, Encoding.ASCII);
            Assert.AreSame(stream, reader.Stream);
            Assert.AreSame(Encoding.ASCII, reader.Encoding);
            Assert.AreEqual(Environment.NewLine, reader.NewLine);
            Assert.IsTrue(reader.AutoNewLine);
        }

        [Test]
        public void PropertyValuesWithConstructorEncodingName()
        {
            var reader = new TextDataReader(stream, "utf-16");
            Assert.AreSame(stream, reader.Stream);
            Assert.AreSame(Encoding.Unicode, reader.Encoding);
            Assert.AreEqual(Environment.NewLine, reader.NewLine);
            Assert.IsTrue(reader.AutoNewLine);
        }

        [Test]
        public void CreateWithShiftJisEncoding()
        {
            // It will automatically register the encodings.
            var reader = new TextDataReader(stream, "shift-jis");
            Assert.That(reader.Encoding.CodePage, Is.EqualTo(932));
        }

        [Test]
        public void TestConstructorWithNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new TextDataReader(null));

            Assert.Throws<ArgumentNullException>(() => new TextDataReader(null, Encoding.ASCII));
            Assert.Throws<ArgumentNullException>(() => new TextDataReader(stream, (Encoding)null));

            Assert.Throws<ArgumentNullException>(() => new TextDataReader(null, "ascii"));
            Assert.Throws<ArgumentNullException>(() => new TextDataReader(stream, (string)null));
        }

        [Test]
        public void EntityDoesNotOwnStream()
        {
            using var dataStream = new DataStream();
            using var commonStream = new MemoryStream();
            int initialCount = DataStream.ActiveStreams;

            var myReader = new TextDataReader(dataStream);
            Assert.That(myReader.Stream, Is.SameAs(dataStream));
            Assert.That(DataStream.ActiveStreams, Is.EqualTo(initialCount));

            myReader = new TextDataReader(commonStream);
            Assert.That(myReader.Stream.BaseStream, Is.SameAs(commonStream));
            Assert.That(DataStream.ActiveStreams, Is.EqualTo(initialCount));
            Assert.That(myReader.Stream.InternalInfo.NumInstances, Is.EqualTo(0));
        }

        [Test]
        public void PropertyActuallyChangesValues()
        {
            var reader = new TextDataReader(stream);
            reader.NewLine = "a";
            Assert.AreEqual("a", reader.NewLine);

            reader.AutoNewLine = false;
            Assert.IsFalse(reader.AutoNewLine);
        }

        [Test]
        public void AutoNewLineIsFalseAfterSettingNewLine()
        {
            var reader = new TextDataReader(stream);
            Assert.IsTrue(reader.AutoNewLine);
            reader.NewLine = "a";
            Assert.IsFalse(reader.AutoNewLine);
        }

        [Test]
        public void ReadChar()
        {
            stream.WriteByte(0x61);
            stream.WriteByte(0x30);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            Assert.AreEqual('a', reader.Read());
            Assert.AreEqual(1, stream.Position);
        }

        [Test]
        public void ReadCharWithEncoding()
        {
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            Assert.AreEqual('a', reader.Read());
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadCharWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            Assert.AreEqual('a', reader.Read());
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadEndOfStreamThrowsException()
        {
            var reader = new TextDataReader(stream);
            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Read());
        }

        [Test]
        public void ReadArrayChars()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var chars = reader.Read(2);

            Assert.AreEqual('1', chars[0]);
            Assert.AreEqual('9', chars[1]);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadArrayWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var chars = reader.Read(2);

            Assert.AreEqual('1', chars[0]);
            Assert.AreEqual('9', chars[1]);
            Assert.AreEqual(6, stream.Position);
        }

        [Test]
        public void ReadArrayThrowsExceptionIfNegative()
        {
            var reader = new TextDataReader(stream);
            Assert.Throws<ArgumentOutOfRangeException>(() => reader.Read(-3));
        }

        [Test]
        public void ReadArrayThrowsEOF()
        {
            stream.WriteByte(0x31);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Read(3));
        }

        [Test]
        public void ReadToCharDoesNotIncludeStopChar()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var text = reader.ReadToToken("9");

            Assert.AreEqual("1", text);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadToCharWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var text = reader.ReadToToken("9");

            Assert.AreEqual("1", text);
            Assert.AreEqual(6, stream.Position);
        }

        [Test]
        public void ReadToPartialToken()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream);
            string text = reader.ReadToToken("95");

            Assert.AreEqual("1", text);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadToTokenWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            string text = reader.ReadToToken("95");

            Assert.AreEqual("1", text);
            Assert.AreEqual(8, stream.Position);
        }

        [Test]
        public void ReadToTokenWhenEOFThrows()
        {
            var reader = new TextDataReader(stream);
            Assert.That(
                () => reader.ReadToToken("3"),
                Throws.InstanceOf<EndOfStreamException>());
        }

        [Test]
        public void ReadToNullOrEmptyToken()
        {
            var reader = new TextDataReader(stream);
            Assert.That(() => reader.ReadToToken(null), Throws.ArgumentNullException);
            Assert.That(() => reader.ReadToToken(string.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void ReadToTokenMultipleBuffers()
        {
            for (int i = 0; i < 150; i++)
                stream.WriteByte(0x30);

            stream.WriteByte(0x35);
            stream.WriteByte(0x38);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            string text = reader.ReadToToken("5");

            Assert.That(text, Is.EqualTo(new string('0', 150)));
            Assert.That(stream.Position, Is.EqualTo(151));
        }

        [Test]
        public void ReadToTokenHalfEncodedTail()
        {
            stream.WriteByte(0x30);
            stream.WriteByte(0x00);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.WriteByte(0xe6);
            stream.WriteByte(0xbc);
            stream.WriteByte(0xa2);
            stream.WriteByte(0xe5);  // half encoded
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            string text = reader.ReadToToken("a");

            Assert.That(text, Is.EqualTo("01"));
            Assert.That(stream.Position, Is.EqualTo(6));
        }

        [Test]
        public void ReadToTokenMultipleCodeUnitChars()
        {
            byte[] buffer = {
                0x01, 0xd8, 0x37, 0xdc, 0x61, 0x00,
                0xa2, 0xe5, // half encoded, missing second utf-16 code unit
            };
            stream.Write(buffer, 0, buffer.Length);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            string text = reader.ReadToToken("a");

            Assert.That(text, Is.EqualTo("\uD801\uDC37"));
            Assert.That(stream.Position, Is.EqualTo(6));
        }

        [Test]
        public void ReadToTokenHalfEncodedBetweenBuffers()
        {
            // first buffer
            for (int i = 0; i < 127; i++)
                stream.WriteByte(0x30);
            stream.WriteByte(0xe6);

            // second buffer
            stream.WriteByte(0xbc);
            stream.WriteByte(0xa2);
            stream.WriteByte(0x08);
            stream.WriteByte(0x30);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            string text = reader.ReadToToken("\x08");

            Assert.That(text, Is.EqualTo(new string('0', 127) + '漢'));
            Assert.That(stream.Position, Is.EqualTo(131));
        }

        [Test]
        public void ReadToTokenHalfEncodedTokenBetweenBuffers()
        {
            // first buffer
            for (int i = 0; i < 127; i++)
                stream.WriteByte(0x30);
            stream.WriteByte(0xe6);

            // second buffer
            stream.WriteByte(0xbc);
            stream.WriteByte(0xa2);
            stream.WriteByte(0x30);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            string text = reader.ReadToToken("漢");

            Assert.That(text, Is.EqualTo(new string('0', 127)));
            Assert.That(stream.Position, Is.EqualTo(130));
        }

        [Test]
        public void ReadUnixLine()
        {
            stream.WriteByte(0x35);
            stream.WriteByte(0x39);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x31);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            reader.AutoNewLine = false;
            reader.NewLine = "\n";
            var line = reader.ReadLine();

            Assert.AreEqual("59", line);
            Assert.AreEqual(3, stream.Position);
        }

        [Test]
        public void ReadLineWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.WriteByte(0x39);
            stream.WriteByte(0x00);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x00);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            reader.AutoNewLine = false;
            reader.NewLine = "\n";
            var line = reader.ReadLine();

            Assert.AreEqual("59", line);
            Assert.AreEqual(8, stream.Position);
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

            var reader = new TextDataReader(stream, Encoding.Unicode);
            reader.AutoNewLine = false;
            reader.NewLine = "\r\n";
            var line = reader.ReadLine();

            Assert.AreEqual("51", line);
            Assert.AreEqual(8, stream.Position);
        }

        [Test]
        public void ReadAutoNewLine()
        {
            stream.WriteByte(0x35);
            stream.WriteByte(0x31);
            stream.WriteByte(0x0D);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x31);
            stream.WriteByte(0x32);
            stream.WriteByte(0x0A);
            stream.WriteByte(0x30);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            Assert.That(reader.ReadLine(), Is.EqualTo("51"));
            Assert.AreEqual(4, stream.Position);

            Assert.That(reader.ReadLine(), Is.EqualTo("12"));
            Assert.AreEqual(7, stream.Position);

            Assert.That(reader.ReadLine(), Is.EqualTo("0"));
            Assert.AreEqual(8, stream.Position);
        }

        [Test]
        public void ReadCustonNewLine()
        {
            stream.WriteByte((byte)'5');
            stream.WriteByte((byte)'1');
            stream.WriteByte((byte)'<');
            stream.WriteByte((byte)'b');
            stream.WriteByte((byte)'r');
            stream.WriteByte((byte)'/');
            stream.WriteByte((byte)'>');
            stream.WriteByte((byte)'0');
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            reader.NewLine = "<br/>";
            reader.AutoNewLine = false;

            Assert.That(reader.ReadLine(), Is.EqualTo("51"));
            Assert.AreEqual(7, stream.Position);

            Assert.That(reader.ReadLine(), Is.EqualTo("0"));
            Assert.AreEqual(8, stream.Position);
        }

        [Test]
        public void ReadLineWithoutNewLineDoesNotThrow()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
            var line = string.Empty;
            Assert.DoesNotThrow(() => line = reader.ReadLine());
            Assert.AreEqual("19", line);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadLineWhenEOFThrowsException()
        {
            var reader = new TextDataReader(stream);
            Assert.That(
                () => reader.ReadLine(),
                Throws.InstanceOf<EndOfStreamException>());

            reader.AutoNewLine = false;
            Assert.That(
                () => reader.ReadLine(),
                Throws.InstanceOf<EndOfStreamException>());
        }

        [Test]
        public void ReadToEnd()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var text = reader.ReadToEnd();

            Assert.AreEqual("15", text);
            Assert.AreEqual(4, stream.Position);
        }

        [Test]
        public void ReadToEndWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x31);
            stream.WriteByte(0x00);
            stream.WriteByte(0x35);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            var text = reader.ReadToEnd();

            Assert.AreEqual("15", text);
            Assert.AreEqual(6, stream.Position);
        }

        [Test]
        public void ReadToEndStartingAtSomePos()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x35);
            stream.Position = 1;

            var reader = new TextDataReader(stream);
            var text = reader.ReadToEnd();

            Assert.AreEqual("5", text);
            Assert.AreEqual(2, stream.Position);
        }

        [Test]
        public void ReadToEndIfEmpty()
        {
            var reader = new TextDataReader(stream);
            Assert.AreEqual(string.Empty, reader.ReadToEnd());
        }

        [Test]
        public void PeekChar()
        {
            stream.WriteByte(0x41);
            stream.WriteByte(0x42);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream, Encoding.Unicode);
            Assert.AreEqual('a', reader.Peek());
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekCharWithPreamble()
        {
            stream.WriteByte(0xFF);
            stream.WriteByte(0xFE);
            stream.WriteByte(0x61);
            stream.WriteByte(0x00);
            stream.Position = 0;

            var reader = new TextDataReader(stream, Encoding.Unicode);
            Assert.AreEqual('a', reader.Peek());
            Assert.AreEqual(0, stream.Position);
        }

        [Test]
        public void PeekEndOfStreamThrowsException()
        {
            var reader = new TextDataReader(stream);
            Assert.Throws<System.IO.EndOfStreamException>(() => reader.Peek());
        }

        [Test]
        public void PeekArrayChars()
        {
            stream.WriteByte(0x31);
            stream.WriteByte(0x39);
            stream.WriteByte(0x35);
            stream.Position = 0;

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream);
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

            var reader = new TextDataReader(stream);
            reader.NewLine = "\r\n";
            var line = reader.PeekLine();

            Assert.AreEqual("51", line);
            Assert.AreEqual(0, stream.Position);
        }
    }
}
