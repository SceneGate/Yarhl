// ConverterTests.cs
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
    public class ConverterTests
    {
        [Test]
        public void ConvertToGeneric()
        {
            Assert.AreEqual(Converter.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void ConvertToWithType()
        {
            Assert.AreEqual(Converter.ConvertTo(typeof(int), "3"), 3);
            Assert.AreEqual(Converter.ConvertTo(typeof(string), 3), "3");
        }

        [Test]
        public void ConvertToWithTypeThrowsIfTypeIsNull()
        {
            Type dstType = null;
            Assert.That(
                () => Converter.ConvertTo(dstType, "3"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfSrcIsNull()
        {
            Assert.That(
                () => Converter.ConvertTo<int>(null),
                Throws.ArgumentNullException);
            Assert.That(
                () => Converter.ConvertTo(typeof(int), null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfThereAreTwoEqualConverters()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Converter.ConvertTo(typeof(short), test));
            Assert.AreEqual(
                "Multiple converters for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(short).FullName}",
                ex.Message);
        }

        [Test]
        public void ConvertToThrowsIfNoConverter()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Converter.ConvertTo(typeof(short), (short)3));
            Assert.AreEqual(
                "Cannot find converter for: " +
                $"{typeof(short).FullName} -> {typeof(short).FullName}",
                ex.Message);
        }

        [Test]
        public void ConvertToThrowsIfConstructorFails()
        {
            var test = new StringFormatTest { Value = "3" };
            var ex = Assert.Throws<Exception>(() =>
                Converter.ConvertTo(typeof(ushort), test));
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
                Converter.ConvertTo(typeof(long), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(long).FullName}",
                ex.Message);

            // But we can use the ConvertWith
            var converter = new FormatTestNoConstructor("3");
            Assert.AreEqual(
                Converter.ConvertWith(converter, new StringFormatTest("1")),
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
                Converter.ConvertTo(typeof(ulong), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(ulong).FullName}",
                ex.Message);

            // But we can use the ConvertWith of classes with Factory pattern.
            var converter = FormatTestPrivateConstructor.Create();
            Assert.AreEqual(
                Converter.ConvertWith(converter, new StringFormatTest("1")),
                0);
        }

        [Test]
        public void ConvertToBaseWithDerivedConverter()
        {
            // It should use the converter: ushort -> Derived
            // The converter will generate a derived type and will cast-down
            // to base.
            Base val = null;
            Assert.DoesNotThrow(() => val = Converter.ConvertTo<Base>((ushort)3));
            Assert.IsInstanceOf<Derived>(val);
            Assert.AreEqual(3, val.X);

            Assert.DoesNotThrow(() => val = Converter.ConvertTo<Base>((int)3));
            Assert.IsInstanceOf<Base>(val);
            Assert.AreEqual(5, val.X);
        }

        [Test]
        public void ConvertFromBaseWithDerivedConverterThrows()
        {
            // We cannot do the inverse, from base type use the derived converter
            Base val = new Base { X = 3 };
            Assert.Throws<InvalidOperationException>(
                () => Converter.ConvertTo<ushort>(val));
        }

        [Test]
        public void ConvertToDerivedWithDerivedConverter()
        {
            // Just to validate converter, derived with derived converter
            Derived derived = null;
            Assert.DoesNotThrow(() => derived = Converter.ConvertTo<Derived>((ushort)4));
            Assert.AreEqual(5, derived.Y);
            Assert.AreEqual(4, derived.X);

            ushort conv = 0;
            Assert.DoesNotThrow(() => conv = Converter.ConvertTo<ushort>(derived));
            Assert.AreEqual(5, conv);
        }

        [Test]
        public void ConvertToDerivedWithBaseConverterThrows()
        {
            Assert.Throws<InvalidOperationException>(
                () => Converter.ConvertTo<Derived>(5));
        }

        [Test]
        public void ConvertFromDerivedWithBaseConverter()
        {
            var format = new Derived { Y = 11, X = 10 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = Converter.ConvertTo<int>(format));
            Assert.AreEqual(15, conv);
        }

        [Test]
        public void ConvertWithType()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                Converter.ConvertWith(typeof(FormatTestDuplicatedConverter2), format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithTypeThrowsIfTypeIsNull()
        {
            var format = new StringFormatTest("3");
            Type type = null;
            Assert.That(
                () => Converter.ConvertWith(type, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithTypeThrowsIfConverterNotFound()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => Converter.ConvertWith(typeof(HiddenConverter), format),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    $"Cannot find converter {typeof(HiddenConverter).FullName}"));
        }

        [Test]
        public void ConvertWithTypeThrowsIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => Converter.ConvertWith(typeof(DateTime), format),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    $"Cannot find converter {typeof(DateTime).FullName}"));
        }

        [Test]
        public void ConvertWithGeneric()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                Converter.ConvertWith<FormatTestDuplicatedConverter1>(format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithInit()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                Converter.ConvertWith<HiddenConverter, int>(4, format),
                Is.EqualTo(7));
        }

        [Test]
        public void ConvertWithInstance()
        {
            var format = new StringFormatTest("3");
            var converter = new HiddenConverter();
            Assert.That(
                Converter.ConvertWith(converter, format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithThrowsIfFormatIsNull()
        {
            StringFormatTest format = null;

            Assert.That(
                () => Converter.ConvertWith<HiddenConverter>(format),
                Throws.ArgumentNullException);
            Assert.That(
                () => Converter.ConvertWith<HiddenConverter, int>(4, format),
                Throws.ArgumentNullException);

            var converter = new HiddenConverter();
            Assert.That(
                () => Converter.ConvertWith(converter, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithInstanceThrowsIfConverterIsNull()
        {
            var format = new StringFormatTest("3");
            HiddenConverter converter = null;
            Assert.That(
                () => Converter.ConvertWith(converter, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            string msg = "Converter doesn't implement IConverter<,>";

            Assert.That(
                () => Converter.ConvertWith<ConverterWithoutGenericInterface>(format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => Converter.ConvertWith<ConverterWithoutGenericInterface, int>(4, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            var converter = new ConverterWithoutGenericInterface();
            Assert.That(
                () => Converter.ConvertWith(converter, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
        }

        [Test]
        public void ConvertWithInstanceThrowsExceptionIfInvalidConverter()
        {
            var format = new StringFormatTest("3");
            string msg = "Converter cannot convert from/to the type";

            Assert.That(
                () => Converter.ConvertWith<SingleOuterConverterExample>(format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => Converter.ConvertWith<ConverterAndOtherInterface, int>(4, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            var converter = new SingleOuterConverterExample();
            Assert.That(
                () => Converter.ConvertWith(converter, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
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

        [PartNotDiscoverable]
        public class HiddenConverter :
            IConverter<StringFormatTest, byte>,
            IInitializer<int>
        {
            public int Offset { get; private set; }

            public void Initialize(int param)
            {
                Offset = param;
            }

            public byte Convert(StringFormatTest format)
            {
                return (byte)(System.Convert.ToByte(format.Value) + Offset);
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
