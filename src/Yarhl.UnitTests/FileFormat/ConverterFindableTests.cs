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
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    [TestFixture]
    public class ConverterFindableTests
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
