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
        public void ConvertBasicHeaderNoEntries()
        {
            var po = new Po(new PoHeader("myId", "yo") { CreationDate = "today" });
            string expected = @"msgid """"
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
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
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

            var po = new Po(header);

            string expected = @"msgid """"
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
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
        }

        [Test]
        public void ConvertNoHeaderEntries()
        {
            var po = new Po();
            po.Add(new PoEntry { Original = "original" });
            po.Add(new PoEntry { Original = "totranslate", Translated = "translated" });

            string expected = @"
msgid ""original""
msgstr """"

msgid ""totranslate""
msgstr ""translated""
";
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
        }

        [Test]
        public void ConvertFullEntry()
        {
            var po = new Po();
            po.Add(new PoEntry {
                Original = "original",
                Translated = "translated",
                TranslatorComment = "a comment",
                ExtractedComments = "hehe",
                Reference = "ref1",
                PreviousContext = "prev ctx",
                PreviousOriginal = "prev org",
                Context = "a ctx"
            });

            string expected = @"
#  a comment
#. hehe
#: ref1
#| msgctxt prev ctx
#| msgid prev org
msgctxt ""a ctx""
msgid ""original""
msgstr ""translated""
";
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
        }

        [Test]
        public void ConvertSpecialEntry()
        {
            var po = new Po();
            po.Add(new PoEntry { Original = "original", Translated = "trans\nl\"a\"ted" });

            string expected = @"
msgid ""original""
msgstr """"
""trans\n""
""l\""a\""ted""
";
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
        }

        [Test]
        public void ConvertEntryAndHeader()
        {
            var po = new Po(new PoHeader("myId", "yo") { CreationDate = "today" });
            po.Add(new PoEntry { Original = "original", Translated = "translated" });

            string expected = @"msgid """"
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
            CompareText(po.ConvertTo<BinaryFormat>(), expected);
        }

        [Test]
        public void NullArgumentThrowException()
        {
            Po2Binary converter = new Po2Binary();
            Assert.Throws<ArgumentNullException>(() => converter.Convert((Po)null));
        }

        void CompareText(BinaryFormat binary, string expected)
        {
            binary.Stream.Position = 0;
            DataReader reader = new DataReader(binary.Stream);
            Assert.AreEqual(expected, reader.ReadString((int)binary.Stream.Length));
        }
    }
}
