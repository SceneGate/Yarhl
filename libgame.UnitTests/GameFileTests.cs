//
// GameFileTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
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
namespace Libgame.UnitTests
{
    using System;
    using Mono.Addins;
    using NUnit.Framework;
    using Libgame.FileFormat;
    using FileFormat;

    [TestFixture]
    class GameFileTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(".addins");
                AddinManager.Registry.Update();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (AddinManager.IsInitialized)
                AddinManager.Shutdown();
        }

        [Test]
        public void ConstructorSetName()
        {
            GameFile file = new GameFile("mytest");
            Assert.AreEqual("mytest", file.Name);
        }

        [Test]
        public void ConstructorSetNameAndFormat()
        {
            Format dummyFormat = new StringFormatTest("");
            GameFile file = new GameFile("mytest", dummyFormat);
            Assert.AreEqual("mytest", file.Name);
            Assert.AreSame(dummyFormat, file.Format);
        }

        [Test]
        public void ConstructorFormatInHistory()
        {
            Format dummyFormat = new StringFormatTest("");
            GameFile file = new GameFile("mytest", dummyFormat);
            Assert.AreEqual(1, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat, file.FormatHistory[0]);
        }

        [Test]
        public void ConstructorWithoutFormatEmptyHistory()
        {
            GameFile file = new GameFile("mytest");
            Assert.AreEqual(0, file.FormatHistory.Count);
        }

        [Test]
        public void ConstructorFormatInProperty()
        {
            Format dummyFormat = new StringFormatTest("");
            GameFile file = new GameFile("mytest", dummyFormat);
            Assert.AreSame(dummyFormat, file.Format);
        }

        [Test]
        public void ConstructorWithoutFormatNullProperty()
        {
            GameFile file = new GameFile("mytest");
            Assert.IsNull(file.Format);
        }

        [Test]
        public void TransformChangeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformTo<IntFormatTest>();
            Assert.IsInstanceOf<IntFormatTest>(file.Format);
            Assert.AreNotSame(dummyFormat, file.Format);
            Assert.AreEqual(3, (file.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformAddFormatToHistory()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformTo<IntFormatTest>();
            Assert.AreEqual(2, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat, file.FormatHistory[0]);
            Assert.IsInstanceOf<IntFormatTest>(file.FormatHistory[1]);
            Assert.AreEqual(3, (file.FormatHistory[1] as IntFormatTest).Value);
        }

        [Test]
        public void TransformNoFormatFileThrowException()
        {
            GameFile file = new GameFile("mytest");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                file.TransformTo<IntFormatTest>());
            Assert.AreEqual("Cannot transform a file without format", ex.Message);
        }

        [Test]
        public void TransformReturnsItself()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            Assert.AreSame(file, file.TransformTo<IntFormatTest>());
        }

        [Test]
        public void TransformConcatenating()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformTo<IntFormatTest>().TransformTo<StringFormatTest>();
            Assert.IsInstanceOf<StringFormatTest>(file.Format);
            Assert.AreNotSame(dummyFormat, file.Format);
            Assert.AreEqual("3", (file.Format as StringFormatTest).Value);

            Assert.AreEqual(3, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat, file.FormatHistory[0]);
            Assert.IsInstanceOf<IntFormatTest>(file.FormatHistory[1]);
            Assert.IsInstanceOf<StringFormatTest>(file.FormatHistory[2]);
            Assert.AreEqual(3, (file.FormatHistory[1] as IntFormatTest).Value);
            Assert.AreEqual("3", (file.FormatHistory[2] as StringFormatTest).Value);
        }

        [Test]
        public void TransformWithChangeFormat()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformWith<IntFormatTest>(converter);
            Assert.IsInstanceOf<IntFormatTest>(file.Format);
            Assert.AreNotSame(dummyFormat, file.Format);
            Assert.AreEqual(4, (file.Format as IntFormatTest).Value);
        }

        [Test]
        public void TransformWithAddFormatToHistory()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformWith<IntFormatTest>(converter);
            Assert.AreEqual(2, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat, file.FormatHistory[0]);
            Assert.IsInstanceOf<IntFormatTest>(file.FormatHistory[1]);
            Assert.AreEqual(4, (file.FormatHistory[1] as IntFormatTest).Value);
        }

        [Test]
        public void TransformmWithAndNoFormatFileThrowException()
        {
            PrivateConverter converter = new PrivateConverter();
            GameFile file = new GameFile("mytest");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                file.TransformWith<IntFormatTest>(converter));
            Assert.AreEqual("Cannot transform a file without format", ex.Message);
        }

        [Test]
        public void TransformWithReturnsItself()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            Assert.AreSame(file, file.TransformWith<IntFormatTest>(converter));
        }

        [Test]
        public void TransformWithConcatenating()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);

            file.TransformWith<IntFormatTest>(converter)
                .TransformWith<StringFormatTest>(converter);
            Assert.IsInstanceOf<StringFormatTest>(file.Format);
            Assert.AreNotSame(dummyFormat, file.Format);
            Assert.AreEqual("4", (file.Format as StringFormatTest).Value);

            Assert.AreEqual(3, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat, file.FormatHistory[0]);
            Assert.IsInstanceOf<IntFormatTest>(file.FormatHistory[1]);
            Assert.IsInstanceOf<StringFormatTest>(file.FormatHistory[2]);
            Assert.AreEqual(4, (file.FormatHistory[1] as IntFormatTest).Value);
            Assert.AreEqual("4", (file.FormatHistory[2] as StringFormatTest).Value);
        }

        [Test]
        public void SetFormatChangeFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.SetFormat(dummyFormat2, false);
            Assert.AreNotSame(file.Format, dummyFormat1);
            Assert.AreSame(file.Format, dummyFormat2);
        }

        [Test]
        public void SetFormatForFilesWithoutFormatChangeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest");
            file.SetFormat(dummyFormat, false);
            Assert.AreSame(file.Format, dummyFormat);
        }

        [Test]
        public void SetFormatCleanPreviousFormats()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.SetFormat(dummyFormat2, false);
            Assert.AreEqual(1, file.FormatHistory.Count);
            Assert.AreSame(dummyFormat2, file.FormatHistory[0]);
        }

        [Test]
        public void SetFormatWithDisposePreviousFormats()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.SetFormat(dummyFormat2, true);
            Assert.IsTrue(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void SetFormatWithNotDisposePreviousFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.SetFormat(dummyFormat2, false);
            Assert.IsFalse(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void SetFormatWithNullFormats()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.SetFormat(null, false);
            Assert.IsEmpty(file.FormatHistory);
            Assert.IsNull(file.Format);
        }

        [Test]
        public void CleanFormatsCleanHistory()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.CleanFormats(false);
            Assert.IsEmpty(file.FormatHistory);
            Assert.IsNull(file.Format);
        }

        [Test]
        public void CleanFormatsWithDisposeFormats()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.CleanFormats(true);
            Assert.IsTrue(dummyFormat.Disposed);
        }

        [Test]
        public void CleanFormatsWithNotDisposeFormats()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.CleanFormats(false);
            Assert.IsFalse(dummyFormat.Disposed);
        }

        public class PrivateConverter : 
            IConverter<StringFormatTest, IntFormatTest>,
            IConverter<IntFormatTest, StringFormatTest>
        {
            public IntFormatTest Convert(StringFormatTest test)
            {
                return new IntFormatTest(System.Convert.ToInt32(test.Value) + 1);
            }

            public StringFormatTest Convert(IntFormatTest test)
            {
                return new StringFormatTest(test.Value.ToString());
            }
        }
    }
}
