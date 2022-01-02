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
namespace Yarhl.UnitTests.Media.Text
{
    using System;
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class PoEntryTests
    {
        [Test]
        public void DefaultValues()
        {
            PoEntry entry = new PoEntry();
            Assert.That(entry.Original, Is.Empty);
            Assert.That(entry.Translated, Is.Empty);
            Assert.That(entry.Context, Is.Empty);
            Assert.That(entry.TranslatorComment, Is.Empty);
            Assert.That(entry.ExtractedComments, Is.Empty);
            Assert.That(entry.Reference, Is.Empty);
            Assert.That(entry.Flags, Is.Empty);
            Assert.That(entry.PreviousContext, Is.Empty);
            Assert.That(entry.PreviousOriginal, Is.Empty);

            entry = new PoEntry("original");
            Assert.That(entry.Original, Is.EqualTo("original"));
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

        [Test]
        public void CloneNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PoEntry((PoEntry)null));
        }

        [Test]
        public void Clone()
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

            PoEntry clone = new PoEntry(entry);

            Assert.AreNotSame(entry, clone);
            Assert.AreEqual("test0", clone.Original);
            Assert.AreEqual("test1", clone.Translated);
            Assert.AreEqual("test2", clone.Context);
            Assert.AreEqual("test3", clone.TranslatorComment);
            Assert.AreEqual("test4", clone.ExtractedComments);
            Assert.AreEqual("test5", clone.Reference);
            Assert.AreEqual("test6", clone.Flags);
            Assert.AreEqual("test7", clone.PreviousContext);
            Assert.AreEqual("test8", clone.PreviousOriginal);
        }
    }
}
