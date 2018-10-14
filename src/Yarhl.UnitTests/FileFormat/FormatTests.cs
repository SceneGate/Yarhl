//
//  FormatTests.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using System.Composition;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class FormatTests
    {
        [Test]
        public void DisposeChangesDisposed()
        {
            var format = new StringFormatTest("3");
            Assert.IsFalse(format.Disposed);
            format.Dispose();
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void DisposeTwiceDoesNotThrowException()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.DoesNotThrow(format.Dispose);
            Assert.IsTrue(format.Disposed);
        }

        [Test]
        public void StaticGenericConvertToConverts()
        {
            Assert.AreEqual(Format.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void StaticTypedArgConvertToConverts()
        {
            Assert.That(Format.ConvertTo<int, string>("3"), Is.EqualTo(3));
        }

        [Test]
        public void StaticTypedArgConvertThrowsIfNull()
        {
            Assert.That(
                () => Format.ConvertTo<int, string>(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void StaticConvertToConverts()
        {
            Assert.AreEqual(Format.ConvertTo(typeof(int), "3"), 3);
            Assert.AreEqual(Format.ConvertTo(typeof(string), 3), "3");
        }

         [Test]
        public void StaticConvertToThrowsIfTypeIsNull()
        {
            Type dstType = null;
            Assert.That(
                () => Format.ConvertTo(dstType, "3"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void StaticConvertToThrowsIfSrcIsNull()
        {
            Assert.That(
                () => Format.ConvertTo(typeof(int), null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void StaticConvertToThrowsIfThereAreTwoEqualConverters()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(short), test));
            Assert.AreEqual(
                "Multiple converters for: " +
                "Yarhl.UnitTests.FileFormat.StringFormatTest -> System.Int16",
                ex.Message);
        }

        [Test]
        public void StaticConvertToThrowsIfNoConverter()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(short), (short)3));
            Assert.AreEqual(
                "Cannot find converter for: System.Int16 -> System.Int16",
                ex.Message);
        }

        [Test]
        public void StaticConvertToThrowsIfConstructorFails()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<Exception>(() =>
                Format.ConvertTo(typeof(ushort), test));
            Assert.AreEqual(
                "Exception of type 'System.Exception' was thrown.",
                ex.Message);
        }

        [Test]
        public void StaticConvertToThrowsExceptionIfConstructorsHaveArgs()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(long), test));
            Assert.AreEqual(
                "The converter has not a public constructor with no arguments.\n" +
                "Create an instance of the converter and use ConvertWith.",
                ex.Message);
        }

        [Test]
        public void StaticConvertToThrowsIfNoPublicConstructor()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(ulong), test));
            Assert.AreEqual(
                "The converter has not a public constructor with no arguments.\n" +
                "Create an instance of the converter and use ConvertWith.",
                ex.Message);
        }

        [Test]
        public void ConvertToBaseWithDerivedConverter()
        {
            // It should use the converter: ushort -> Derived
            // The converter will generate a derived type and will cast-down
            // to base.
            Base val = null;
            Assert.DoesNotThrow(() => val = Format.ConvertTo<Base>((ushort)3));
            Assert.IsInstanceOf<Derived>(val);
            Assert.AreEqual(3, val.X);
        }

        [Test]
        public void ConvertFromBaseWithDerivedConverterThrows()
        {
            // We cannot do the inverse, from base type use the derived converter
            Base val = new Base { X = 3 };
            Assert.Throws<InvalidOperationException>(
                () => Format.ConvertTo<ushort>(val));
        }

        [Test]
        public void ConvertToDerivedWithDerivedConverter()
        {
            // Just to validate converter, derived with derived ocnverter
            Derived derived = null;
            Assert.DoesNotThrow(() => derived = Format.ConvertTo<Derived>((ushort)4));
            Assert.AreEqual(5, derived.Y);
            Assert.AreEqual(4, derived.X);

            ushort conv = 0;
            Assert.DoesNotThrow(() => conv = Format.ConvertTo<ushort>(derived));
            Assert.AreEqual(5, conv);
        }

        [Test]
        public void ConvertToDerivedWithBaseConverterThrows()
        {
            Assert.Throws<InvalidOperationException>(
                () => Format.ConvertTo<Derived>(5));
        }

        [Test]
        public void ConvertFromDerivedWithBaseConverter()
        {
            var format = new Derived { Y = 11, X = 10 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = Format.ConvertTo<int>(format));
            Assert.AreEqual(15, conv);
        }

        [Test]
        public void StaticGenericConvertiWithCreatesConverterAndConverts()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                Format.ConvertWith<FormatTestDuplicatedConverter2, StringFormatTest, short>(format),
                Is.EqualTo(3));
        }

        [Test]
        public void StaticGenericConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                Format.ConvertWith<StringFormatTest, short>(converter, format),
                3);
        }

        [Test]
        public void StaticConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                Format.ConvertWith(converter, format, typeof(short)),
                3);
        }

        [Test]
        public void StaticConvertWithThrowsExceptionIfConverterIsNull()
        {
            TwoConvertersExample converter = null;
            Assert.That(
                () => Format.ConvertWith<string, int>(converter, "3"),
                Throws.ArgumentNullException);
            Assert.That(
                () => Format.ConvertWith(converter, "3", typeof(int)),
                Throws.ArgumentNullException);
        }

        [Test]
        public void StaticConvertWithThrowsExceptionIfDstTypeIsNull()
        {
            var converter = new TwoConvertersExample();
            Assert.That(
                () => Format.ConvertWith(converter, "3", null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void StaticConvertWithThrowsExceptionIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            double converter = 0;
            var ex = Assert.Throws<ArgumentException>(() =>
                Format.ConvertWith(converter, format, typeof(short)));
            Assert.AreEqual(
                "Converter doesn't implement IConverter<,>" +
                Environment.NewLine + "Parameter name: converter",
                ex.Message);
        }

        [Test]
        public void StaticConvertWithThrowsExceptionIfInvalidConverter()
        {
            var format = new StringFormatTest("3");
            var converter = new StringFormatTest2IntFormatTestConverter();
            var ex = Assert.Throws<ArgumentException>(() =>
                Format.ConvertWith(converter, format, typeof(short)));
            Assert.AreEqual(
                "Converter cannot convert from/to the type" +
                Environment.NewLine + "Parameter name: converter",
                ex.Message);
        }

        [Test]
        public void GenericConvertToConverts()
        {
            var format = new StringFormatTest("3");
            Assert.AreEqual(format.ConvertTo<int>(), 3);
        }

        [Test]
        public void GenericConvertToThrowExceptionIfDisposed()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.ConvertTo<int>());
        }

        [Test]
        public void ConvertToConverts()
        {
            var format = new StringFormatTest("3");
            Assert.AreEqual(format.ConvertTo(typeof(int)), 3);
        }

        [Test]
        public void ConvertToThrowsIfDisposed()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.ConvertTo(typeof(int)));
        }

        [Test]
        public void ConvertToThrowsIfDstTypeIsNull()
        {
            var format = new StringFormatTest("3");
            Assert.That(() => format.ConvertTo(null), Throws.ArgumentNullException);
        }

        [Test]
        public void GenericConvertWithCreatesConverterAndConverts()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                format.ConvertWith<FormatTestDuplicatedConverter2, StringFormatTest, short>,
                Is.EqualTo(3));
        }

        [Test]
        public void GenericConvertWithCreatingConverterThrowsIfDisposed()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.That(
                format.ConvertWith<FormatTestDuplicatedConverter2, StringFormatTest, short>,
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GenericConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                format.ConvertWith<StringFormatTest, short>(converter),
                3);
        }

        [Test]
        public void GenericConvertWithThrowsIfConverterIsNull()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => format.ConvertWith<StringFormatTest, short>(null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void GenericConvertWithThrowsIfDisposed()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
                format.ConvertWith<StringFormatTest, short>(converter));
        }

        [Test]
        public void ConvertWithConverts()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.That(
                () => format.ConvertWith(converter, typeof(short)),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithThrowsIfDisposed()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            format.Dispose();
            Assert.That(
                () => format.ConvertWith(converter, typeof(short)),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void ConvertWithThrowsIfDstTypeIsNull()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Type dstType = null;
            Assert.That(
                () => format.ConvertWith(converter, dstType),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithThrowsIfConverterIsNull()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => format.ConvertWith(null, typeof(short)),
                Throws.ArgumentNullException);
        }

        [Test]
        public void FormatAttributeSetsName()
        {
            var attr = typeof(StringFormatTest)
                .GetCustomAttributes(typeof(FormatAttribute), true);
            Assert.That(attr, Has.One.Items);

            var formatAttr = attr[0] as FormatAttribute;
            Assert.That(formatAttr.Name, Is.EqualTo("Yarhl.UnitTests.StringFormat"));
        }

        [Test]
        public void FormatAttributeWithoutConstructorHasNullName()
        {
            var attr = typeof(IntFormatTest)
                .GetCustomAttributes(typeof(FormatAttribute), true);
            Assert.That(attr, Has.One.Items);

            var formatAttr = attr[0] as FormatAttribute;
            Assert.That(formatAttr.Name, Is.Null);
        }

        [Test]
        public void FormatsAreNotDuplicated()
        {
            Assert.That(Format.GetFormats().Select(f => f.GetType()), Is.Unique);
        }

        [Test]
        public void GetFormatsReturnsKnownFormats()
        {
            Assert.That(
                Format.GetFormats().Select(f => f.GetType().FullName),
                Does.Contain("Yarhl.FileFormat.BinaryFormat"));
        }

        [Export(typeof(IConverter<StringFormatTest, short>))]
        public class FormatTestDuplicatedConverter1 :
            IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        [Export(typeof(IConverter<StringFormatTest, short>))]
        public class FormatTestDuplicatedConverter2 :
            IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        [Export(typeof(IConverter<StringFormatTest, ushort>))]
        public class FormatTestBadConstructor :
            IConverter<StringFormatTest, ushort>
        {
            public FormatTestBadConstructor()
            {
                throw new Exception();
            }

            public ushort Convert(StringFormatTest test)
            {
                return 0;
            }
        }

        [Export(typeof(IConverter<StringFormatTest, long>))]
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

        [Export(typeof(IConverter<StringFormatTest, ulong>))]
        public class FormatTestPrivateConstructor :
            IConverter<StringFormatTest, ulong>
        {
            FormatTestPrivateConstructor()
            {
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

        [Export(typeof(IConverter<ushort, Derived>))]
        [Export(typeof(IConverter<Derived, ushort>))]
        [Export(typeof(IConverter<ushort, Base>))]
        public class ConvertDerived :
            IConverter<ushort, Derived>, IConverter<Derived, ushort>
        {
            public Derived Convert(ushort source)
            {
                return new Derived {
                    X = source,
                    Y = (ushort)(source + 1)
                };
            }

            public ushort Convert(Derived source)
            {
                return source.Y;
            }
        }

        [Export(typeof(IConverter<int, Base>))]
        [Export(typeof(IConverter<Base, int>))]
        [Export(typeof(IConverter<Derived, int>))]
        public class ConvertBase :
            IConverter<int, Base>, IConverter<Base, int>
        {
            public Base Convert(int source)
            {
                return new Base {
                    X = (ushort)(source + 2)
                };
            }

            public int Convert(Base source)
            {
                return source.X + 5;
            }
        }
    }
}
