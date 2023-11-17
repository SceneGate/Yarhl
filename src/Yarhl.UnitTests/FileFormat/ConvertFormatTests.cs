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
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public partial class ConvertFormatTests
    {
        [Test]
        public void ConvertWithType()
        {
            using var format = new StringFormatTest("3");
            Assert.That(
                ConvertFormat.With(typeof(FormatTestDuplicatedConverter2), format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithTypeThrowsIfTypeIsNull()
        {
            using var format = new StringFormatTest("3");
            Type type = null;
            Assert.That(
                () => ConvertFormat.With(type, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithTypeCouldWorkWithNullFormats()
        {
            Assert.That(ConvertFormat.With(typeof(NullConverter), null), Is.Null);
        }

        [Test]
        public void ConvertWithTypeThrowsIfConstructorFails()
        {
            using var test = new StringFormatTest { Value = "3" };
            Assert.Throws<TargetInvocationException>(() =>
                ConvertFormat.With(typeof(FormatTestBadConstructor), test));

            // Just for coverage
            var converter = new FormatTestBadConstructor("2");
            Assert.That(
                converter.Convert(new StringFormatTest("3")),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithTypeThrowsIfNoImplementIConverter()
        {
            using var format = new StringFormatTest("3");
            Assert.That(
                () => ConvertFormat.With(typeof(DateTime), format),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Converter doesn't implement IConverter<,>"));
        }

        [Test]
        public void ConvertWithTypeThrowsExceptionIfInvalidConverter()
        {
            using var format = new StringFormatTest("3");
            string msg = "Converter cannot convert the type: Yarhl.UnitTests.FileFormat.StringFormatTest";

            Assert.That(
                () => ConvertFormat.With(typeof(SingleOuterConverterExample), format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
        }
    }
}
