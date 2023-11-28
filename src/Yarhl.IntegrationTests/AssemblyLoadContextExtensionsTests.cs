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
namespace Yarhl.IntegrationTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Yarhl.Plugins;
using Yarhl.Plugins.FileFormat;

// By forcing to run in parallel each test needs to re-load the assemblies.
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class AssemblyLoadContextExtensionsTests
{
    [Test]
    public void TestPreconditionYarhlMediaIsInPluginsFolder()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        string pluginDir = Path.Combine(programDir, "Plugins");
        Assert.IsTrue(Directory.Exists(pluginDir));

        Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "Yarhl.Media.Text.dll")));
        Assert.IsTrue(File.Exists(Path.Combine(pluginDir, "MyBadPlugin.dll")));

        Assert.IsFalse(File.Exists(Path.Combine(programDir, "Yarhl.Media.Text.dll")));
    }

    [Test]
    public void LoadingYarhlMediaFromPath()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        string assemblyPath = Path.Combine(programDir, "Plugins", "Yarhl.Media.Text.dll");
        Assembly? loaded = TypeLocator.Instance.LoadContext.TryLoadFromAssemblyPath(assemblyPath);

        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.GetName().Name, Is.EqualTo("Yarhl.Media.Text"));

        Assert.That(
            TypeLocator.Instance.LoadContext.Assemblies.Select(a => a.GetName().Name),
            Does.Contain("Yarhl.Media.Text"));
    }

    [Test]
    public void LoadingInvalidAssemblyReturnsNull()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        string assemblyPath = Path.Combine(programDir, "Plugins", "MyBadPlugin.dll");
        Assembly? loaded = TypeLocator.Instance.LoadContext.TryLoadFromAssemblyPath(assemblyPath);

        Assert.That(loaded, Is.Null);
    }

    [Test]
    public void LoadingIgnoreSystemLibraries()
    {
        IEnumerable<Assembly> loaded = TypeLocator.Instance.LoadContext.TryLoadFromExecutingDirectory();

        Assert.That(loaded.Select(a => a.GetName().Name), Does.Not.Contain("testhost"));
    }

    [Test]
    public void LoadingExecutingDirGetsYarhl()
    {
        // We cannot use ConverterLocator as it will load Yarhl as it uses some of its types.
        IEnumerable<Assembly> loaded = TypeLocator.Instance.LoadContext.TryLoadFromExecutingDirectory();

        Assert.That(loaded.Select(a => a.GetName().Name), Does.Contain("Yarhl"));

        Assert.That(
            TypeLocator.Instance.LoadContext.Assemblies.Select(a => a.GetName().Name),
            Does.Contain("Yarhl"));
    }

    [Test]
    public void LoadingPluginsDirGetsYarhlMedia()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        string pluginDir = Path.Combine(programDir, "Plugins");
        IEnumerable<Assembly> loaded = TypeLocator.Instance.LoadContext
            .TryLoadFromDirectory(pluginDir, false);

        Assert.That(loaded.Select(a => a.GetName().Name), Does.Contain("Yarhl.Media.Text"));

        Assert.That(
            TypeLocator.Instance.LoadContext.Assemblies.Select(a => a.GetName().Name),
            Does.Contain("Yarhl.Media.Text"));
    }

    [Test]
    public void LoadingPluginsDirRecursiveGetsYarhlMedia()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        IEnumerable<Assembly> loaded = TypeLocator.Instance.LoadContext
            .TryLoadFromDirectory(programDir, true);

        Assert.That(loaded.Select(a => a.GetName().Name), Does.Contain("Yarhl.Media.Text"));

        Assert.That(
            TypeLocator.Instance.LoadContext.Assemblies.Select(a => a.GetName().Name),
            Does.Contain("Yarhl.Media.Text"));
    }

    [Test]
    public void FindFormatFromPluginsDir()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
        string pluginDir = Path.Combine(programDir, "Plugins");
        TypeLocator.Instance.LoadContext.TryLoadFromDirectory(pluginDir, false);

        var formats = ConvertersLocator.Instance.Formats;
        Assert.That(formats, Is.Not.Empty);
        Assert.That(
            formats.Select(t => t.Name),
            Does.Contain("Yarhl.Media.Text.Po"));
    }

    [Test]
    public void FindConverterFromPluginsDir()
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath)!;
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
