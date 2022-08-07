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
using System.Text;
using Moq;
using NUnit.Framework;
using Yarhl.Media.Text.Encodings;

[TestFixture]
public class SimpleSpanEncodingTests
{
    [Test]
    public void SimpleConstructorSetExceptionFallbacks()
    {
        var encodingMock = new Mock<SimpleSpanEncoding>(999);

        var encoding = encodingMock.Object;
        Assert.That(encoding.DecoderFallback, Is.SameAs(DecoderFallback.ExceptionFallback));
        Assert.That(encoding.EncoderFallback, Is.SameAs(EncoderFallback.ExceptionFallback));
    }

    [Test]
    public void ConstructorSetsFallbacks()
    {
        var encodingMock = new Mock<SimpleSpanEncoding>(
            999,
            EncoderFallback.ReplacementFallback,
            DecoderFallback.ReplacementFallback);

        var encoding = encodingMock.Object;
        Assert.That(encoding.DecoderFallback, Is.SameAs(DecoderFallback.ReplacementFallback));
        Assert.That(encoding.EncoderFallback, Is.SameAs(EncoderFallback.ReplacementFallback));
    }

    [Test]
    public void ConstructorNullFallbacksSetsReplacement()
    {
        var encodingMock = new Mock<SimpleSpanEncoding>(999, null, null);

        var encoding = encodingMock.Object;
        Assert.That(encoding.DecoderFallback, Is.SameAs(DecoderFallback.ReplacementFallback));
        Assert.That(encoding.EncoderFallback, Is.SameAs(EncoderFallback.ReplacementFallback));
    }

    [Test]
    public void ConstructorGuards()
    {
        Assert.That(
            () => new Mock<SimpleSpanEncoding>(-1).Object,
            Throws.InnerException.InstanceOf<ArgumentOutOfRangeException>());

        Assert.That(
            () => new Mock<SimpleSpanEncoding>(
                -1,
                EncoderFallback.ReplacementFallback,
                DecoderFallback.ReplacementFallback).Object,
            Throws.InnerException.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void DefaultPropertyValues()
    {
        string expectedName = "Lutry";
        var encodingMock = new Mock<SimpleSpanEncoding>(999);
        encodingMock.CallBase = true;
        encodingMock.Setup(e => e.EncodingName).Returns(expectedName);

        var encoding = encodingMock.Object;
        Assert.That(encoding.WindowsCodePage, Is.Zero);
        Assert.That(encoding.IsMailNewsDisplay, Is.False);
        Assert.That(encoding.IsMailNewsSave, Is.False);
        Assert.That(encoding.IsBrowserDisplay, Is.False);
        Assert.That(encoding.IsBrowserSave, Is.False);
        Assert.That(encoding.CodePage, Is.EqualTo(999));
        Assert.That(encoding.HeaderName, Is.EqualTo(expectedName));
        Assert.That(encoding.BodyName, Is.EqualTo(expectedName));
        Assert.That(encoding.WebName, Is.Empty);
    }

    [Test]
    public void SingleByteWhenMaxCountIsOne()
    {
        var encodingMock = new Mock<SimpleSpanEncoding>(999) { CallBase = true };
        encodingMock.Setup(e => e.GetMaxByteCount(1)).Returns(1);

        var encoding = encodingMock.Object;
        Assert.That(encoding.IsSingleByte, Is.True);
    }

    [Test]
    public void SingleByteWhenMaxCountIsBigger()
    {
        var encodingMock = new Mock<SimpleSpanEncoding>(999) { CallBase = true };
        encodingMock.Setup(e => e.GetMaxByteCount(1)).Returns(2);

        var encoding = encodingMock.Object;
        Assert.That(encoding.IsSingleByte, Is.False);
    }

    [Test]
    public void GetByteCountsCallSpanMethod()
    {
        var encoding = new DummyEncoding();

        encoding.GetByteCount("abc");
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo("abc"));

        encoding.GetByteCount(new char[] { '1', '2', '3' });
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo("123"));

        encoding.GetByteCount(new char[] { 'z', 'x', 'w' }, 1, 1);
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo("x"));

        Assert.That(encoding.GetByteCountSpanCalls, Is.EqualTo(3));
    }

    [Test]
    public void GetByteCountsGuards()
    {
        var buffer = new char[3];
        var encoding = new DummyEncoding();

        Assert.That(() => encoding.GetByteCount((string)null), Throws.ArgumentNullException);
        Assert.That(() => encoding.GetByteCount((char[])null), Throws.ArgumentNullException);
        Assert.That(() => encoding.GetByteCount((char[])null, 0, 0), Throws.ArgumentNullException);
        Assert.That(() => encoding.GetByteCount(buffer, -1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetByteCount(buffer, 5, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetByteCount(buffer, 0, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetByteCount(buffer, 0, 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetByteCount(ReadOnlySpan<char>.Empty), Throws.Nothing);
    }

    [Test]
    public void GetByteCountCallsEncode()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();

        encoding.GetByteCount(text.AsSpan());

        Assert.That(encoding.EncodeCalls, Is.EqualTo(1));
        Assert.That(encoding.LastEncodeText, Is.EqualTo(text));
    }

    // It's not possible to Moq methods that uses Span<T> or ReadOnlySpan<T>
    private sealed class DummyEncoding : SimpleSpanEncoding
    {
        public DummyEncoding()
            : base(999)
        {
        }

        public int GetByteCountSpanCalls { get; private set; }

        public int EncodeCalls { get; private set; }

        public string LastGetByteCountText { get; private set; }

        public string LastEncodeText { get; private set; }

        public override string EncodingName => nameof(DummyEncoding);

        public override int GetByteCount(ReadOnlySpan<char> chars)
        {
            GetByteCountSpanCalls++;
            LastGetByteCountText = new string(chars);
            return base.GetByteCount(chars);
        }

        public override int GetMaxByteCount(int charCount) => 1;

        public override int GetMaxCharCount(int byteCount) => 1;

        protected override void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer)
        {
            string data = Encoding.ASCII.GetString(bytes);
            buffer.Write(data.AsSpan());
        }

        protected override void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer, bool isFallbackText = false)
        {
            EncodeCalls++;
            LastEncodeText = new string(chars);
            byte[] data = Encoding.ASCII.GetBytes(new string(chars));
            buffer.Write(data);
        }
    }
}
