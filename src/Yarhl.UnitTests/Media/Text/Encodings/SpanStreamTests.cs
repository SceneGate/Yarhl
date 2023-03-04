// Copyright (c) 2022 SceneGate

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
namespace Yarhl.UnitTests.Media.Text.Encodings;

using System;
using NUnit.Framework;
using Yarhl.Media.Text.Encodings;

[TestFixture]
public class SpanStreamTests
{
    [Test]
    public void WriteValue()
    {
        var buffer = new byte[1];
        var stream = new SpanStream<byte>(buffer);

        stream.Write(0xCA);

        Assert.That(stream.Length, Is.EqualTo(1));
        Assert.That(buffer[0], Is.EqualTo(0xCA));
    }

    [Test]
    public void WriteValueOutOfBound()
    {
        Assert.That(
            () => {
                var buffer = new byte[1];
                var stream = new SpanStream<byte>(buffer);
                stream.Write(0xCA);
                stream.Write(0xFE);
            },
            Throws.InstanceOf<IndexOutOfRangeException>());
    }

    [Test]
    public void WriteValues()
    {
        var buffer = new byte[2];
        var stream = new SpanStream<byte>(buffer);

        var data = new byte[2] { 0xCA, 0xFE };
        stream.Write(data);

        Assert.That(stream.Length, Is.EqualTo(2));
        Assert.That(buffer[0], Is.EqualTo(0xCA));
        Assert.That(buffer[1], Is.EqualTo(0xFE));
    }

    [Test]
    public void WriteValuesOutOfBound()
    {
        Assert.That(
            () => {
                var buffer = new byte[2];
                var stream = new SpanStream<byte>(buffer);

                var data = new byte[] { 0xCA, 0xFE, 0xFE };
                stream.Write(data);
            },
            Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void CountValue()
    {
        var stream = new SpanStream<byte>(Span<byte>.Empty);

        stream.Write(0xCA);

        Assert.That(stream.Length, Is.EqualTo(1));
    }

    [Test]
    public void CountValues()
    {
        var stream = new SpanStream<byte>(Span<byte>.Empty);

        var data = new byte[2] { 0xCA, 0xFE };
        stream.Write(data);

        Assert.That(stream.Length, Is.EqualTo(2));
    }
}
