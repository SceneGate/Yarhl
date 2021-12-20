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
    public class PoHeaderTests
    {
        [Test]
        public void DefaultValues()
        {
            var header = new PoHeader();
            Assert.That(header.ProjectIdVersion, Is.Empty);
            Assert.That(header.ReportMsgidBugsTo, Is.Empty);
            Assert.That(header.Language, Is.Empty);

            header = new PoHeader("myID", "yo", "es");
            Assert.AreEqual("myID", header.ProjectIdVersion);
            Assert.AreEqual("yo", header.ReportMsgidBugsTo);
            Assert.AreEqual(DateTime.Now.ToShortDateString(), header.CreationDate);
            Assert.That(header.RevisionDate, Is.Empty);
            Assert.That(header.LastTranslator, Is.Empty);
            Assert.That(header.LanguageTeam, Is.Empty);
            Assert.AreEqual("es", header.Language);
            Assert.AreEqual("1.0", PoHeader.MimeVersion);
            Assert.AreEqual("text/plain; charset=UTF-8", PoHeader.ContentType);
            Assert.AreEqual("8bit", PoHeader.ContentTransferEncoding);
            Assert.That(header.PluralForms, Is.Empty);
            Assert.IsNotNull(header.Extensions);
            Assert.IsEmpty(header.Extensions);
        }

        [Test]
        public void TestProperties()
        {
            var header = new PoHeader("myID", "yo", "us") {
                ProjectIdVersion = "test1",
                ReportMsgidBugsTo = "test2",
                CreationDate = "test3",
                RevisionDate = "test4",
                LastTranslator = "test5",
                LanguageTeam = "test6",
                Language = "test7",
                PluralForms = "test8",
            };
            header.Extensions["X-MyExt"] = "the value";

            Assert.AreEqual("test1", header.ProjectIdVersion);
            Assert.AreEqual("test2", header.ReportMsgidBugsTo);
            Assert.AreEqual("test3", header.CreationDate);
            Assert.AreEqual("test4", header.RevisionDate);
            Assert.AreEqual("test5", header.LastTranslator);
            Assert.AreEqual("test6", header.LanguageTeam);
            Assert.AreEqual("test7", header.Language);
            Assert.AreEqual("test8", header.PluralForms);
            Assert.That(header.Extensions["X-MyExt"], Is.EqualTo("the value"));
        }

        [Test]
        public void CloneNullThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PoHeader(null));
        }

        [Test]
        public void Clone()
        {
            var header = new PoHeader("myID", "yo", "us") {
                ProjectIdVersion = "test1",
                ReportMsgidBugsTo = "test2",
                CreationDate = "test3",
                RevisionDate = "test4",
                LastTranslator = "test5",
                LanguageTeam = "test6",
                Language = "test7",
                PluralForms = "test8",
            };
            header.Extensions["X-MyExt"] = "the value";

            PoHeader clone = new PoHeader(header);
            Assert.AreNotSame(header, clone);
            Assert.AreEqual("test1", clone.ProjectIdVersion);
            Assert.AreEqual("test2", clone.ReportMsgidBugsTo);
            Assert.AreEqual("test3", clone.CreationDate);
            Assert.AreEqual("test4", clone.RevisionDate);
            Assert.AreEqual("test5", clone.LastTranslator);
            Assert.AreEqual("test6", clone.LanguageTeam);
            Assert.AreEqual("test7", clone.Language);
            Assert.AreEqual("test8", clone.PluralForms);
            Assert.That(clone.Extensions["X-MyExt"], Is.EqualTo("the value"));
        }
    }
}
