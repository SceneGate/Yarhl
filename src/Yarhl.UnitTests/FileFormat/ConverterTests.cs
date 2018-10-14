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
            var converterTypes = PluginManager.Instance
                .FindExtensions<IConverter<string, ulong>>();
            Assert.IsNotEmpty(converterTypes);
            Assert.IsInstanceOf<SingleOuterConverterExample.SingleInnerConverterExample>(
                converterTypes.Single());
        }

        [Test]
        public void FindSingleOuterConverter()
        {
            var converterTypes = PluginManager.Instance
                .FindExtensions<IConverter<string, uint>>();
            Assert.IsNotEmpty(converterTypes);
            Assert.IsInstanceOf<SingleOuterConverterExample>(
                converterTypes.Single());
        }

        [Test]
        public void FindTwoConvertersInSameClass()
        {
            var converterType1 = PluginManager.Instance
                .FindExtensions<IConverter<string, int>>();
            Assert.IsInstanceOf<TwoConvertersExample>(converterType1.Single());

            Assert.DoesNotThrow(() =>
                converterType1.Single(t =>
                    t.GetType().GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(string) &&
                        i.GenericTypeArguments[1] == typeof(int))));

            var converterType2 = PluginManager.Instance
                .FindExtensions<IConverter<int, string>>();
            Assert.IsInstanceOf<TwoConvertersExample>(converterType2.Single());

            Assert.DoesNotThrow(() =>
                converterType2.Single(t =>
                    t.GetType().GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(int) &&
                        i.GenericTypeArguments[1] == typeof(string))));
        }

        [Test]
        public void FindDerivedConverter()
        {
            var converterTypes = PluginManager.Instance
                .FindExtensions<IConverter<string, ushort>>();
            Assert.IsInstanceOf<DerivedConverter>(converterTypes.Single());
        }
    }
}
