// PoEntryTests.cs
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
namespace Yarhl.UnitTests.Media.Text
{
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class PoEntryTests
    {
        [Test]
        public void DefaultValues()
        {
            PoEntry entry = new PoEntry();
            Assert.AreEqual(string.Empty, entry.Original);
            Assert.AreEqual(string.Empty, entry.Translated);
            Assert.IsNull(entry.Context);
            Assert.IsNull(entry.TranslatorComment);
            Assert.IsNull(entry.ExtractedComments);
            Assert.IsNull(entry.Reference);
            Assert.IsNull(entry.Flags);
            Assert.IsNull(entry.PreviousContext);
            Assert.IsNull(entry.PreviousOriginal);

            entry = new PoEntry("original");
            Assert.AreEqual("original", entry.Original);
        }

        [Test]
        public void TestProperties()
        {
            PoEntry entry = new PoEntry {
                Original = "test0",
                Translated = "test1",
                Context = "test2",
                TranslatorComment = "test3",
                ExtractedComments = "test4",
                Reference = "test5",
                Flags = "test6",
                PreviousContext = "test7",
                PreviousOriginal = "test8",
            };

            Assert.AreEqual("test0", entry.Original);
            Assert.AreEqual("test1", entry.Translated);
            Assert.AreEqual("test2", entry.Context);
            Assert.AreEqual("test3", entry.TranslatorComment);
            Assert.AreEqual("test4", entry.ExtractedComments);
            Assert.AreEqual("test5", entry.Reference);
            Assert.AreEqual("test6", entry.Flags);
            Assert.AreEqual("test7", entry.PreviousContext);
            Assert.AreEqual("test8", entry.PreviousOriginal);
        }

        [Test]
        public void TextShowsOriginalOrTranslated()
        {
            PoEntry entry = new PoEntry();
            Assert.That(entry.Text, Is.EqualTo(string.Empty));

            entry.Original = "original";
            Assert.That(entry.Text, Is.EqualTo(entry.Original));

            entry.Translated = "translated";
            Assert.That(entry.Text, Is.EqualTo(entry.Translated));
        }
    }
}
