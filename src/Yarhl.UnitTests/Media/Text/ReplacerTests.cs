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
    using System.Collections.Generic;
    using NUnit.Framework;
    using Yarhl.Media.Text;

    [TestFixture]
    public class ReplacerTests
    {
        [Test]
        public void MapsAreNotNull()
        {
            Replacer map = new Replacer();
            Assert.That(map.Map, Is.Not.Null);
        }

        [Test]
        public void SetAddsCharToMap()
        {
            Replacer map = new Replacer();
            Assert.That(map.Map.Count, Is.EqualTo(0));

            map.Add("a", "b");
            Assert.That(map.Map.Count, Is.EqualTo(1));
            Assert.That(map.Map[0].Original, Is.EqualTo("a"));
            Assert.That(map.Map[0].Modified, Is.EqualTo("b"));
        }

        [Test]
        public void SetReplacesExistingEntry()
        {
            Replacer map = new Replacer();
            map.Add("a", "b");
            map.Add("a", "c");

            Assert.That(map.Map.Count, Is.EqualTo(1));
            Assert.That(map.Map[0].Original, Is.EqualTo("a"));
            Assert.That(map.Map[0].Modified, Is.EqualTo("c"));
        }

        [Test]
        public void SetThrowsIfArgumentsAreNullOrEmpty()
        {
            Replacer map = new Replacer();
            Assert.That(() => map.Add(null, "a"), Throws.ArgumentNullException);
            Assert.That(() => map.Add("a", null), Throws.ArgumentNullException);
            Assert.That(() => map.Add(string.Empty, "a"), Throws.ArgumentNullException);
            Assert.That(() => map.Add("a", string.Empty), Throws.ArgumentNullException);
        }

        [Test]
        public void RemoveMapEntry()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");
            Assert.That(replacer.Map.Count, Is.EqualTo(1));
            replacer.Remove("a");
            Assert.That(replacer.Map.Count, Is.EqualTo(0));
        }

        [Test]
        public void RemoveMapEntryEmpty()
        {
            Replacer replacer = new Replacer();
            Assert.That(
                () => replacer.Remove("a"),
                Throws.InstanceOf<KeyNotFoundException>());
        }

        [Test]
        public void RemoveFakeMapEntry()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");
            Assert.That(
                () => replacer.Remove("c"),
                Throws.InstanceOf<KeyNotFoundException>());
            Assert.That(
                () => replacer.Remove("b"),
                Throws.InstanceOf<KeyNotFoundException>());
        }

        [Test]
        public void RemoveEmptyMapEntry()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");
            Assert.That(
                () => replacer.Remove(string.Empty),
                Throws.ArgumentNullException);
            Assert.That(
                () => replacer.Remove(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void TransformChars()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "z");
            replacer.Add("b", "y");

            Assert.That(replacer.TransformForward("ababtr"), Is.EqualTo("zyzytr"));
            Assert.That(replacer.TransformBackward("zyzytr"), Is.EqualTo("ababtr"));
        }

        [Test]
        public void TransformStrings()
        {
            Replacer replacer = new Replacer();
            replacer.Add("<br/>", "\n");
            replacer.Add("z", ".");

            Assert.That(replacer.TransformForward("zz<br/>zz"), Is.EqualTo("..\n.."));
            Assert.That(replacer.TransformBackward("..\n.."), Is.EqualTo("zz<br/>zz"));
        }

        [Test]
        public void TransformWithMultipleSourceMatchesTakesOrder()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "z");
            replacer.Add("ab", "y");

            Assert.That(replacer.TransformForward("aabaa"), Is.EqualTo("zyzz"));
            Assert.That(replacer.TransformBackward("zyzz"), Is.EqualTo("aabaa"));

            replacer = new Replacer();
            replacer.Add("ab", "y");
            replacer.Add("a", "z");

            Assert.That(replacer.TransformForward("aabaa"), Is.EqualTo("zzbzz"));
            Assert.That(replacer.TransformBackward("zzbzz"), Is.EqualTo("aabaa"));
        }

        [Test]
        public void TransformWithModifiedInOriginal()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");
            replacer.Add("b", "c");

            Assert.That(replacer.TransformForward("aabaa"), Is.EqualTo("bbcbb"));
            Assert.That(replacer.TransformBackward("bbcbb"), Is.EqualTo("aabaa"));

            replacer.Add("b", "a");

            Assert.That(replacer.TransformForward("aabaa"), Is.EqualTo("bbabb"));
            Assert.That(replacer.TransformBackward("bbabb"), Is.EqualTo("aabaa"));
        }

        [Test]
        public void TransformWithoutMapReturnsOriginal()
        {
            Replacer replacer = new Replacer();
            Assert.That(replacer.TransformForward("abc"), Is.EqualTo("abc"));
        }

        [Test]
        public void TransformEmptyLineDoesNotThrowException()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");

            Assert.That(replacer.TransformForward(string.Empty), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TransformNullThrowException()
        {
            Replacer replacer = new Replacer();
            replacer.Add("a", "b");

            Assert.That(() => replacer.TransformForward(null), Throws.ArgumentNullException);
        }
    }
}