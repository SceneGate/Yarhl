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
    using Yarhl.FileFormat;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    [TestFixture]
    public class Po2BinaryTests
    {
        [Test]
        public void ConvertEmptyPo()
        {
            var po = new Po();
            CompareText(ConvertFormat.To<BinaryFormat>(po), string.Empty);
        }

        [Test]
        public void ConvertEmptyString()
        {
            Assert.DoesNotThrow(() => ConvertStringToPo(string.Empty));
        }

        [Test]
        public void ReadMultiLine()
        {
            string text = "msgid \"hey\"";
            Assert.AreEqual("hey", ConvertStringToPo(text).Entries[0].Original);

            text = "msgid \"hey\\n\"\n\"test\"";
            Assert.AreEqual("hey\ntest", ConvertStringToPo(text).Entries[0].Original);

            text = "msgid \"\"\n\"hey\"";
            Assert.AreEqual("hey", ConvertStringToPo(text).Entries[0].Original);
        }

        [Test]
        public void ConvertWrongMultiLineThrowsException()
        {
            // multi line fields must not start with spaces
            string text = "msgid \"\"\n \"test\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");

            text = "msgid  \"\"\n\"test\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");

            // Cannot end with space
            text = "msgid \"\"\n\"test\" ";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");

            text = "msgid \"\"t\n\"test\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");

            // Cannot miss first quotes
            text = "msgid \n\"test\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid line quotes");

            // Cannot have open quotes
            text = "msgid \"\"\n\"test";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");

            text = "msgid \"\"\ntest\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Line quotes in invalid position");
        }

        [Test]
        public void ConvertBasicHeaderNoEntries()
        {
            var po = new Po(new PoHeader("myId", "yo", "es") { CreationDate = "today" });
            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""PO-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: es\n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
";
            text = text.Replace("\r\n", "\n");

            var newPo = ConvertStringToPo(text);

            CompareText(ConvertFormat.To<BinaryFormat>(po), text);
            Assert.AreEqual(po.Header.ProjectIdVersion, newPo.Header.ProjectIdVersion);
            Assert.AreEqual(po.Header.ReportMsgidBugsTo, newPo.Header.ReportMsgidBugsTo);
            Assert.AreEqual(po.Header.CreationDate, newPo.Header.CreationDate);
            Assert.AreEqual(po.Header.Language, newPo.Header.Language);
            Assert.IsEmpty(newPo.Header.LastTranslator);
            Assert.IsEmpty(newPo.Entries);
        }

        [Test]
        public void ConvertFullHeaderNoEntries()
        {
            var header = new PoHeader("myId", "yo", "SC") {
                CreationDate = "today",
                RevisionDate = "tomorrow",
                LastTranslator = "she",
                LanguageTeam = "bestteam",
                PluralForms = "pl",
            };
            header.Extensions["Generator"] = "yarhl";
            header.Extensions["Hey"] = "hoy";

            var testPo = new Po(header);

            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""PO-Revision-Date: tomorrow\n""
""Last-Translator: she\n""
""Language-Team: bestteam\n""
""Language: SC\n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
""Plural-Forms: pl\n""
""X-Generator: yarhl\n""
""X-Hey: hoy\n""
";
            text = text.Replace("\r\n", "\n");
            var newPo = ConvertStringToPo(text);
            var newHeader = newPo.Header;

            CompareText(ConvertFormat.To<BinaryFormat>(testPo), text);
            Assert.AreEqual(header.ProjectIdVersion, newHeader.ProjectIdVersion);
            Assert.AreEqual(header.ReportMsgidBugsTo, newHeader.ReportMsgidBugsTo);
            Assert.AreEqual(header.CreationDate, newHeader.CreationDate);
            Assert.AreEqual(header.RevisionDate, newHeader.RevisionDate);
            Assert.AreEqual(header.LastTranslator, newHeader.LastTranslator);
            Assert.AreEqual(header.LanguageTeam, newHeader.LanguageTeam);
            Assert.AreEqual(header.Language, newHeader.Language);
            Assert.AreEqual(header.PluralForms, newHeader.PluralForms);
            Assert.That(header.Extensions["Generator"], Is.EqualTo(newHeader.Extensions["Generator"]));
            Assert.That(header.Extensions["Hey"], Is.EqualTo(newHeader.Extensions["Hey"]));
            Assert.IsEmpty(newPo.Entries);
        }

        [Test]
        public void ThrowExceptionIfInvalidHeaderFixedFields()
        {
            string text = "msgid \"\"\nmsgstr \"\"\n\"Content-Type: hehe\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid Content-Type");

            text = "msgid \"\"\nmsgstr \"\"\n\"Content-Transfer-Encoding: hehe\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid Content-Transfer-Encoding");

            text = "msgid \"\"\nmsgstr \"\"\n\"MIME-Version: hehe\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid MIME-Version");
        }

        [Test]
        public void ThrowIfUnknownOrInvalidHeaderField()
        {
            string text = "msgid \"\"\nmsgstr \"\"\n\"Unknown: hehe\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown header: Unknown: hehe");

            text = "msgid \"\"\nmsgstr \"\"\n\"Last-Translator:nospace\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid line format: Last-Translator:nospace");
        }

        [Test]
        public void ParseExtendedHeaderEntriesd()
        {
            string text = @"
msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""Language: SC\n""
""X-Generator: libgame""
";
            text = text.Replace("\r\n", "\n");
            Po testPo = null;
            Assert.DoesNotThrow(() => testPo = ConvertStringToPo(text));
            Assert.That(testPo?.Header?.Extensions["Generator"], Is.EqualTo("libgame"));

            text = "msgid \"\"\nmsgstr \"\"\n\"X: libgame\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown header: Unknown: X");

            text = "msgid \"\"\nmsgstr \"\"\n\"X-: libgame\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown header: Unknown: X-");
        }

        [Test]
        public void ConvertNoHeaderEntries()
        {
            var testPo = new Po();
            testPo.Add(new PoEntry { Original = "original" });
            testPo.Add(new PoEntry { Original = "totranslate", Translated = "translated" });

            string text = @"
msgid ""original""
msgstr """"

msgid ""totranslate""
msgstr ""translated""
";
            text = text.Replace("\r\n", "\n");
            var newPo = ConvertStringToPo(text);

            CompareText(ConvertFormat.To<BinaryFormat>(testPo), text);
            Assert.That(newPo.Header, Is.Not.Null);
            Assert.That(newPo.Header.ProjectIdVersion, Is.Empty);
            Assert.AreEqual(2, newPo.Entries.Count);
        }

        [Test]
        public void ConvertFullEntry()
        {
            var testPo = new Po();
            testPo.Add(new PoEntry {
                Original = "original",
                Translated = "translated",
                TranslatorComment = "a comment",
                ExtractedComments = "hehe",
                Reference = "ref1",
                Flags = "flag1,flag2",
                PreviousContext = "prev ctx",
                PreviousOriginal = "prev org",
                Context = "a ctx",
            });

            string text = @"
#  a comment
#. hehe
#: ref1
#, flag1,flag2
#| msgctxt prev ctx
#| msgid prev org
msgctxt ""a ctx""
msgid ""original""
msgstr ""translated""
";
            text = text.Replace("\r\n", "\n");
            var newPo = ConvertStringToPo(text);

            CompareText(ConvertFormat.To<BinaryFormat>(testPo), text);
            Assert.AreEqual(1, newPo.Entries.Count);
            Assert.AreEqual(testPo.Entries[0].Original, newPo.Entries[0].Original);
            Assert.AreEqual(testPo.Entries[0].Translated, newPo.Entries[0].Translated);
            Assert.AreEqual(testPo.Entries[0].TranslatorComment, newPo.Entries[0].TranslatorComment);
            Assert.AreEqual(testPo.Entries[0].ExtractedComments, newPo.Entries[0].ExtractedComments);
            Assert.AreEqual(testPo.Entries[0].Reference, newPo.Entries[0].Reference);
            Assert.AreEqual(testPo.Entries[0].Flags, newPo.Entries[0].Flags);
            Assert.AreEqual(testPo.Entries[0].PreviousContext, newPo.Entries[0].PreviousContext);
            Assert.AreEqual(testPo.Entries[0].PreviousOriginal, newPo.Entries[0].PreviousOriginal);
            Assert.AreEqual(testPo.Entries[0].Context, newPo.Entries[0].Context);
        }

        [Test]
        public void ConvertSpecialEntry()
        {
            var testPo = new Po();
            testPo.Add(new PoEntry {
                Original = "original",
                Translated = "trans\nl\"a\"ted",
            });

            string text = @"
msgid ""original""
msgstr """"
""trans\n""
""l\""a\""ted""
";
            text = text.Replace("\r\n", "\n");
            var newPo = ConvertStringToPo(text);

            CompareText(ConvertFormat.To<BinaryFormat>(testPo), text);
            Assert.AreEqual(1, newPo.Entries.Count);
            Assert.AreEqual(testPo.Entries[0].Original, newPo.Entries[0].Original);
            Assert.AreEqual(testPo.Entries[0].Translated, newPo.Entries[0].Translated);
        }

        [Test]
        public void InvalidFieldsInEntryThrowException()
        {
            string text = "msgid \"test\"\nmsgunk \"ein?\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown line 'msgunk \"ein?\"'");

            text = "msgid \"test\"\n#| msgunk \"ein?\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown line '#| msgunk \"ein?\"'");

            text = "msgid \"test\"\nunk \"ein?\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown line 'unk \"ein?\"'");

            text = "msgid \"\"\nmsgstr \"\"\n\"#:nospace\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Invalid line format: #:nospace");
        }

        [Test]
        public void ConvertEntryAndHeader()
        {
            var testPo = new Po(new PoHeader("myId", "yo", "es") { CreationDate = "today" });
            testPo.Add(new PoEntry { Original = "original", Translated = "translated" });

            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""PO-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: es\n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""

msgid ""original""
msgstr ""translated""
";
            text = text.Replace("\r\n", "\n");

            var newPo = ConvertStringToPo(text);

            CompareText(ConvertFormat.To<BinaryFormat>(testPo), text);
            Assert.AreEqual(testPo.Header.ProjectIdVersion, newPo.Header.ProjectIdVersion);
            Assert.AreEqual(testPo.Header.ReportMsgidBugsTo, newPo.Header.ReportMsgidBugsTo);
            Assert.AreEqual(testPo.Header.Language, newPo.Header.Language);
            Assert.AreEqual(testPo.Header.CreationDate, newPo.Header.CreationDate);
            Assert.AreEqual(1, newPo.Entries.Count);
            Assert.AreEqual(testPo.Entries[0].Original, newPo.Entries[0].Original);
            Assert.AreEqual(testPo.Entries[0].Translated, newPo.Entries[0].Translated);
        }

        [Test]
        public void HeaderNotFirstPlaceThrowsException()
        {
            string text = @"
msgid ""original""
msgstr ""translated""

msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""PO-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: \n""
""MIME-Version: 1.0\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
";
            text = text.Replace("\r\n", "\n");
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Original field must be filled");
        }

        [Test]
        public void NullArgumentThrowException()
        {
            Po2Binary converter = new Po2Binary();
            Assert.Throws<ArgumentNullException>(() => converter.Convert((Po)null));
        }

        [Test]
        public void NullBinaryThrowException()
        {
            Binary2Po converter = new Binary2Po();
            Assert.Throws<ArgumentNullException>(() => converter.Convert((BinaryFormat)null));
        }

        [Test]
        public void FinalSpacesAreIgnored()
        {
            string text = "msgid \"test\"\n \n \n";
            Po newPo = ConvertStringToPo(text);
            Assert.AreEqual(1, newPo.Entries.Count);
            Assert.AreEqual("test", newPo.Entries[0].Original);
        }

        [Test]
        public void EntryWithMultiLineExtractedComments()
        {
            var testPo = new Po();
            testPo.Add(new PoEntry {
                Original = "The Quick Brown Fox Jumps Over The Lazy Dog",
                ExtractedComments =
                    "TRANSLATORS: A test phrase with all letters of the English alphabet.\n" +
                    "Replace it with a sample text in your language, such that it is\n" +
                    "representative of language's writing system.",
                TranslatorComment =
                    "NOTE: This is a very long comment that I am writting to test if\n" +
                    "this is working properly.",
                Reference = "kdeui/fonts/kfontchooser.cpp:382",
            });

            string text = @"
#  NOTE: This is a very long comment that I am writting to test if
#  this is working properly.
#. TRANSLATORS: A test phrase with all letters of the English alphabet.
#. Replace it with a sample text in your language, such that it is
#. representative of language's writing system.
#: kdeui/fonts/kfontchooser.cpp:382
msgid ""The Quick Brown Fox Jumps Over The Lazy Dog""
msgstr """"
";
            text = text.Replace("\r\n", "\n");

            // Enable after #85: CompareText(testPo.ConvertTo<BinaryFormat>(), text)
            Po newPo = ConvertStringToPo(text);
            Assert.AreEqual(1, newPo.Entries.Count);
            Assert.AreEqual(testPo.Entries[0].Original, newPo.Entries[0].Original);
            Assert.AreEqual(testPo.Entries[0].ExtractedComments, newPo.Entries[0].ExtractedComments);
            Assert.AreEqual(testPo.Entries[0].Reference, newPo.Entries[0].Reference);
        }

        [Test]
        public void StreamNotAtOrigin()
        {
            var header = new PoHeader("testId", "reporter", "es");
            var po = new Po(header);
            var entry = new PoEntry("Test") { Translated = "Prueba" };
            po.Add(entry);

            BinaryFormat poBinary = ConvertFormat.To<BinaryFormat>(po);

            Assert.AreNotEqual(0, poBinary.Stream.Position);

            Po result = ConvertFormat.To<Po>(poBinary);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Entries.Count);
            Assert.AreEqual(po.Entries[0].Original, result.Entries[0].Original);
        }

        static void CompareText(BinaryFormat binary, string expected)
        {
            binary.Stream.Position = 0;
            TextDataReader reader = new TextDataReader(binary.Stream);
            Assert.AreEqual(expected, reader.ReadToEnd());
        }

        static Po ConvertStringToPo(string binary)
        {
            using BinaryFormat textFormat = new BinaryFormat();
            new TextDataWriter(textFormat.Stream).Write(binary);
            textFormat.Stream.Position = 0;

            return ConvertFormat.To<Po>(textFormat);
        }
    }
}
