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
    using System.Composition;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class ConvertFormatTests
    {
        [Test]
        public void ConvertToGeneric()
        {
            Assert.AreEqual(ConvertFormat.To<int>("3"), 3);
        }

        [Test]
        public void ConvertToWithType()
        {
            Assert.AreEqual(ConvertFormat.To(typeof(int), "3"), 3);
            Assert.AreEqual(ConvertFormat.To(typeof(string), 3), "3");
        }

        [Test]
        public void ConvertToWithTypeThrowsIfTypeIsNull()
        {
            Type dstType = null;
            Assert.That(
                () => ConvertFormat.To(dstType, "3"),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfSrcIsNull()
        {
            Assert.That(
                () => ConvertFormat.To<int>(null),
                Throws.ArgumentNullException);
            Assert.That(
                () => ConvertFormat.To(typeof(int), null),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertToThrowsIfThereAreTwoEqualConverters()
        {
            var test = new StringFormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                ConvertFormat.To(typeof(short), test));
            Assert.AreEqual(
                "Multiple converters for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(short).FullName}",
                ex.Message);
        }

        [Test]
        public void ConvertToThrowsIfNoConverter()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                ConvertFormat.To(typeof(short), (short)3));
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
                ConvertFormat.To(typeof(ushort), test));
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
                ConvertFormat.To(typeof(long), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(long).FullName}",
                ex.Message);

            // But we can use the With()
            var converter = new FormatTestNoConstructor("3");
            Assert.AreEqual(
                ConvertFormat.With(converter, new StringFormatTest("1")),
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
                ConvertFormat.To(typeof(ulong), test));
            Assert.AreEqual(
                "Cannot find converter for: " +
                $"{typeof(StringFormatTest).FullName} -> {typeof(ulong).FullName}",
                ex.Message);

            // But we can use the With() of classes with Factory pattern.
            var converter = FormatTestPrivateConstructor.Create();
            Assert.AreEqual(
                ConvertFormat.With(converter, new StringFormatTest("1")),
                0);
        }

        [Test]
        public void ConvertToBaseWithDerivedConverter()
        {
            // It should use the converter: ushort -> Derived
            // The converter will generate a derived type and will cast-down
            // to base.
            Base val = null;
            Assert.DoesNotThrow(() => val = ConvertFormat.To<Base>((ushort)3));
            Assert.IsInstanceOf<Derived>(val);
            Assert.AreEqual(3, val.X);

            Assert.DoesNotThrow(() => val = ConvertFormat.To<Base>((int)3));
            Assert.IsInstanceOf<Base>(val);
            Assert.AreEqual(5, val.X);
        }

        [Test]
        public void ConvertFromBaseWithDerivedConverterThrows()
        {
            // We cannot do the inverse, from base type use the derived converter
            Base val = new Base { X = 3 };
            Assert.Throws<InvalidOperationException>(
                () => ConvertFormat.To<ushort>(val));
            Assert.Throws<InvalidOperationException>(
                () => ConvertFormat.With<ConvertDerived>(val));
        }

        [Test]
        public void ConvertToDerivedWithDerivedConverter()
        {
            // Just to validate converter, derived with derived converter
            Derived derived = null;
            Assert.DoesNotThrow(() => derived = ConvertFormat.To<Derived>((ushort)4));
            Assert.AreEqual(5, derived.Y);
            Assert.AreEqual(4, derived.X);

            ushort conv = 0;
            Assert.DoesNotThrow(() => conv = ConvertFormat.To<ushort>(derived));
            Assert.AreEqual(5, conv);
        }

        [Test]
        public void ConvertToDerivedWithBaseConverterThrows()
        {
            Assert.Throws<InvalidOperationException>(
                () => ConvertFormat.To<Derived>(5));
        }

        [Test]
        public void ConvertFromDerivedWithBaseConverter()
        {
            var format = new Derived { Y = 11, X = 10 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = ConvertFormat.To<int>(format));
            Assert.AreEqual(15, conv);

            Assert.DoesNotThrow(() => conv = (int)ConvertFormat.With<ConvertBase>(format));
            Assert.AreEqual(15, conv);
        }

        [Test]
        public void ConvertFromImplementationWithInterfaceConverter()
        {
            var format = new InterfaceImpl { Z = 14 };
            int conv = 0;
            Assert.DoesNotThrow(() => conv = (int)ConvertFormat.With<ConverterInterface>(format));
            Assert.AreEqual(14, conv);

            Assert.DoesNotThrow(() => conv = ConvertFormat.To<int>(format));
            Assert.AreEqual(14, conv);
        }

        [Test]
        public void ConvertWithType()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                ConvertFormat.With(typeof(FormatTestDuplicatedConverter2), format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithTypeThrowsIfTypeIsNull()
        {
            var format = new StringFormatTest("3");
            Type type = null;
            Assert.That(
                () => ConvertFormat.With(type, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithTypeThrowsIfConverterNotFound()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => ConvertFormat.With(typeof(HiddenConverter), format),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    $"Cannot find converter {typeof(HiddenConverter).FullName}"));
        }

        [Test]
        public void ConvertWithTypeThrowsIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                () => ConvertFormat.With(typeof(DateTime), format),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    $"Cannot find converter {typeof(DateTime).FullName}"));
        }

        [Test]
        public void ConvertWithGeneric()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                ConvertFormat.With<FormatTestDuplicatedConverter1>(format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithInit()
        {
            var format = new StringFormatTest("3");
            Assert.That(
                ConvertFormat.With<HiddenConverter, int>(4, format),
                Is.EqualTo(7));
        }

        [Test]
        public void ConvertWithInstance()
        {
            var format = new StringFormatTest("3");
            var converter = new HiddenConverter();
            Assert.That(
                ConvertFormat.With(converter, format),
                Is.EqualTo(3));
        }

        [Test]
        public void ConvertWithThrowsIfFormatIsNull()
        {
            StringFormatTest format = null;

            Assert.That(
                () => ConvertFormat.With<HiddenConverter>(format),
                Throws.ArgumentNullException);
            Assert.That(
                () => ConvertFormat.With<HiddenConverter, int>(4, format),
                Throws.ArgumentNullException);

            var converter = new HiddenConverter();
            Assert.That(
                () => ConvertFormat.With(converter, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithInstanceThrowsIfConverterIsNull()
        {
            var format = new StringFormatTest("3");
            HiddenConverter converter = null;
            Assert.That(
                () => ConvertFormat.With(converter, format),
                Throws.ArgumentNullException);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfNoImplementIConverter()
        {
            var format = new StringFormatTest("3");
            string msg = "Converter doesn't implement IConverter<,>";

            Assert.That(
                () => ConvertFormat.With<ConverterWithoutGenericInterface>(format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => ConvertFormat.With<ConverterWithoutGenericInterface, int>(4, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            var converter = new ConverterWithoutGenericInterface();
            Assert.That(
                () => ConvertFormat.With(converter, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
        }

        [Test]
        public void ConvertWithInstanceThrowsExceptionIfInvalidConverter()
        {
            var format = new StringFormatTest("3");
            string msg = "Converter cannot convert from/to the type";

            Assert.That(
                () => ConvertFormat.With<SingleOuterConverterExample>(format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));
            Assert.That(
                () => ConvertFormat.With<ConverterAndOtherInterface, int>(4, format),
                Throws.InvalidOperationException.With.Message.EqualTo(msg));

            var converter = new SingleOuterConverterExample();
            Assert.That(
                () => ConvertFormat.With(converter, format),
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

        public class InterfaceImpl : IInterface
        {
            public int Z { get; set; }
        }

        public class ConverterInterface :
            IConverter<IInterface, int>
        {
            public int Convert(IInterface source)
            {
                return source.Z;
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
