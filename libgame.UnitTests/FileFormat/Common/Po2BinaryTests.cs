//
// Po2BinaryTests.cs
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
namespace Libgame.UnitTests.FileFormat.Common
{
    using System;
    using Libgame.FileFormat.Common;
    using Libgame.IO;
    using NUnit.Framework;

    [TestFixture]
    public class Po2BinaryTests
    {
        [Test]
        public void ConvertEmptyPo()
        {
            var po = new Po();
            CompareText(po.ConvertTo<BinaryFormat>(), string.Empty);
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
            var po = new Po(new PoHeader("myId", "yo") { CreationDate = "today" });
            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""POT-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: \n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
";

            var newPo = ConvertStringToPo(text);

            CompareText(po.ConvertTo<BinaryFormat>(), text);
            Assert.AreEqual(po.Header.ProjectIdVersion, newPo.Header.ProjectIdVersion);
            Assert.AreEqual(po.Header.ReportMsgidBugsTo, newPo.Header.ReportMsgidBugsTo);
            Assert.AreEqual(po.Header.CreationDate, newPo.Header.CreationDate);
            Assert.IsEmpty(newPo.Header.LastTranslator);
            Assert.IsEmpty(newPo.Entries);
        }

        [Test]
        public void ConvertFullHeaderNoEntries()
        {
            var header = new PoHeader("myId", "yo") {
                CreationDate = "today",
                RevisionDate = "tomorrow",
                LastTranslator = "she",
                LanguageTeam = "bestteam",
                Language = "SC",
                PluralForms = "pl"
            };
            var testPo = new Po(header);

            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""POT-Revision-Date: tomorrow\n""
""Last-Translator: she\n""
""Language-Team: bestteam\n""
""Language: SC\n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
""Plural-Forms: pl\n""
";

            var newPo = ConvertStringToPo(text);
            var newHeader = newPo.Header;

            CompareText(testPo.ConvertTo<BinaryFormat>(), text);
            Assert.AreEqual(header.ProjectIdVersion, newHeader.ProjectIdVersion);
            Assert.AreEqual(header.ReportMsgidBugsTo, newHeader.ReportMsgidBugsTo);
            Assert.AreEqual(header.CreationDate, newHeader.CreationDate);
            Assert.AreEqual(header.RevisionDate, newHeader.RevisionDate);
            Assert.AreEqual(header.LastTranslator, newHeader.LastTranslator);
            Assert.AreEqual(header.LanguageTeam, newHeader.LanguageTeam);
            Assert.AreEqual(header.Language, newHeader.Language);
            Assert.AreEqual(header.ContentType, newHeader.ContentType);
            Assert.AreEqual(header.ContentTransferEncoding, newHeader.ContentTransferEncoding);
            Assert.AreEqual(header.PluralForms, newHeader.PluralForms);
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
        }

        [Test]
        public void ThrowIfUnknownHeaderField()
        {
            string text = "msgid \"\"\nmsgstr \"\"\n\"Unknown: hehe\"";
            Assert.Throws<FormatException>(
                () => ConvertStringToPo(text),
                "Unknown header: Unknown: hehe");
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

            var newPo = ConvertStringToPo(text);

            CompareText(testPo.ConvertTo<BinaryFormat>(), text);
            Assert.IsNull(newPo.Header);
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
                Context = "a ctx"
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

            var newPo = ConvertStringToPo(text);

            CompareText(testPo.ConvertTo<BinaryFormat>(), text);
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
                Translated = "trans\nl\"a\"ted"
            });

            string text = @"
msgid ""original""
msgstr """"
""trans\n""
""l\""a\""ted""
";

            var newPo = ConvertStringToPo(text);

            CompareText(testPo.ConvertTo<BinaryFormat>(), text);
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
        }

        [Test]
        public void ConvertEntryAndHeader()
        {
            var testPo = new Po(new PoHeader("myId", "yo") { CreationDate = "today" });
            testPo.Add(new PoEntry { Original = "original", Translated = "translated" });

            string text = @"msgid """"
msgstr """"
""Project-Id-Version: myId\n""
""Report-Msgid-Bugs-To: yo\n""
""POT-Creation-Date: today\n""
""POT-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: \n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""

msgid ""original""
msgstr ""translated""
";

            var newPo = ConvertStringToPo(text);

            CompareText(testPo.ConvertTo<BinaryFormat>(), text);
            Assert.AreEqual(testPo.Header.ProjectIdVersion, newPo.Header.ProjectIdVersion);
            Assert.AreEqual(testPo.Header.ReportMsgidBugsTo, newPo.Header.ReportMsgidBugsTo);
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
""POT-Revision-Date: \n""
""Last-Translator: \n""
""Language-Team: \n""
""Language: \n""
""Content-Type: text/plain; charset=UTF-8\n""
""Content-Transfer-Encoding: 8bit\n""
";

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
            Po2Binary converter = new Po2Binary();
            Assert.Throws<ArgumentNullException>(() => converter.Convert((BinaryFormat)null));
        }

        static void CompareText(BinaryFormat binary, string expected)
        {
            binary.Stream.Position = 0;
            TextReader reader = new TextReader(binary.Stream);
            Assert.AreEqual(expected, reader.ReadToEnd());
        }

        static Po ConvertStringToPo(string binary)
        {
            BinaryFormat textFormat = new BinaryFormat();
            new TextWriter(textFormat.Stream).Write(binary);
            textFormat.Stream.Position = 0;

            return textFormat.ConvertTo<Po>();
        }
    }
}
