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
            NameIsCorrect("Yarhl.Common", "Po");
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
            PoHeader header = new PoHeader("id", "yo", "es");
            var po = new Po(header);
            Assert.IsNotNull(po.Header);
            Assert.AreSame(header, po.Header);
        }

        [Test]
        public void SetHeader()
        {
            var po = CreateDummyFormat();
            PoHeader header = new PoHeader("id", "yo", "es");
            Assert.That(() => po.Header = header, Throws.Nothing);
        }

        [Test]
        public void SetHeaderAfterDisposeThrowsException()
        {
            PoHeader header = new PoHeader("id", "yo", "es");
            var po = CreateDummyFormat();
            po.Dispose();
            Assert.That(
                () => po.Header = header,
                Throws.InstanceOf<ObjectDisposedException>());
        }

        [Test]
        public void SetNullHeaderDoesNotThrowsException()
        {
            var po = CreateDummyFormat();
            Assert.That(() => po.Header = null, Throws.Nothing);
        }

        [Test]
        public void SetHeaderWithMissingFieldsThrowsException()
        {
            var po = CreateDummyFormat();
            var header = new PoHeader();

            var exception = Throws.InstanceOf<FormatException>()
                .With.Message.EqualTo("ProjectIdVersion is empty");
            Assert.That(() => po.Header = header, exception);

            header.ProjectIdVersion = "id";
            exception = Throws.InstanceOf<FormatException>()
                .With.Message.EqualTo("ReportMsgidBugsTo is empty");
            Assert.That(() => po.Header = header, exception);

            header.ReportMsgidBugsTo = "yo";
            exception = Throws.InstanceOf<FormatException>()
                .With.Message.EqualTo("Language is empty");
            Assert.That(() => po.Header = header, exception);
        }

        [Test]
        public void AddIsAddingEntries()
        {
            var po = CreateDummyFormat();
            var entry = new PoEntry("test");
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
        public void AddThrowsIfOriginalIsEmpty()
        {
            var po = CreateDummyFormat();
            var entry = new PoEntry();

            var exception = Throws.InstanceOf<FormatException>()
                .With.Message.EqualTo("Original is empty");
            Assert.That(() => po.Add(entry), exception);
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

        [Test]
        public void AddMultipleEntriesAfterDisposeThrows()
        {
            var po = CreateDummyFormat();
            po.Dispose();
            Assert.That(
                () => po.Add(new[] { new PoEntry("t1") }),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void FindEntryReturnsEntry()
        {
            var po = CreateDummyFormat();
            var entries = new[] { new PoEntry("org1"), new PoEntry("org2") };
            po.Add(entries);

            Assert.That(po.FindEntry("org1"), Is.SameAs(entries[0]));
            Assert.That(po.FindEntry("org2"), Is.SameAs(entries[1]));
        }

        [Test]
        public void FindEntryWithSameOriginalDifferentContextReturnsCorrect()
        {
            var po = CreateDummyFormat();
            po.Add(new PoEntry("t1") { Context = "ctx1" });
            po.Add(new PoEntry("t1") { Context = "ctx2" });

            Assert.That(po.FindEntry("t1", "ctx1"), Is.SameAs(po.Entries[0]));
            Assert.That(po.FindEntry("t1", "ctx2"), Is.SameAs(po.Entries[1]));
        }

        [Test]
        public void FindUnknownEntryFromIndexerReturnsNull()
        {
            var po = CreateDummyFormat();
            Assert.That(po.FindEntry("unknown"), Is.Null);
        }

        [Test]
        public void FindEntryWithSameOriginalAndUnknownContextReturnsNull()
        {
            var po = CreateDummyFormat();
            po.Add(new PoEntry("t1") { Context = "ctx1" });
            po.Add(new PoEntry("t1") { Context = "ctx2" });

            Assert.That(po.FindEntry("t1", "unk"), Is.Null);
        }

        [Test]
        public void FindEntryAfterDisposeThrows()
        {
            var po = CreateDummyFormat();
            po.Dispose();
            Assert.That(
                () => po.FindEntry("t1"),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void FindEntryNullOriginalThrows()
        {
            var po = CreateDummyFormat();
            Assert.That(() => po.FindEntry(null), Throws.ArgumentNullException);
            Assert.That(() => po.FindEntry(string.Empty), Throws.ArgumentNullException);
        }

        protected override Po CreateDummyFormat()
        {
            return new Po();
        }
    }
}
