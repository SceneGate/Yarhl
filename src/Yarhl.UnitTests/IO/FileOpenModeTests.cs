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
    using NUnit.Framework;
    using Yarhl.IO;

    [TestFixture]
    public class FileOpenModeTests
    {
        [Test]
        public void ConvertToFileMode()
        {
            Assert.AreEqual(FileMode.Open, FileOpenMode.Read.ToFileMode());
            Assert.AreEqual(FileMode.Create, FileOpenMode.Write.ToFileMode());
            Assert.AreEqual(FileMode.Append, FileOpenMode.Append.ToFileMode());
            Assert.AreEqual(FileMode.OpenOrCreate, FileOpenMode.ReadWrite.ToFileMode());
            Assert.Throws<ArgumentOutOfRangeException>(() => ((FileOpenMode)0x100).ToFileMode());
        }

        [Test]
        public void ConvertToFileAccess()
        {
            Assert.AreEqual(FileAccess.Read, FileOpenMode.Read.ToFileAccess());
            Assert.AreEqual(FileAccess.Write, FileOpenMode.Write.ToFileAccess());
            Assert.AreEqual(FileAccess.Write, FileOpenMode.Append.ToFileAccess());
            Assert.AreEqual(FileAccess.ReadWrite, FileOpenMode.ReadWrite.ToFileAccess());
            Assert.Throws<ArgumentOutOfRangeException>(() => ((FileOpenMode)0x100).ToFileAccess());
        }
    }
}
