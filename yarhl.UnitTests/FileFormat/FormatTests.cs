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
    using Mono.Addins;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class FormatTests
    {
        [Test]
        public void Convert()
        {
            Assert.AreEqual(Format.ConvertTo(typeof(int), "3"), 3);
            Assert.AreEqual(Format.ConvertTo(typeof(string), 3), "3");
        }

        [Test]
        public void ConvertThrowsExceptionIfTwoConverters()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(short), test));
            Assert.AreEqual(
                "No single converter for: " +
                "Yarhl.UnitTests.FileFormat.StringFormatTest -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoConverters()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(short), (short)3));
            Assert.AreEqual(
                "No single converter for: System.Int16 -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfConstructorFails()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(ushort), test));
            Assert.AreEqual("Exception in converter constructor", ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoConstructorWithNoArgs()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(long), test));
            Assert.AreEqual(
                "The converter has no constructor without arguments.\n" +
                "Create the converter object and use ConvertWith<T>.",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoPublicConstructor()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.ConvertTo(typeof(ulong), test));
            Assert.AreEqual(
                "The converter has no constructor without arguments.\n" +
                "Create the converter object and use ConvertWith<T>.",
                ex.Message);
        }

        [Test]
        public void ConvertToBaseWithDerivedConverter()
        {
            // It should use the derived converter
            // The converter will generate a derived type and will cast-down
            // to base.
            Base val = null;
            Assert.DoesNotThrow(() => val = Format.ConvertTo<Base>((ushort)3));
            Assert.IsInstanceOf<Derived>(val);
            Assert.AreEqual(3, val.X);
        }

        [Test]
        public void TryToConvertFromBaseWithDerivedConverter()
        {
            // We cannot do the inverse, from base type use the derived converter
            Base val = new Base { X = 3 };
            Assert.Throws<InvalidOperationException>(
                () => Format.ConvertTo<ushort>(val));
        }

        [Test]
        public void ConvertDerivedWithDerivedConverter()
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
        public void TryToConvertToDerivedWithBase()
        {
            Assert.Throws<InvalidOperationException>(
                () => Format.ConvertTo<Derived>(5));
        }

        [Test]
        public void ConvertFromDerivedWithBase()
        {
            var format = new Derived { Y = 11, X = 10 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = Format.ConvertTo<int>(format));
            Assert.AreEqual(15, conv);
        }

        [Test]
        public void ConvertToGeneric()
        {
            Assert.AreEqual(Format.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void ConvertTo()
        {
            Assert.AreEqual(Format.ConvertTo(typeof(int), "3"), 3);
        }

        [Test]
        public void ConvertGeneric()
        {
            Assert.AreEqual(Format.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void ConvertWithGeneric()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(Format.ConvertWith(converter, format), 3);
        }

        [Test]
        public void ConvertWith()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                Format.ConvertWith(converter, format, typeof(short)),
                3);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfNoImplementIConverter()
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
        public void ConvertWithThrowsExceptionIfInvalidConverter()
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
        public void ClassConvertToGeneric()
        {
            var format = new StringFormatTest("3");
            Assert.AreEqual(format.ConvertTo<int>(), 3);
        }

        [Test]
        public void ClassConvertToGenericThrowExceptionIfDisposed()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.ConvertTo<int>());
        }

        [Test]
        public void ClassConvertTo()
        {
            var format = new StringFormatTest("3");
            Assert.AreEqual(format.ConvertTo(typeof(int)), 3);
        }

        [Test]
        public void ClassConvertToThrowExceptionIfDisposed()
        {
            var format = new StringFormatTest("3");
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() => format.ConvertTo(typeof(int)));
        }

        [Test]
        public void ClassConvertWith()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(format.ConvertWith(converter), 3);
        }

        [Test]
        public void ClassConvertWithThrowsExceptionIfDisposed()
        {
            var format = new StringFormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            format.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
                format.ConvertWith(converter));
        }

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

        [Extension]
        public class FormatTestDuplicatedConverter1 : IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        [Extension]
        public class FormatTestDuplicatedConverter2 : IConverter<StringFormatTest, short>
        {
            public short Convert(StringFormatTest test)
            {
                return System.Convert.ToInt16(test.Value);
            }
        }

        [Extension]
        public class FormatTestBadConstructor : IConverter<StringFormatTest, ushort>
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

        [Extension]
        public class FormatTestNoConstructor : IConverter<StringFormatTest, long>
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

        [Extension]
        public class FormatTestPrivateConstructor : IConverter<StringFormatTest, ulong>
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

        [Extension]
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

        [Extension]
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
