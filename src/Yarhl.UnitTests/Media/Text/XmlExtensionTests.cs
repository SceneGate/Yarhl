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
    using System.Xml.Linq;
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class XmlExtensionTests
    {
        XElement entry;

        [OneTimeSetUp]
        public void SetUp()
        {
            entry = new XElement("test");
        }

        [Test]
        public void SingleLinesAreNotIndented()
        {
            entry.SetIndentedValue("Single line", 2);
            Assert.AreEqual("Single line", entry.Value);
        }

        [Test]
        public void MultiLinesAreIndented()
        {
            entry.SetIndentedValue("Multi\nlines", 2);
            Assert.AreEqual("\n    Multi\n    lines\n  ", entry.Value);
        }

        [Test]
        public void SettingNullEntryThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => ((XElement)null).SetIndentedValue("test", 2));
        }

        [Test]
        public void SettingNullValueThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => entry.SetIndentedValue(null, 2));
        }

        [Test]
        public void SettingNegativeIndentationThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => entry.SetIndentedValue(string.Empty, -1));
        }

        [Test]
        public void GettingNullEntryThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () => ((XElement)null).GetIndentedValue());
        }

        [Test]
        public void ZeroIdentation()
        {
            TestIndentation("test", "test", 0);
            TestIndentation("test\nhey", "\ntest\nhey\n  ", 0);
        }

        [Test]
        public void DoNotReplaceMiddleDuplicateSpaces()
        {
            TestIndentation("test  hey", "test  hey");
            TestIndentation("test   hey", "test   hey");
            TestIndentation("test     hey", "test     hey");
            TestIndentation("test  hey\nhey  test", "\n    test  hey\n    hey  test\n  ");
        }

        [Test]
        public void IgnoreSpaces()
        {
            entry.SetAttributeValue("ignoreSpaces", true);
            TestIndentation(" test  hey ", " test  hey ");
            TestIndentation("test\nhey", "\ntest\nhey\n  ");
            TestIndentation(" hehe  \n   hey  test   ", "\n hehe  \n   hey  test   \n  ");
            entry.SetAttributeValue("ignoreSpaces", false);
        }

        [Test]
        public void ReplaceAdditionalStartingSpacesIfNewLines()
        {
            TestIndentation(" test", " test");
            TestIndentation("  test", "  test");
            TestIndentation(" ", " ");
            TestIndentation(" test\n  hey", "\n    {!SP}test\n    {!SP}{!SP}hey\n  ");
            TestIndentation(" test\nhey  test", "\n    {!SP}test\n    hey  test\n  ");
            TestIndentation(" \n ", "\n    {!SP}\n    {!SP}\n  ");
            TestGetting(" test", " test");
            TestGetting(" test\n  hey  test\n \n", "test\nhey  test\n");
            TestGetting("\n     t\n      a\n  ", "t\na");
        }

        [Test]
        public void ReplaceAdditionalTrailingSpacesIfNewLines()
        {
            TestIndentation("test ", "test ");
            TestIndentation("test  ", "test  ");
            TestIndentation("test \nhey  ", "\n    test{!SP}\n    hey{!SP}{!SP}\n  ");
            TestIndentation(" test \n hey \n", "\n    {!SP}test{!SP}\n    {!SP}hey{!SP}\n    \n  ");
            TestGetting("test ", "test ");
            TestGetting("test \nhey  \na  b   \n \n", "test\nhey\na  b\n");
            TestGetting("\n    t \n    a  \n  ", "t\na");
            TestGetting(" \n a\n  ", "a");
        }

        [Test]
        public void JustNewLineOrEmptyLine()
        {
            TestIndentation(string.Empty, string.Empty);
            TestIndentation("\n", "\n    \n    \n  ");
        }

        [Test]
        public void RelatedSpaces()
        {
            TestIndentation("hey\vhey\nhehe", "\n    hey[@!!0B]hey\n    hehe\n  ");
        }

        void TestIndentation(string original, string transformed, int indent = 2)
        {
            entry.SetIndentedValue(original, indent);
            Assert.AreEqual(transformed, entry.Value);
            Assert.AreEqual(original, entry.GetIndentedValue());
        }

        void TestGetting(string transformed, string original)
        {
            entry.Value = transformed;
            Assert.AreEqual(original, entry.GetIndentedValue());
        }
    }
}
