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
namespace Libgame.UnitTests.FileSystem
{
    using System;
    using Mono.Addins;
    using NUnit.Framework;
    using Libgame.FileFormat;
    using Libgame.FileSystem;
    using FileFormat;

    [TestFixture]
    public class GameFileTests
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
        }

        [Test]
        public void TransformDisposeFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.TransformTo<IntFormatTest>();
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(file.Format.Disposed);
        }

        [Test]
        public void TransformNotDisposingFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.TransformTo<IntFormatTest>(false);
            Assert.IsFalse(dummyFormat.Disposed);
            Assert.IsFalse(file.Format.Disposed);
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
        }

        [Test]
        public void TransformWithDisposeFormat()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.TransformWith<IntFormatTest>(converter);
            Assert.IsTrue(dummyFormat.Disposed);
            Assert.IsFalse(file.Format.Disposed);
        }

        [Test]
        public void TransformWithNotDisposingFormat()
        {
            PrivateConverter converter = new PrivateConverter();
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.TransformWith<IntFormatTest>(converter, false);
            Assert.IsFalse(dummyFormat.Disposed);
            Assert.IsFalse(file.Format.Disposed);
        }

        [Test]
        public void SetFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.Format = dummyFormat2;
            Assert.AreNotSame(file.Format, dummyFormat1);
            Assert.AreSame(file.Format, dummyFormat2);
        }

        [Test]
        public void SetFormatWithoutPreviousFormat()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest");
            file.Format = dummyFormat;
            Assert.AreSame(file.Format, dummyFormat);
        }

        [Test]
        public void SetFormatDoesNotDisposePreviousFormat()
        {
            Format dummyFormat1 = new StringFormatTest("3");
            Format dummyFormat2 = new IntFormatTest(4);
            GameFile file = new GameFile("mytest", dummyFormat1);
            file.Format = dummyFormat2;
            Assert.IsFalse(dummyFormat1.Disposed);
            Assert.IsFalse(dummyFormat2.Disposed);
        }

        [Test]
        public void SetFormatToNull()
        {
            Format dummyFormat = new StringFormatTest("3");
            GameFile file = new GameFile("mytest", dummyFormat);
            file.Format = null;
            Assert.IsNull(file.Format);
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
