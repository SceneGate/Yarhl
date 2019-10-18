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

    public abstract class BaseGeneralTests<T>
        where T : IFormat
    {
        protected string Name { get => typeof(T).FullName; }

        [Test]
        public void FormatIsFoundAndIsUnique()
        {
            var formats = PluginManager.Instance.GetFormats()
                .Select(f => f.Metadata.Type);
            Assert.That(formats, Does.Contain(typeof(T)));
            Assert.That(formats, Is.Unique);
        }

        [Test]
        public void FormatNameMatchAndIsUnique()
        {
            var names = PluginManager.Instance.GetFormats()
                .Select(f => f.Metadata.Name);
            Assert.That(names, Does.Contain(Name));
            Assert.That(names, Is.Unique);
        }

        protected abstract T CreateDummyFormat();
    }
}
