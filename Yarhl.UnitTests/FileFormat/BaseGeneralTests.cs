//
// BaseGeneralTests.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Yarhl.UnitTests.FileFormat
{
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.FileFormat;

    public abstract class BaseGeneralTests<T>
        where T : Format
    {
        [Test]
        public void FormatIsFound()
        {
            Assert.IsTrue(PluginManager.Instance
                          .FindExtensions<Format>().Contains(typeof(T)));
        }

        public void NameIsCorrect(string packageName, string formatName)
        {
            T format = CreateDummyFormat();
            Assert.AreEqual(packageName + "." + formatName, format.Name);
            format.Dispose();
        }

        [Test]
        public void DisposeChangesDisposed()
        {
            T format = CreateDummyFormat();
            format.Dispose();
            Assert.IsTrue(format.Disposed);
        }

        protected abstract T CreateDummyFormat();
    }
}
