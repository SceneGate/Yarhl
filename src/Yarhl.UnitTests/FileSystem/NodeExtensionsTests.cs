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
namespace Yarhl.UnitTests.FileSystem;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.FileSystem;
using Yarhl.UnitTests.FileFormat;

[TestFixture]
public class NodeExtensionsTests
{
    [Test]
    public void TransformWithGeneric()
    {
        int[] expected = new int[] { 1, 2, 3 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("1")));
        parent.Add(new Node("node2", new StringFormat("2")));
        parent.Add(new Node("node3", new StringFormat("3")));

        NavigableNodeCollection<Node> output = parent.Children
            .TransformCollectionWith<StringFormat2IntFormat>();

        // Nodes already transformed, before any iteration
        Assert.That(parent.Children[0].Format, Is.TypeOf<IntFormat>());

        Assert.That(output, Is.TypeOf<NavigableNodeCollection<Node>>());
        Assert.That(output, Is.SameAs(parent.Children));
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }

    [Test]
    public void TransformWithGenericIEnumerable()
    {
        int[] expected = new int[] { 1, 2, 3 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("1")));
        parent.Add(new Node("node2", new StringFormat("2")));
        parent.Add(new Node("node3", new StringFormat("3")));

        IEnumerable<Node> outputEnumerable = parent.Children
            .Where(n => true) // just to get an IEnumerable
            .TransformWith<StringFormat2IntFormat>();
        Assert.That(outputEnumerable, Is.InstanceOf<IEnumerable<Node>>());

        // Nodes not transformed yet until is iterated
        Assert.That(parent.Children[0].Format, Is.TypeOf<StringFormat>());

        // Convert so iterating for the tests don't run it twice
        Node[] output = outputEnumerable.ToArray();
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }

    [Test]
    public void TransformWithType()
    {
        int[] expected = new int[] { 0xC0, 0xC1, 0xC2 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("C1")));
        parent.Add(new Node("node2", new StringFormat("C2")));
        parent.Add(new Node("node3", new StringFormat("C3")));

        NavigableNodeCollection<Node> output = parent.Children.TransformCollectionWith(
            typeof(StringFormatConverterWithConstructor),
            NumberStyles.HexNumber,
            -1);

        // Nodes already transformed, before any iteration
        Assert.That(parent.Children[0].Format, Is.TypeOf<IntFormat>());

        Assert.That(output, Is.TypeOf<NavigableNodeCollection<Node>>());
        Assert.That(output, Is.SameAs(parent.Children));
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }

    [Test]
    public void TransformWithTypeIEnumerable()
    {
        int[] expected = new int[] { 0xC0, 0xC1, 0xC2 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("C1")));
        parent.Add(new Node("node2", new StringFormat("C2")));
        parent.Add(new Node("node3", new StringFormat("C3")));

        IEnumerable<Node> outputEnumerable = parent.Children
            .Where(n => true) // just to get an IEnumerable
            .TransformWith(typeof(StringFormatConverterWithConstructor), NumberStyles.HexNumber, -1);
        Assert.That(outputEnumerable, Is.InstanceOf<IEnumerable<Node>>());

        // Nodes not transformed yet until is iterated
        Assert.That(parent.Children[0].Format, Is.TypeOf<StringFormat>());

        // Convert so iterating for the tests don't run it twice
        Node[] output = outputEnumerable.ToArray();
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }

    [Test]
    public void TransformWithInstance()
    {
        var converter = new StringFormat2IntFormat();

        int[] expected = new int[] { 1, 2, 3 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("1")));
        parent.Add(new Node("node2", new StringFormat("2")));
        parent.Add(new Node("node3", new StringFormat("3")));

        NavigableNodeCollection<Node> output = parent.Children.TransformCollectionWith(converter);

        // Nodes already transformed, before any iteration
        Assert.That(parent.Children[0].Format, Is.TypeOf<IntFormat>());

        Assert.That(output, Is.TypeOf<NavigableNodeCollection<Node>>());
        Assert.That(output, Is.SameAs(parent.Children));
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }

    [Test]
    public void TransformWithInstanceIEnumerable()
    {
        var converter = new StringFormat2IntFormat();

        int[] expected = new int[] { 1, 2, 3 };
        using var parent = new Node("parent");
        parent.Add(new Node("node1", new StringFormat("1")));
        parent.Add(new Node("node2", new StringFormat("2")));
        parent.Add(new Node("node3", new StringFormat("3")));

        IEnumerable<Node> outputEnumerable = parent.Children
            .Where(n => true) // just to get an IEnumerable
            .TransformWith(converter);
        Assert.That(outputEnumerable, Is.InstanceOf<IEnumerable<Node>>());

        // Nodes not transformed yet until is iterated
        Assert.That(parent.Children[0].Format, Is.TypeOf<StringFormat>());

        // Convert so iterating for the tests don't run it twice
        Node[] output = outputEnumerable.ToArray();
        Assert.That(output.Select(n => n.Format), Has.All.TypeOf<IntFormat>());
        Assert.That(
            output.Select(n => n.GetFormatAs<IntFormat>().Value),
            Is.EquivalentTo(expected));
    }
}
