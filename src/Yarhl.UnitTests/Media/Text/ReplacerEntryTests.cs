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
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class ReplacerEntryTests
    {
        [Test]
        public void EqualsObjReturnsTrueWhenEqual()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            object entry2 = new ReplacerEntry("o", "m");
            Assert.That(entry1.Equals(entry2), Is.True);
        }

        [Test]
        public void EqualsObjReturnsFalseWhenDifferent()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            object entry2 = new ReplacerEntry("o", "t");
            Assert.That(entry1.Equals(entry2), Is.False);

            object entry3 = new ReplacerEntry("t", "m");
            Assert.That(entry1.Equals(entry3), Is.False);
        }

        [Test]
        public void EqualsObjReturnsFalseWhenDifferentType()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            Assert.That(entry1.Equals(5), Is.False);
        }

        [Test]
        public void EqualsReturnsTrueWhenEqual()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            ReplacerEntry entry2 = new ReplacerEntry("o", "m");
            Assert.That(entry1.Equals(entry2), Is.True);
        }

        [Test]
        public void EqualsReturnsFalseWhenDifferent()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            ReplacerEntry entry2 = new ReplacerEntry("o", "t");
            Assert.That(entry1.Equals(entry2), Is.False);

            ReplacerEntry entry3 = new ReplacerEntry("t", "m");
            Assert.That(entry1.Equals(entry3), Is.False);
        }

        [Test]
        public void EqualityIsLikeEquals()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            ReplacerEntry entry2 = new ReplacerEntry("o", "t");
            ReplacerEntry entry3 = new ReplacerEntry("t", "m");
            ReplacerEntry entry4 = new ReplacerEntry("o", "m");
            Assert.That(entry1 == entry4, Is.True);
            Assert.That(entry1 != entry4, Is.False);
            Assert.That(entry1 == entry2, Is.False);
            Assert.That(entry1 != entry2, Is.True);
            Assert.That(entry1 == entry3, Is.False);
            Assert.That(entry1 != entry3, Is.True);
        }

        [Test]
        public void HashCodeDependsOnFields()
        {
            ReplacerEntry entry1 = new ReplacerEntry("o", "m");
            ReplacerEntry entry2 = new ReplacerEntry("o", "t");
            ReplacerEntry entry3 = new ReplacerEntry("t", "m");
            ReplacerEntry entry4 = new ReplacerEntry("o", "m");
            Assert.That(entry1.GetHashCode(), Is.EqualTo(entry4.GetHashCode()));
            Assert.That(entry1.GetHashCode(), Is.Not.EqualTo(entry2.GetHashCode()));
            Assert.That(entry1.GetHashCode(), Is.Not.EqualTo(entry3.GetHashCode()));
        }
    }
}
