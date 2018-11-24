//
// ConverterMetadataTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2018 Benito Palacios Sánchez
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
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class ConverterMetadataTests
    {
        [Test]
        public void GetSourcesReturnOneElementArrayForSingleSource()
        {
            var metadata = new ConverterMetadata {
                Sources = typeof(int)
            };
            Type[] sources = metadata.GetSources();
            Assert.That(sources.Length, Is.EqualTo(1));
            Assert.That(sources[0], Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetSourcesReturnTwoElementsArrayForListOfSources()
        {
            var metadata = new ConverterMetadata {
                Sources = new Type[] { typeof(int), typeof(string) }
            };
            Type[] sources = metadata.GetSources();
            Assert.That(sources.Length, Is.EqualTo(2));
            Assert.That(sources[0], Is.EqualTo(typeof(int)));
            Assert.That(sources[1], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GetSourcesReturnEmptyArrayForDefaultValue()
        {
            var metadata = new ConverterMetadata();
            Assert.That(metadata.GetSources(), Is.Empty);
        }

        [Test]
        public void GetDestinationsReturnOneElementArrayForSingleSource()
        {
            var metadata = new ConverterMetadata {
                Destinations = typeof(int)
            };
            Type[] dests = metadata.GetDestinations();
            Assert.That(dests.Length, Is.EqualTo(1));
            Assert.That(dests[0], Is.EqualTo(typeof(int)));
        }

        [Test]
        public void GetDestinationsReturnTwoElementsArrayForListOfSources()
        {
            var metadata = new ConverterMetadata {
                Destinations = new Type[] { typeof(int), typeof(string) }
            };
            Type[] dests = metadata.GetDestinations();
            Assert.That(dests.Length, Is.EqualTo(2));
            Assert.That(dests[0], Is.EqualTo(typeof(int)));
            Assert.That(dests[1], Is.EqualTo(typeof(string)));
        }

        [Test]
        public void GetDestinationsReturnEmptyArrayForDefaultValue()
        {
            var metadata = new ConverterMetadata();
            Assert.That(metadata.GetDestinations(), Is.Empty);
        }

        [Test]
        public void CanConvertSourceThrowsExceptionIfNullArgument()
        {
            var metadata = new ConverterMetadata {
                Sources = new Type[] { typeof(int), typeof(string) }
            };
            Assert.That(
                () => metadata.CanConvert(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void CanConvertReturnsTrueForExactType()
        {
            var metadata = new ConverterMetadata {
                Sources = typeof(int)
            };
            Assert.That(metadata.CanConvert(typeof(int)), Is.True);
        }

        [Test]
        public void CanConvertReturnsTrueForTypeInList()
        {
            var metadata = new ConverterMetadata {
                Sources = new[] { typeof(string), typeof(int) }
            };
            Assert.That(metadata.CanConvert(typeof(int)), Is.True);
        }

        [Test]
        public void CanConvertReturnsTrueForDerivedTypes()
        {
            var metadata = new ConverterMetadata {
                Sources = typeof(Base)
            };
            Assert.That(metadata.CanConvert(typeof(Derived)), Is.True);
        }

        [Test]
        public void CanConvertReturnsFalseForDifferentTypes()
        {
            var metadata = new ConverterMetadata {
                Sources = new[] { typeof(string), typeof(int) }
            };
            Assert.That(metadata.CanConvert(typeof(DateTime)), Is.False);
        }

        [Test]
        public void CanConvertReturnsForExactSourceAndDest()
        {
            var metadata = new ConverterMetadata {
                Sources = typeof(int),
                Destinations = typeof(string)
            };
            Assert.That(
                metadata.CanConvert(typeof(int), typeof(string)),
                Is.True);
            Assert.That(
                metadata.CanConvert(typeof(string), typeof(string)),
                Is.False);
            Assert.That(
                metadata.CanConvert(typeof(int), typeof(int)),
                Is.False);
        }

        [Test]
        public void CanConvertReturnsForSourceAndDestInSameOrderList()
        {
            var metadata = new ConverterMetadata {
                Sources = new[] { typeof(int), typeof(DateTime) },
                Destinations = new[] { typeof(string), typeof(sbyte) }
            };
            Assert.That(
                metadata.CanConvert(typeof(DateTime), typeof(sbyte)),
                Is.True);
            Assert.That(
                metadata.CanConvert(typeof(DateTime), typeof(string)),
                Is.False);
        }

        [Test]
        public void CanConvertReturnsForSourceAndDestDerived()
        {
            var metadata = new ConverterMetadata {
                Sources = new[] { typeof(Base) },
                Destinations = new[] { typeof(Derived) }
            };
            Assert.That(
                metadata.CanConvert(typeof(Derived), typeof(Base)),
                Is.True);

            metadata = new ConverterMetadata {
                Sources = new[] { typeof(Derived) },
                Destinations = new[] { typeof(Base) }
            };
            Assert.That(
                metadata.CanConvert(typeof(Base), typeof(Base)),
                Is.False);
            Assert.That(
                metadata.CanConvert(typeof(Derived), typeof(Derived)),
                Is.False);
        }

        [Test]
        public void CanConvertSourceDestThrowsExceptionIfNullArgument()
        {
            var metadata = new ConverterMetadata {
                Sources = new Type[] { typeof(int), typeof(string) },
                Destinations = new Type[] { typeof(int), typeof(string) }
            };
            Assert.That(
                () => metadata.CanConvert(null, typeof(int)),
                Throws.ArgumentNullException);
            Assert.That(
                () => metadata.CanConvert(typeof(int), null),
                Throws.ArgumentNullException);
        }

        class Base
        {
        }

        class Derived : Base
        {
        }
    }
}
