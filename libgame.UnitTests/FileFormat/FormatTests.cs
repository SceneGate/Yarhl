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
using System;


namespace Libgame.UnitTests.FileFormat
{
    using Libgame.FileFormat;
    using Mono.Addins;
    using NUnit.Framework;

    [TestFixture]
    public class FormatTests
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(".addins");
                AddinManager.Registry.Update();
            }
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            if (AddinManager.IsInitialized)
                AddinManager.Shutdown();
        }

        [Test]
        public void Convert()
        {
            Assert.AreEqual(Format.Convert(typeof(string), "3", typeof(int)), 3);
            Assert.AreEqual(Format.Convert(typeof(int), 3, typeof(string)), "3");
        }

        [Test]
        public void ConvertThrowsExceptionIfTwoConverters()
        {
            var test = new FormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.Convert(typeof(FormatTest), test, typeof(short)));
            Assert.AreEqual(
                "No single converter for " +
                "Libgame.UnitTests.FileFormat.FormatTest -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoConverters()
        {
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.Convert(typeof(short), 3, typeof(short)));
            Assert.AreEqual(
                "No single converter for System.Int16 -> System.Int16",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfConstructorFails()
        {
            var test = new FormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.Convert(typeof(FormatTest), test, typeof(ushort)));
            Assert.AreEqual("Exception in converter constructor", ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoConstructorWithNoArgs()
        {
            var test = new FormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.Convert(typeof(FormatTest), test, typeof(long)));
            Assert.AreEqual(
                "The converter has no constructor without arguments.\n" +
                "Create the converter object and use ConvertWith<T>.",
                ex.Message);
        }

        [Test]
        public void ConvertThrowsExceptionIfNoPublicConstructor()
        {
            var test = new FormatTest("3");
            var ex = Assert.Throws<InvalidOperationException>(() =>
                Format.Convert(typeof(FormatTest), test, typeof(ulong)));
            Assert.AreEqual(
                "The converter has no constructor without arguments.\n" +
                "Create the converter object and use ConvertWith<T>.", 
                ex.Message);
        }

        [Test]
        public void ConvertToBase()
        {
            Assert.DoesNotThrow(() => Format.Convert<ushort, Base>(3));
        }

        [Test]
        public void ConvertFromBase()
        {
            Assert.DoesNotThrow(() => Format.Convert<Base, ushort>(null));
        }

        [Test]
        public void ConvertFrom()
        {
            Assert.AreEqual(Format.ConvertFrom("3", typeof(int)), 3);
        }

        [Test]
        public void ConvertTo()
        {
            Assert.AreEqual(Format.ConvertTo<int>("3"), 3);
        }

        [Test]
        public void ConvertGeneric()
        {
            Assert.AreEqual(Format.Convert<string, int>("3"), 3);
        }

        [Test]
        public void ConvertWithGeneric()
        {
            var format = new FormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(Format.ConvertWith<short>(format, converter), 3);
        }

        [Test]
        public void ConvertWith()
        {
            var format = new FormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(
                Format.ConvertWith(format, typeof(short), converter),
                3);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfNoImplementIConverter()
        {
            var format = new FormatTest("3");
            var converter = new Double();
            var ex = Assert.Throws<ArgumentException>(() =>
                Format.ConvertWith(format, typeof(short), converter));
            Assert.AreEqual(
                "Converter doesn't implement IConverter<,>\n" +
                "Parameter name: converter", 
                ex.Message);
        }

        [Test]
        public void ConvertWithThrowsExceptionIfInvalidConverter()
        {
            var format = new FormatTest("3");
            var converter = new FormatTestConverter();
            var ex = Assert.Throws<ArgumentException>(() =>
                Format.ConvertWith(format, typeof(short), converter));
            Assert.AreEqual(
                "Converter cannot convert from/to the type\n" +
                "Parameter name: converter", 
                ex.Message);
        }

        [Test]
        public void ClassConvertTo()
        {
            var format = new FormatTest("3");
            Assert.AreEqual(format.ConvertTo<int>(), 3);
        }

        [Test]
        public void ClassConvertWith()
        {
            var format = new FormatTest("3");
            var converter = new FormatTestDuplicatedConverter2();
            Assert.AreEqual(format.ConvertWith<short>(converter), 3);
        }
    }

    [Extension]
    public class FormatTest : Format
    {
        public FormatTest(string str)
        {
            Value = str;
        }

        public string Value { get; private set; }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }

        public override string Name {
            get { return "unittest.format"; }
        }
    }

    [Extension]
    public class FormatTestConverter : IConverter<FormatTest, int>
    {
        public int Convert(FormatTest test)
        {
            return System.Convert.ToInt32(test.Value);
        }
    }

    [Extension]
    public class FormatTestDuplicatedConverter1 : IConverter<FormatTest, short>
    {
        public short Convert(FormatTest test)
        {
            return System.Convert.ToInt16(test.Value);
        }
    }

    [Extension]
    public class FormatTestDuplicatedConverter2 : IConverter<FormatTest, short>
    {
        public short Convert(FormatTest test)
        {
            return System.Convert.ToInt16(test.Value);
        }
    }

    [Extension]
    public class FormatTestBadConstructor : IConverter<FormatTest, ushort>
    {
        public FormatTestBadConstructor()
        {
            throw new Exception();
        }

        public ushort Convert(FormatTest test)
        {
            return 0;
        }
    }

    [Extension]
    public class FormatTestNoConstructor : IConverter<FormatTest, long>
    {
        public FormatTestNoConstructor(string dummy)
        {
            Dummy = dummy;
        }

        public string Dummy { get; set; }

        public long Convert(FormatTest format)
        {
            return 0;
        }
    }

    [Extension]
    public class FormatTestPrivateConstructor : IConverter<FormatTest, ulong>
    {
        private FormatTestPrivateConstructor()
        {
        }

        public ulong Convert(FormatTest format)
        {
            return 0;
        }
    }

    public class Base
    {
    }

    public class Derived : Base
    {
    }

    [Extension]
    public class ConvertDerived : 
        IConverter<ushort, Derived>, IConverter<Derived, ushort>
    {
        public Derived Convert(ushort source)
        {
            return new Derived();
        }

        public ushort Convert(Derived source)
        {
            return 10;
        }
    }
}

