// FormatConversionTests.cs
//
// Copyright (c) 2019 SceneGate Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using System.Composition;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class FormatConversionTests
    {
        [Test]
        public void GenericConvertToConverts()
        {
            Assert.AreEqual(FormatConversion.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void TypedArgConvertToConverts()
        {
            Assert.That(FormatConversion.ConvertTo<int, string>("3"), Is.EqualTo(3));
        }

        [Test]
        public void TypedArgConvertThrowsIfNull()
        {
            Assert.That(
                () => FormatConversion.ConvertTo<int, string>(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToConverts()
        {
            Assert.AreEqual(FormatConversion.ConvertTo(typeof(int), "3"), 3);
            Assert.AreEqual(FormatConversion.ConvertTo(typeof(string), 3), "3");
        }

        [Test]
        public void ConvertToThrowsIfTypeIsNull()
        {
            Type dstType = null;
            Assert.That(
                () => FormatConversion.ConvertTo(dstType, "3"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfSrcIsNull()
        {
            Assert.That(
                () => FormatConversion.ConvertTo(typeof(int), null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfThereAreTwoEqualConverters()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                FormatConversion.ConvertTo(typeof(short), test));
            Assert.AreEqual(
                "Multiple converters for: " +
                "Yarhl.UnitTests.FileFormat.StringFormatTest -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertToThrowsIfNoConverter()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                FormatConversion.ConvertTo(typeof(short), (short)3));
            Assert.AreEqual(
                "Cannot find converter for: System.Int16 -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertToThrowsIfConstructorFails()
        {
            var test = new StringFormatTest { Value = "3" };
            var ex = Assert.Throws<Exception>(() =>
                FormatConversion.ConvertTo(typeof(ushort), test));
            Assert.AreEqual(
                "Exception of type 'System.Exception' was thrown.",
                ex.Message);

            // Just for coverage
            var converter = new FormatTestBadConstructor("2");
            Assert.That(
                converter.Convert(new StringFormatTest("3")),
                Is.EqualTo(0));
        }

        [Test]
        public void ConvertNeedsToBeHiddenIfConstructorsHaveArgs()
        {
            // With MEF we can't have an extension without a default constructor
            // because it will throw an exception in every general request.
            // So we need to hide those extensions.
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                FormatConversion.ConvertTo(typeof(long), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                "Yarhl.UnitTests.FileFormat.StringFormatTest -> System.Int64",
                ex.Message);

            // But we can use the ConvertWith
            var converter = new FormatTestNoConstructor("3");
            Assert.AreEqual(
                FormatConversion.ConvertWith(converter, new StringFormatTest("1")),
                0);
        }

        [Test]
        public void ConvertNeedsToBeHiddenIfNoPublicConstructor()
        {
            // With MEF we can't have an extension without a default constructor
            // because it will throw an exception in every general request.
            // So we need to hide those extensions.
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                FormatConversion.ConvertTo(typeof(ulong), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                "Yarhl.UnitTests.FileFormat.StringFormatTest -> System.UInt64",
                ex.Message);

            // But we can use the ConvertWith of classes with Factory pattern.
            var converter = FormatTestPrivateConstructor.Create();
            Assert.AreEqual(
                FormatConversion.ConvertWith(converter, new StringFormatTest("1")),
                0);
        }

        [Test]
        public void ConvertToBaseWithDerivedConverter()
        {
            // It should use the converter: ushort -> Derived
            // The converter will generate a derived type and will cast-down
            // to base.
            Base val = null;
            Assert.DoesNotThrow(() => val = FormatConversion.ConvertTo<Base>((ushort)3));
            Assert.IsInstanceOf<Derived>(val);
            Assert.AreEqual(3, val.X);

            Assert.DoesNotThrow(() => val = FormatConversion.ConvertTo<Base>((int)3));
            Assert.IsInstanceOf<Base>(val);
            Assert.AreEqual(5, val.X);
        }

        [Test]
        public void ConvertFromBaseWithDerivedConverterThrows()
        {
            // We cannot do the inverse, from base type use the derived converter
            Base val = new Base { X = 3 };
            Assert.Throws<InvalidOperationException>(
                () => FormatConversion.ConvertTo<ushort>(val));
        }

        [Test]
        public void ConvertToDerivedWithDerivedConverter()
        {
            // Just to validate converter, derived with derived ocnverter
            Derived derived = null;
            Assert.DoesNotThrow(() => derived = FormatConversion.ConvertTo<Derived>((ushort)4));
            Assert.AreEqual(5, derived.Y);
            Assert.AreEqual(4, derived.X);

            ushort conv = 0;
            Assert.DoesNotThrow(() => conv = FormatConversion.ConvertTo<ushort>(derived));
            Assert.AreEqual(5, conv);
        }

        [Test]
        public void ConvertToDerivedWithBaseConverterThrows()
        {
            Assert.Throws<InvalidOperationException>(
                () => FormatConversion.ConvertTo<Derived>(5));
        }

        [Test]
        public void ConvertFromDerivedWithBaseConverter()
        {
            var format = new Derived { Y = 11, X = 10 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = FormatConversion.ConvertTo<int>(format));
            Assert.AreEqual(15, conv);
        }

        [Test]
        public void GenericConvertiWithCreatesConverterAndConverts()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                FormatConversion.ConvertWith<FormatTestDuplicatedConverter2, StringFormatTest, short>(format),
                Is.EqualTo(3));
        }

        [Test]
        public void GenericConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter1();
            Assert.AreEqual(
                FormatConversion.ConvertWith<StringFormatTest, short>(converter, format),
                3);
        }

        [Test]
        public void ConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                FormatConversion.ConvertWith(converter, format, typeof(short)),
                3);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfConverterIsNull()
        {
            TwoConvertersExample converter = null;
            Assert.That(
                () => FormatConversion.ConvertWith<string, int>(converter, "3"),
                Throws.ArgumentNullException);
            Assert.That(
                () => FormatConversion.ConvertWith(converter, "3", typeof(int)),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfDstTypeIsNull()
        {
            var converter = new TwoConvertersExample();
            Assert.That(
                () => FormatConversion.ConvertWith(converter, "3", null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            double converter = 0;
            var ex = Assert.Throws<ArgumentException>(() =>
                FormatConversion.ConvertWith(converter, format, typeof(short)));
            Assert.AreEqual(
                "Converter doesn't implement IConverter<,>" +
                Environment.NewLine + "Parameter name: converter",
                ex.Message);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfInvalidConverter()
        {
            var format = new StringFormatTest("3");
            var converter = new StringFormatTest2IntFormatTestConverter();
            var ex = Assert.Throws<ArgumentException>(() =>
                FormatConversion.ConvertWith(converter, format, typeof(short)));
            Assert.AreEqual(
                "Converter cannot convert from/to the type" +
                Environment.NewLine + "Parameter name: converter",
                ex.Message);
        }

        [Test]
        public void FormatMetadataContainsNameAndType()
        {
            var format = PluginManager.Instance.GetFormats()
                .Single(p => p.Metadata.Type == typeof(StringFormatTest));
            Assert.That(
                format.Metadata.Name,
                Is.EqualTo("Yarhl.UnitTests.FileFormat.StringFormatTest"));
        }

        [Test]
        public void FormatsAreNotDuplicated()
        {
            Assert.That(
                PluginManager.Instance.GetFormats().Select(f => f.Metadata.Type),
                Is.Unique);
        }

        [Test]
        public void GetFormatsReturnsKnownFormats()
        {
            Assert.That(
                PluginManager.Instance.GetFormats().Select(f => f.Metadata.Name),
                Does.Contain("Yarhl.FileFormat.BinaryFormat"));
        }

        public class FormatTestDuplicatedConverter1 :
            IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        public class FormatTestDuplicatedConverter2 :
            IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        public class FormatTestBadConstructor :
            IConverter<StringFormatTest, ushort>
        {
            public FormatTestBadConstructor()
            {
                throw new Exception();
            }

            public FormatTestBadConstructor(string dummy)
            {
                // This one doesn't throw
            }

            public ushort Convert(StringFormatTest test)
            {
                return 0;
            }
        }

        [PartNotDiscoverable]
        public class FormatTestNoConstructor :
            IConverter<StringFormatTest, long>
        {
            public FormatTestNoConstructor(string dummy)
            {
                Dummy = dummy;
            }

            public string Dummy { get; set; }

            public long Convert(StringFormatTest format)
            {
                return 0;
            }
        }

        [PartNotDiscoverable]
        public class FormatTestPrivateConstructor :
            IConverter<StringFormatTest, ulong>
        {
            FormatTestPrivateConstructor()
            {
            }

            public static FormatTestPrivateConstructor Create()
            {
                return new FormatTestPrivateConstructor();
            }

            public ulong Convert(StringFormatTest format)
            {
                return 0;
            }
        }

        public class Base
        {
            public ushort X { get; set; }
        }

        public class Derived : Base
        {
            public ushort Y { get; set; }
        }

        public class ConvertDerived :
            IConverter<ushort, Derived>, IConverter<Derived, ushort>
        {
            public Derived Convert(ushort source)
            {
                return new Derived {
                    X = source,
                    Y = (ushort)(source + 1),
                };
            }

            public ushort Convert(Derived source)
            {
                return source.Y;
            }
        }

        public class ConvertBase :
            IConverter<int, Base>, IConverter<Base, int>
        {
            public Base Convert(int source)
            {
                return new Base {
                    X = (ushort)(source + 2),
                };
            }

            public int Convert(Base source)
            {
                return source.X + 5;
            }
        }
    }
}
