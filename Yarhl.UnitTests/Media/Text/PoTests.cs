//
// Po.cs
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
    using System;
    using FileFormat;
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class PoTests : BaseGeneralTests<Po>
    {
        [Test]
        public void CorrectName()
        {
            NameIsCorrect("Yarhl.common", "po");
        }

        [Test]
        public void DefaultValues()
        {
            var po = new Po();
            Assert.IsNull(po.Header);
            Assert.IsEmpty(po.Entries);
        }

        [Test]
        public void ConstructorWithHeader()
        {
            PoHeader header = new PoHeader("id", "yo");
            var po = new Po(header);
            Assert.IsNotNull(po.Header);
            Assert.AreSame(header, po.Header);
        }

        [Test]
        public void AddIsAddingEntries()
        {
            var po = CreateDummyFormat();
            var entry = new PoEntry();
            po.Add(entry);
            Assert.AreEqual(1, po.Entries.Count);
            Assert.Contains(entry, po.Entries);
        }

        [Test]
        public void AddChecksForErrors()
        {
            var po = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => po.Add((PoEntry)null));

            po.Dispose();
            Assert.Throws<ObjectDisposedException>(() => po.Add((PoEntry)null));
        }

        [Test]
        public void MergeEntriesIfWithSameId()
        {
            var po = CreateDummyFormat();
            var entry1 = new PoEntry("orig") { Reference = "ref1" };
            var entry2 = new PoEntry("orig") { Reference = "ref2" };
            po.Add(entry1);
            Assert.DoesNotThrow(() => po.Add(entry2));
            Assert.AreEqual(1, po.Entries.Count);
            Assert.AreEqual("orig", po.Entries[0].Original);
            Assert.AreEqual("ref1,ref2", po.Entries[0].Reference);
        }

        [Test]
        public void MergeEntriesIfWithSameIdAndSameContext()
        {
            var po = CreateDummyFormat();
            var entry1 = new PoEntry("orig") { Context = "1", Reference = "ref1" };
            var entry2 = new PoEntry("orig") { Context = "1", Reference = "ref2" };
            var entry3 = new PoEntry("orig") { Context = "1" };
            po.Add(entry1);
            Assert.DoesNotThrow(() => po.Add(entry2));
            Assert.DoesNotThrow(() => po.Add(entry3));
            Assert.AreEqual(1, po.Entries.Count);
            Assert.AreEqual("orig", po.Entries[0].Original);
            Assert.AreEqual("ref1,ref2", po.Entries[0].Reference);
        }

        [Test]
        public void MultipleEntriesDifferentTranslationThrows()
        {
            var po = CreateDummyFormat();
            var entry1 = new PoEntry("orig") { Translated = "trans" };
            var entry2 = new PoEntry("orig") { Translated = "trans2" };
            po.Add(entry1);
            Assert.Throws<InvalidOperationException>(() => po.Add(entry2));
        }

        [Test]
        public void CanAddMultipleEntriesSameIdDifferentContext()
        {
            var po = CreateDummyFormat();
            var entry1 = new PoEntry("orig") { Context = "1", Reference = "ref1" };
            var entry2 = new PoEntry("orig") { Context = "2", Reference = "ref2" };
            po.Add(entry1);
            Assert.DoesNotThrow(() => po.Add(entry2));
            Assert.AreEqual(2, po.Entries.Count);
            Assert.AreEqual("ref1", po.Entries[0].Reference);
            Assert.AreEqual("ref2", po.Entries[1].Reference);
        }

        [Test]
        public void AddMultipleEntries()
        {
            var po = CreateDummyFormat();
            var entries = new PoEntry[] { new PoEntry("org1"), new PoEntry("org2") };
            po.Add(entries);

            Assert.AreEqual(2, po.Entries.Count);
            Assert.Contains(entries[0], po.Entries);
            Assert.Contains(entries[1], po.Entries);
        }

        [Test]
        public void AddMultipleCheksForError()
        {
            var po = CreateDummyFormat();
            Assert.Throws<ArgumentNullException>(() => po.Add((PoEntry[])null));
        }

        protected override Po CreateDummyFormat()
        {
            return new Po();
        }
    }
}
