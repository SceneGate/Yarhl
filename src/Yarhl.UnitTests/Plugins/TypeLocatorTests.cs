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
namespace Yarhl.UnitTests.Plugins;

using System.Linq;
using NUnit.Framework;
using Yarhl.IO;
using Yarhl.Plugins;

[TestFixture]
public class TypeLocatorTests
{
    [Test]
    public void InstanceInitializePluginManager()
    {
        var instance = TypeLocator.Instance;
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.LoadContext, Is.Not.Null);
    }

    [Test]
    public void InstanceIsCreatedOnce()
    {
        var instance1 = TypeLocator.Instance;
        var instance2 = TypeLocator.Instance;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void FindImplementationOfInterface()
    {
        var extensions = TypeLocator.Instance
            .FindImplementationsOf(typeof(IExistsInterface))
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(1));
        Assert.Multiple(() => {
            Assert.That(extensions[0].Name, Is.EqualTo(typeof(ExistsClass).FullName));
            Assert.That(extensions[0].Type, Is.EqualTo(typeof(ExistsClass)));
            Assert.That(extensions[0].InterfaceImplemented, Is.EqualTo(typeof(IExistsInterface)));
        });
    }

    [Test]
    public void FindImplementationOfInterfaceWithAssembly()
    {
        var extensions = TypeLocator.Instance
            .FindImplementationsOf(typeof(IExistsInterface), typeof(IExistsInterface).Assembly)
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(1));
        Assert.Multiple(() => {
            Assert.That(extensions[0].Name, Is.EqualTo(typeof(ExistsClass).FullName));
            Assert.That(extensions[0].Type, Is.EqualTo(typeof(ExistsClass)));
            Assert.That(extensions[0].InterfaceImplemented, Is.EqualTo(typeof(IExistsInterface)));
        });
    }

    [Test]
    public void FindImplementationOfClass()
    {
        var extensions = TypeLocator.Instance
            .FindImplementationsOf(typeof(ExistsClass))
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(1));
        Assert.Multiple(() => {
            Assert.That(extensions[0].Name, Is.EqualTo(typeof(ExistsClass).FullName));
            Assert.That(extensions[0].Type, Is.EqualTo(typeof(ExistsClass)));
            Assert.That(extensions[0].InterfaceImplemented, Is.EqualTo(typeof(ExistsClass)));
        });
    }

    [Test]
    public void FindImplementationOfInterfaceDifferentAssemblyReturnsEmpty()
    {
        var extensions = TypeLocator.Instance
            .FindImplementationsOf(typeof(IExistsInterface), typeof(IBinary).Assembly)
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(0));
    }

    [Test]
    public void FindImplementationWithNullTypeThrowsException()
    {
        Assert.That(
            () => TypeLocator.Instance.FindImplementationsOf(null),
            Throws.ArgumentNullException);
        Assert.That(
            () => TypeLocator.Instance.FindImplementationsOf(null, typeof(IExistsInterface).Assembly),
            Throws.ArgumentNullException);
        Assert.That(
            () => TypeLocator.Instance.FindImplementationsOf(typeof(IExistsInterface), null),
            Throws.ArgumentNullException);
    }
}
