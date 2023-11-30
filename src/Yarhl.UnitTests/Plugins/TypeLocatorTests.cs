// Copyright (c) 2023 SceneGate

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
using System.Runtime.Loader;
using NUnit.Framework;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Plugins;

[TestFixture]
public class TypeLocatorTests
{
    [Test]
    public void InstanceInitializePluginManager()
    {
        TypeLocator instance = TypeLocator.Default;
        Assert.That(instance, Is.Not.Null);
        Assert.That(instance.LoadContext, Is.Not.Null);
    }

    [Test]
    public void InstanceIsCreatedOnce()
    {
        TypeLocator instance1 = TypeLocator.Default;
        TypeLocator instance2 = TypeLocator.Default;
        Assert.That(instance1, Is.SameAs(instance2));
    }

    [Test]
    public void InitializeWithCustomLoadContextProvidesIsolation()
    {
        var loadContext = new AssemblyLoadContext(nameof(InitializeWithCustomLoadContextProvidesIsolation));
        TypeLocator isolatedLocator = new TypeLocator(loadContext);

        Assert.That(
            isolatedLocator.FindImplementationsOf(typeof(IFormat)).ToArray(),
            Is.Empty);
    }

    [Test]
    public void FindImplementationOfInterface()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOf(typeof(IExistsInterface))
            .ToList();

        int index = extensions.FindIndex(i => i.Type == typeof(ExistsClass));
        Assert.That(index, Is.Not.EqualTo(-1));

        Assert.Multiple(() => {
            Assert.That(extensions[index].Name, Is.EqualTo(typeof(ExistsClass).FullName));
            Assert.That(extensions[index].InterfaceImplemented, Is.EqualTo(typeof(IExistsInterface)));
        });
    }

    [Test]
    public void FindImplementationOfInterfaceWithAssembly()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOf(typeof(IExistsInterface), typeof(IExistsInterface).Assembly)
            .ToList();

        int index = extensions.FindIndex(i => i.Type == typeof(ExistsClass));
        Assert.That(index, Is.Not.EqualTo(-1));

        Assert.Multiple(() => {
            Assert.That(extensions[index].Name, Is.EqualTo(typeof(ExistsClass).FullName));
            Assert.That(extensions[index].InterfaceImplemented, Is.EqualTo(typeof(IExistsInterface)));
        });
    }

    [Test]
    public void FindImplementationOfClass()
    {
        var extensions = TypeLocator.Default
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
        var extensions = TypeLocator.Default
            .FindImplementationsOf(typeof(IExistsInterface), typeof(IBinary).Assembly)
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(0));
    }

    [Test]
    public void FindImplementationWithNullTypeThrowsException()
    {
        Assert.That(
            () => TypeLocator.Default.FindImplementationsOf(null),
            Throws.ArgumentNullException);
        Assert.That(
            () => TypeLocator.Default.FindImplementationsOf(null, typeof(IExistsInterface).Assembly),
            Throws.ArgumentNullException);
        Assert.That(
            () => TypeLocator.Default.FindImplementationsOf(typeof(IExistsInterface), null),
            Throws.ArgumentNullException);
    }

    [Test]
    public void FindImplementationThrowsWithGenericTypeDefinitions()
    {
        Assert.That(
            () => TypeLocator.Default
                .FindImplementationsOf(typeof(IGenericInterface<,>))
                .ToArray(),
            Throws.ArgumentException);
        Assert.That(
            () => TypeLocator.Default
                .FindImplementationsOf(typeof(IGenericInterface<,>), typeof(IGenericInterface<,>).Assembly)
                .ToArray(),
            Throws.ArgumentException);
    }

    [Test]
    public void FindImplementationIgnoresAbstractClasses()
    {
        var results = TypeLocator.Default
            .FindImplementationsOf(typeof(IExistsInterface))
            .ToList();

        Assert.That(results.Find(i => i.Type == typeof(AbstractClass)), Is.Null);
    }

    [Test]
    public void FindImplementationIgnoresInterfaces()
    {
        var results = TypeLocator.Default
            .FindImplementationsOf(typeof(IExistsInterface))
            .ToList();

        Assert.That(results.Find(i => i.Type == typeof(ISecondInterface)), Is.Null);
    }

    [Test]
    public void FindImplementationCanFindConstructorWithException()
    {
        var results = TypeLocator.Default
            .FindImplementationsOf(typeof(ConstructorWithException))
            .ToList();

        Assert.That(results, Has.Count.EqualTo(1));
        Assert.Multiple(() => {
            Assert.That(results[0].Name, Is.EqualTo(typeof(ConstructorWithException).FullName));
            Assert.That(results[0].Type, Is.EqualTo(typeof(ConstructorWithException)));
            Assert.That(results[0].InterfaceImplemented, Is.EqualTo(typeof(ConstructorWithException)));
        });
    }

    [Test]
    public void FindImplementationOfGenericInterface1()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IGenericInterface<>))
            .ToList();

        int index = extensions.FindIndex(i => i.Type == typeof(Generic1Class));
        Assert.That(index, Is.Not.EqualTo(-1));

        Assert.Multiple(() => {
            Assert.That(extensions[index].Name, Is.EqualTo(typeof(Generic1Class).FullName));
            Assert.That(extensions[index].InterfaceImplemented, Is.EqualTo(typeof(IGenericInterface<int>)));
            Assert.That(extensions[index].GenericTypes, Has.Count.EqualTo(1));
            Assert.That(extensions[index].GenericTypes[0], Is.EqualTo(typeof(int)));
        });

        // For code coverage -.-
        GenericInterfaceImplementationInfo copyInfo = extensions[index] with { };
        Assert.That(copyInfo, Is.EqualTo(extensions[index]));
    }

    [Test]
    public void FindImplementationOfGenericInterface2()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IGenericInterface<,>))
            .ToList();

        int index = extensions.FindIndex(i => i.Type == typeof(Generic2Class));
        Assert.That(index, Is.Not.EqualTo(-1));

        Assert.Multiple(() => {
            Assert.That(extensions[index].Name, Is.EqualTo(typeof(Generic2Class).FullName));
            Assert.That(extensions[index].InterfaceImplemented, Is.EqualTo(typeof(IGenericInterface<string, int>)));
            Assert.That(extensions[index].GenericTypes, Has.Count.EqualTo(2));
            Assert.That(extensions[index].GenericTypes[0], Is.EqualTo(typeof(string)));
            Assert.That(extensions[index].GenericTypes[1], Is.EqualTo(typeof(int)));
        });
    }

    [Test]
    public void FindImplementationOfGenericClass()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOf(typeof(Generic2Class))
            .ToList();

        int index = extensions.FindIndex(i => i.Type == typeof(Generic2Class));
        Assert.That(index, Is.Not.EqualTo(-1));

        Assert.Multiple(() => {
            Assert.That(extensions[index].Name, Is.EqualTo(typeof(Generic2Class).FullName));
            Assert.That(extensions[index].InterfaceImplemented, Is.EqualTo(typeof(Generic2Class)));
        });
    }

    [Test]
    public void FindImplementationOfMultipleGenericInterfaces()
    {
        var extensions = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IGenericInterface<,>))
            .Where(i => i.Type == typeof(GenericMultipleClass))
            .ToList();

        Assert.That(extensions, Has.Count.EqualTo(2));

        Assert.Multiple(() => {
            Assert.That(extensions[0].Name, Is.EqualTo(typeof(GenericMultipleClass).FullName));
            Assert.That(extensions[1].Name, Is.EqualTo(typeof(GenericMultipleClass).FullName));

            Assert.That(extensions[0].GenericTypes, Has.Count.EqualTo(2));
            Assert.That(extensions[1].GenericTypes, Has.Count.EqualTo(2));

            int indexString2Int = extensions.FindIndex(
                i => i.InterfaceImplemented == typeof(IGenericInterface<string, int>));
            Assert.That(extensions[indexString2Int].GenericTypes[0], Is.EqualTo(typeof(string)));
            Assert.That(extensions[indexString2Int].GenericTypes[1], Is.EqualTo(typeof(int)));
            Assert.That(
                extensions[indexString2Int].InterfaceImplemented,
                Is.EqualTo(typeof(IGenericInterface<string, int>)));

            int indexInt2String = indexString2Int == 0 ? 1 : 0;
            Assert.That(extensions[indexInt2String].GenericTypes[0], Is.EqualTo(typeof(int)));
            Assert.That(extensions[indexInt2String].GenericTypes[1], Is.EqualTo(typeof(string)));
            Assert.That(
                extensions[indexInt2String].InterfaceImplemented,
                Is.EqualTo(typeof(IGenericInterface<int, string>)));
        });
    }

    [Test]
    public void FindImplementationOfGenericIgnoresInterfaces()
    {
        var results = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IGenericInterface<,>))
            .ToList();

        Assert.That(results.Find(i => i.Type == typeof(ISecondGenericInterface)), Is.Null);
    }

    [Test]
    public void FindImplementationOfGenericIgnoresAbstractClasses()
    {
        var results = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IGenericInterface<,>))
            .ToList();

        Assert.That(results.Find(i => i.Type == typeof(AbstractGenericClass)), Is.Null);
    }
}
