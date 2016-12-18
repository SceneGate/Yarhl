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
using System;
using System.Linq;
using NUnit.Framework;
using Mono.Addins;
using Libgame.FileFormat;
using System.Collections.Generic;

namespace Libgame.UnitTests.FileFormat
{
    [TestFixture]
    public class ConverterTests
    {
        [OneTimeSetUp]
        public void SetUp()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(".addins");
                AddinManager.Registry.Update();
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (AddinManager.IsInitialized)
                AddinManager.Shutdown();
        }

        private static List<Type> GetConverters()
        {
            List<Type> converterTypes = new List<Type>();
            Assert.DoesNotThrow(() => {
                converterTypes = AddinManager
                    .GetExtensionNodes<TypeExtensionNode>(typeof(IConverter<,>))
                    .Select(node => node.Type)
                    .ToList();
            });
            return converterTypes;
        }

        [Test]
        public void FindCorrectType()
        {
            List<Type> converterTypes = GetConverters();
            Assert.IsFalse(
                converterTypes.Any(t =>
                    t.GetInterfaces().All(
                        i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() != (typeof(IConverter<,>)))));
        }

        [Test]
        public void FindSingleInnerConverter()
        {
            List<Type> converterTypes = GetConverters();
            Assert.Contains(
                typeof(SingleOuterConverterExample.SingleInnerConverterExample),
                converterTypes);
        }

        [Test]
        public void FindSingleOuterConverter()
        {
            List<Type> converterTypes = GetConverters();
            Assert.Contains(typeof(SingleOuterConverterExample), converterTypes);
        }

        [Test]
        public void FindTwoConvertersInSameClass()
        {
            List<Type> converterTypes = GetConverters();
            Assert.Contains(typeof(TwoConvertersExample), converterTypes);
            Assert.DoesNotThrow(() =>
                converterTypes.Single(t =>
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(string) &&
                        i.GenericTypeArguments[1] == typeof(int))));
            Assert.DoesNotThrow(() =>
                converterTypes.Single(t =>
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GenericTypeArguments.Length == 2 &&
                        i.GenericTypeArguments[0] == typeof(int) &&
                        i.GenericTypeArguments[1] == typeof(string))));
        }

        [Test]
        public void FindDerivedConverter()
        {
            List<Type> converterTypes = GetConverters();
            Assert.Contains(typeof(DerivedConverter), converterTypes);
        }
    }
}

