
namespace Yarhl.IntegrationTests
{
    using System;
    using System.IO;
    using System.Linq;
    using NUnit.Framework;
    using Yarhl;
    using Yarhl.FileFormat;

    [TestFixture]
    public class PluginDiscovery
    {
        [Test]
        public void YarhlMediaIsInPluginsFolder()
        {
            string programDir = AppDomain.CurrentDomain.BaseDirectory;
            string pluginDir = Path.Combine(programDir, PluginManager.PluginDirectory);
            Assert.IsTrue(Directory.Exists(pluginDir));

            Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "Yarhl.Media.dll")));

            Assert.IsFalse(File.Exists(Path.Combine(programDir, "Yarhl.Media.dll")));
        }

        [Test]
        public void CanFoundPoByFormat()
        {
            var formats = Format.GetFormats();
            Assert.That(formats, Is.Not.Empty);
            Assert.That(
                formats.Select(t => t.Metadata.Name),
                Does.Contain("Yarhl.Media.Text.Po"));
        }

        [Test]
        public void CanFoundPoConverterFromTypes()
        {
            Type poType = Format.GetFormats()
                .Single(f => f.Metadata.Name == "Yarhl.Media.Text.Po")
                .Metadata.Type;

            var converters = PluginManager.Instance
                .FindLazyExtensions<IConverter, ConverterMetadata>()
                .Where(f => f.Metadata.Source.Contains(poType));
            Assert.That(converters, Is.Not.Empty);
            Assert.That(
                converters.Select(t => t.Metadata.Name),
                Does.Contain("Yarhl.Media.Text.Po2Binary"));
        }
    }
}
