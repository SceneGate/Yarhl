//
//  ConverterTests.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class ConverterTests
    {
        [Test]
        public void FindSingleInnerConverter()
        {
            IConverter<string, ulong> converter = null;
            Assert.That(
                () => converter = PluginManager.Instance
                    .FindExtensions<IConverter<string, ulong>>()
                    .Single(),
                Throws.Nothing);
            Assert.That(
                converter,
                Is.InstanceOf<SingleOuterConverterExample.SingleInnerConverterExample>());
            Assert.That(converter.Convert("4"), Is.EqualTo(4));
        }

        [Test]
        public void FindSingleOuterConverter()
        {
            IConverter<string, uint> converter = null;
            Assert.That(
                () => converter = PluginManager.Instance
                    .FindExtensions<IConverter<string, uint>>()
                    .Single(),
                Throws.Nothing);
            Assert.That(converter, Is.InstanceOf<SingleOuterConverterExample>());
            Assert.That(converter.Convert("5"), Is.EqualTo(5));
        }

        [Test]
        public void FindTwoConvertersInSameClass()
        {
            var converter1 = PluginManager.Instance
                .FindExtensions<IConverter<string, int>>();
            Assert.IsInstanceOf<TwoConvertersExample>(converter1.Single());

            Assert.DoesNotThrow(() =>
                converter1.Single(t =>
                    t.GetType().GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(string) &&
                        i.GenericTypeArguments[1] == typeof(int))));

            var converter2 = PluginManager.Instance
                .FindExtensions<IConverter<int, string>>();
            Assert.IsInstanceOf<TwoConvertersExample>(converter2.Single());

            Assert.DoesNotThrow(() =>
                converter2.Single(t =>
                    t.GetType().GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(int) &&
                        i.GenericTypeArguments[1] == typeof(string))));
        }

        [Test]
        public void FindDerivedConverter()
        {
            var converters = PluginManager.Instance
                .FindExtensions<IConverter<string, ushort>>();
            IConverter<string, ushort> converter = null;
            Assert.That(
                () => converter = converters.Single(),
                Throws.Nothing);
            Assert.IsInstanceOf<DerivedConverter>(converter);
            Assert.IsInstanceOf<BaseConverter>(converter);
            Assert.That(converter.Convert("3"), Is.EqualTo(3));
        }

        [Test]
        public void FindConvertsWithOtherInterfaces()
        {
            IConverter<string, short> converter = null;
            Assert.That(
                () => converter = PluginManager.Instance
                    .FindExtensions<IConverter<string, short>>()
                    .Single(),
                Throws.Nothing);
            Assert.That(converter, Is.InstanceOf<ConverterAndOtherInterface>());
            Assert.That(converter.Convert("3"), Is.EqualTo(3));
            Assert.That(
                ((ConverterAndOtherInterface)converter).Dispose,
                Throws.Nothing);
        }
    }
}
