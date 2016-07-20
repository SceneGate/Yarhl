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
namespace Libgame.UnitTests.FileFormat
{
    using Libgame.FileFormat;
    using Mono.Addins;
    using NUnit.Framework;

    [TestFixture]
    public class FormatTests
    {
        [TestFixtureSetUp]
        public void Setup()
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
        public void ConvertFromTest()
        {
            Assert.AreEqual(Format.ConvertFrom("3", typeof(int)), 3);
        }

        [Test]
        public void ConvertTest()
        {
            Assert.AreEqual(Format.Convert(typeof(string), "3", typeof(int)), 3);
            Assert.AreEqual(Format.Convert(typeof(int), 3, typeof(string)), "3");
        }
    }

    [Extension]
    public class FormatTest : Format
    {
        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }

        public override string Name {
            get { return "unittest.format"; }
        }
    }

    [Extension]
    public class String2IntegerConverter : 
        IConverter<string, int>, IConverter<int, string>
    {
        public int Convert(string source)
        {
            return System.Convert.ToInt32(source);
        }

        public string Convert(int source)
        {
            return source.ToString();
        }
    }
}

