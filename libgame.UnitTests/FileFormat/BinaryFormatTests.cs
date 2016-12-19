//
// BinaryFormatTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
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
namespace Libgame.UnitTests.FileFormat
{
    using System.IO;
    using Libgame.FileFormat;
    using IO;
    using NUnit.Framework;

    [TestFixture]
    public class BinaryFormatTests : BaseGeneralTests<BinaryFormat>
    {

        [Test]
        public void ConstructorWithStream()
        {
            DataStream stream = new DataStream(new MemoryStream(), 0, 0);
            BinaryFormat format = new BinaryFormat(stream);
            Assert.AreSame(stream, format.Stream);
            format.Dispose();
        }

        [Test]
        public void ConstructorWithPathAllowReadWrite()
        {
            string tempPath = Path.GetTempFileName();
            BinaryFormat format = new BinaryFormat(tempPath);

            Assert.DoesNotThrow(() => format.Stream.WriteByte(0xAE));
            format.Stream.Seek(0, SeekMode.Origin);
            Assert.AreEqual(0xAE, format.Stream.ReadByte());

            format.Dispose();
            File.Delete(tempPath);
        }

        [Test]
        public void ConstructorWithPathCreatesFile()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            BinaryFormat format = new BinaryFormat(tempPath);
            Assert.IsTrue(File.Exists(tempPath));

            format.Dispose();
            File.Delete(tempPath);
        }

        [Test]
        public void DisposeIsDisposingFormat()
        {
            string tempPath = Path.GetTempFileName();
            BinaryFormat format = new BinaryFormat(tempPath);
            format.Dispose();
            Assert.DoesNotThrow(() => File.Delete(tempPath));
        }

        [Test]
        public void CorrectName()
        {
            NameIsCorrect("libgame", "binary");
        }

        protected override BinaryFormat CreateDummyFormat()
        {
            DataStream stream = new DataStream(new MemoryStream(), 0, 0);
            return new BinaryFormat(stream);
        }
    }
}
