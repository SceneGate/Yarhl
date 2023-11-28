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
namespace Yarhl.IntegrationTests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl.Plugins;
    using Yarhl.Plugins.FileFormat;

    [TestFixture]
    public class PluginDiscovery
    {
        [Test]
        public void YarhlMediaIsInPluginsFolder()
        {
            string programDir = Path.GetDirectoryName(Environment.ProcessPath);
            string pluginDir = Path.Combine(programDir, "Plugins");
            Assert.IsTrue(Directory.Exists(pluginDir));

            Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "Yarhl.Media.Text.dll")));

            Assert.IsFalse(File.Exists(Path.Combine(programDir, "Yarhl.Media.Text.dll")));
        }

        [Test]
        public void CanFoundPoByFormat()
        {
            string programDir = Path.GetDirectoryName(Environment.ProcessPath);
            string pluginDir = Path.Combine(programDir, "Plugins");
            TypeLocator.Instance.LoadContext.TryLoadFromDirectory(pluginDir, false);

            var formats = ConvertersLocator.Instance.Formats;
            Assert.That(formats, Is.Not.Empty);
            Assert.That(
                formats.Select(t => t.Name),
                Does.Contain("Yarhl.Media.Text.Po"));
        }

        [Test]
        public void CanFoundPoConverterFromTypes()
        {
            string programDir = Path.GetDirectoryName(Environment.ProcessPath);
            string pluginDir = Path.Combine(programDir, "Plugins");
            TypeLocator.Instance.LoadContext.TryLoadFromDirectory(pluginDir, false);

            Type poType = ConvertersLocator.Instance.Formats
                .Single(f => f.Name == "Yarhl.Media.Text.Po")
                .Type;

            var converters = ConvertersLocator.Instance.Converters
                .Where(f => f.CanConvert(poType));
            Assert.That(converters, Is.Not.Empty);
            Assert.That(
                converters.Select(t => t.Name),
                Does.Contain("Yarhl.Media.Text.Po2Binary"));
        }
    }
}
