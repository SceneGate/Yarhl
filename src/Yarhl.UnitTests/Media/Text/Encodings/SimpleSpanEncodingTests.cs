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

    [Test]
    public void GetBytesAllocateBufferUsesGetByteCount()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();

        var result = encoding.GetBytes(text.AsSpan());

        Assert.That(encoding.LastGetByteCountText, Is.EqualTo(text));
        Assert.That(result.Length, Is.EqualTo(text.Length)); // ASCII encoding
        Assert.That(encoding.GetByteCountSpanCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetBytesAllocateBufferOverloadsUseSpanMethod()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding;

        baseEncoding.GetBytes(text);
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo(text));
        Assert.That(encoding.LastEncodeText, Is.EqualTo(text));

        baseEncoding.GetBytes(text.ToCharArray());
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo(text));
        Assert.That(encoding.LastEncodeText, Is.EqualTo(text));

        baseEncoding.GetBytes(text.ToCharArray(), 1, 2);
        Assert.That(encoding.LastGetByteCountText, Is.EqualTo("af"));
        Assert.That(encoding.LastEncodeText, Is.EqualTo("af"));

        Assert.That(encoding.GetByteCountSpanCalls, Is.EqualTo(3));
        Assert.That(encoding.EncodeCalls, Is.EqualTo(3 * 2)); // buffer length + encode
    }

    [Test]
    public void GetBytesAllocateBufferCallsEncode()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();

        encoding.GetBytes(text.AsSpan());

        Assert.That(encoding.LastEncodeText, Is.EqualTo(text));
        Assert.That(encoding.EncodeCalls, Is.EqualTo(2)); // buffer length + encode
    }

    [Test]
    public void GetBytesAllocateBufferGuards()
    {
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding; // to force the non ReadOnlySpan<char> overloads

        Assert.That(() => baseEncoding.GetBytes((string)null), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes((char[])null), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes((char[])null, 0, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes(new char[3], -1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 5, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetBytes(ReadOnlySpan<char>.Empty), Throws.Nothing);
    }

    [Test]
    public void GetBytesDoesNotUseGetByteCountButReturnsCorrectLength()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();
        Span<byte> output = new byte[10];

        var result = encoding.GetBytes(text.AsSpan(), output);

        Assert.That(result, Is.EqualTo(text.Length)); // ASCII encoding
        Assert.That(encoding.GetByteCountSpanCalls, Is.EqualTo(0));
    }

    [Test]
    public void GetBytesOverloadsUseSpanMethod()
    {
        string text = "cafe";
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding;
        byte[] output = new byte[5];
        byte[] expected = new byte[] { 0x00, (byte)'a', (byte)'f', 0x00, 0x00 };

        baseEncoding.GetBytes(text.ToCharArray(), 1, 2, output, 1);
        Assert.That(encoding.LastEncodeText, Is.EqualTo("af"));
        Assert.That(output, Is.EquivalentTo(expected));

        baseEncoding.GetBytes(text, 1, 2, output, 1);
        Assert.That(encoding.LastEncodeText, Is.EqualTo("af"));
        Assert.That(output, Is.EquivalentTo(expected));

        Assert.That(encoding.GetByteCountSpanCalls, Is.EqualTo(0));
        Assert.That(encoding.EncodeCalls, Is.EqualTo(2));
    }

    [Test]
    public void GetBytesCallsEncode()
    {
        string text = "cafe";
        byte[] output = new byte[5];
        var encoding = new DummyEncoding();

        encoding.GetBytes(text.AsSpan(), output);

        Assert.That(encoding.LastEncodeText, Is.EqualTo(text));
        Assert.That(encoding.EncodeCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetBytesGuards()
    {
        var output = new byte[5];
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding; // to force the non ReadOnlySpan<char> overloads

        Assert.That(() => baseEncoding.GetBytes((char[])null, 0, 0, output, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, 5, null, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes(new char[3], -1, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 5, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, -1, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, 5, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, 5, output, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes(new char[3], 0, 5, output, 6), Throws.InstanceOf<ArgumentOutOfRangeException>());

        Assert.That(() => baseEncoding.GetBytes((string)null, 0, 0, output, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes("abc", 0, 5, null, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetBytes("abc", -1, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes("abc", 5, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes("abc", 0, -1, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes("abc", 0, 5, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes("abc", 0, 5, output, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetBytes("abc", 0, 5, output, 6), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void GetCharCountsCallSpanMethod()
    {
        var encoding = new DummyEncoding();
        byte[] data = new byte[] { (byte)'a', (byte)'b', (byte)'c' };

        encoding.GetCharCount(data);
        Assert.That(encoding.LastGetCharCountData, Is.EquivalentTo(data));

        encoding.GetCharCount(data, 1, 1);
        Assert.That(encoding.LastGetCharCountData, Is.EquivalentTo(new byte[] { (byte)'b' }));

        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(2));
    }

    [Test]
    public void GetCharCountsGuards()
    {
        var buffer = new byte[3];
        var encoding = new DummyEncoding();

        Assert.That(() => encoding.GetCharCount((byte[])null), Throws.ArgumentNullException);
        Assert.That(() => encoding.GetCharCount((byte[])null, 0, 0), Throws.ArgumentNullException);
        Assert.That(() => encoding.GetCharCount(buffer, -1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetCharCount(buffer, 5, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetCharCount(buffer, 0, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetCharCount(buffer, 0, 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetCharCount(ReadOnlySpan<byte>.Empty), Throws.Nothing);
    }

    [Test]
    public void GetCharCountCallsDecode()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();

        encoding.GetCharCount(data.AsSpan());

        Assert.That(encoding.DecodeCalls, Is.EqualTo(1));
        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data));
    }

    [Test]
    public void GetCharsAllocateBufferUsesGetCharCount()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();

        var result = encoding.GetChars(data.AsSpan());

        Assert.That(encoding.LastGetCharCountData, Is.EquivalentTo(data));
        Assert.That(result.Length, Is.EqualTo(data.Length)); // ASCII encoding
        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetCharsAllocateBufferOverloadsUseSpanMethod()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        byte[] partialData = data[1..3];
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding;

        baseEncoding.GetChars(data);
        Assert.That(encoding.LastGetCharCountData, Is.EquivalentTo(data));
        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data));

        baseEncoding.GetChars(data, 1, 2);
        Assert.That(encoding.LastGetCharCountData, Is.EqualTo(partialData));
        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(partialData));

        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(2));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(2 * 2)); // buffer length + encode
    }

    [Test]
    public void GetCharsAllocateBufferCallsDecode()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();

        encoding.GetChars(data.AsSpan());

        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(2)); // buffer length + encode
    }

    [Test]
    public void GetCharsAllocateBufferGuards()
    {
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding; // to force the non ReadOnlySpan<byte> overloads

        Assert.That(() => baseEncoding.GetChars((byte[])null), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetChars((byte[])null, 0, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetChars(new byte[3], -1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 5, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => encoding.GetChars(ReadOnlySpan<byte>.Empty), Throws.Nothing);
    }

    [Test]
    public void GetCharsDoesNotUseGetCharCountButReturnsCorrectLength()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();
        Span<char> output = new char[10];

        var result = encoding.GetChars(data.AsSpan(), output);

        Assert.That(result, Is.EqualTo(data.Length)); // ASCII encoding
        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(0));
    }

    [Test]
    public void GetCharOverloadsUseSpanMethod()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding;
        char[] output = new char[5];
        char[] expected = new char[] { '\0', 'a', 'f', '\0', '\0' };

        baseEncoding.GetChars(data, 1, 2, output, 1);
        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data[1..3]));
        Assert.That(output, Is.EquivalentTo(expected));

        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(0));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetCharCallsDecode()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        char[] output = new char[5];
        var encoding = new DummyEncoding();

        encoding.GetChars(data.AsSpan(), output);

        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetCharGuards()
    {
        var output = new char[5];
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding; // to force the non ReadOnlySpan<byte> overloads

        Assert.That(() => baseEncoding.GetChars((byte[])null, 0, 0, output, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, 5, null, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetChars(new byte[3], -1, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 5, 0, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, -1, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, 5, output, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, 5, output, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetChars(new byte[3], 0, 5, output, 6), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void GetStringUsesGetCharCount()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();

        var result = encoding.GetString(data);

        Assert.That(encoding.LastGetCharCountData, Is.EquivalentTo(data));
        Assert.That(result.Length, Is.EqualTo(data.Length)); // ASCII encoding
        Assert.That(encoding.GetCharCountSpanCalls, Is.EqualTo(1));
    }

    [Test]
    public void GetStringCallsDecode()
    {
        byte[] data = Encoding.ASCII.GetBytes("cafe");
        var encoding = new DummyEncoding();
        string expected = "cafe";

        var actual = encoding.GetString(data);

        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(2)); // buffer length + encode
        Assert.That(actual, Is.EqualTo(expected));

        var partialActual = encoding.GetString(data, 1, 2);
        Assert.That(encoding.LastDecodeData, Is.EquivalentTo(data[1..3]));
        Assert.That(encoding.DecodeCalls, Is.EqualTo(2 * 2)); // buffer length + encode
        Assert.That(partialActual, Is.EqualTo(expected[1..3]));
    }

    [Test]
    public void GetStringGuards()
    {
        var encoding = new DummyEncoding();
        var baseEncoding = (Encoding)encoding;

        Assert.That(() => baseEncoding.GetString((byte[])null), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetString((byte[])null, 0, 0), Throws.ArgumentNullException);
        Assert.That(() => baseEncoding.GetString(new byte[3], -1, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetString(new byte[3], 5, 0), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetString(new byte[3], 0, -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
        Assert.That(() => baseEncoding.GetString(new byte[3], 0, 5), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void EncodeFallbackWithSingleCharCodePoints()
    {
        char errorChar = DummyEncoding.EncodingErrorSimpleCodePointChar;
        int errorIndex = 1;
        string textString = $"a{errorChar}o";

        var fallbackBuffer = new Mock<EncoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(errorChar, errorIndex))
            .Returns(false);

        var fallback = new Mock<EncoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new byte[10];

        encoding.GetBytes(textString, output); // use an overload that triggers Encode only once

        fallbackBuffer.Verify(x => x.Fallback(errorChar, errorIndex), Times.Once);
    }

    [Test]
    public void EncodeFallbackWithMultiCharCodePoints()
    {
        string errorRune = DummyEncoding.EncodingErrorMultiCodePointText;
        int errorIndex = 1;
        string textString = "u👀u";

        var fallbackBuffer = new Mock<EncoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(errorRune[0], errorRune[1], errorIndex))
            .Returns(false);

        var fallback = new Mock<EncoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new byte[10];

        encoding.GetBytes(textString, output); // use an overload that triggers Encode only once

        fallbackBuffer.Verify(x => x.Fallback(errorRune[0], errorRune[1], errorIndex), Times.Once);
    }

    [Test]
    public void EncodeFallbackReturningFalseDoNotEncodeError()
    {
        char errorChar = DummyEncoding.EncodingErrorSimpleCodePointChar;
        int errorIndex = 1;
        string textString = $"a{errorChar}o";

        var fallbackBuffer = new Mock<EncoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(errorChar, errorIndex))
            .Returns(false);

        var fallback = new Mock<EncoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new byte[10];

        encoding.GetBytes(textString, output); // use an overload that triggers Encode only once

        Assert.That(encoding.EncodeCalls, Is.EqualTo(1));
    }

    [Test]
    public void EncodeFallbackReturningCharsAreEncoded()
    {
        char errorChar = DummyEncoding.EncodingErrorSimpleCodePointChar;
        int errorIndex = 1;
        string textString = $"a{errorChar}o";

        string errorToEncode = "1993";
        int returnedChars = 0;

        byte[] expectedResult = Encoding.ASCII.GetBytes($"a{errorToEncode}o");

        var fallbackBuffer = new Mock<EncoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(errorChar, errorIndex))
            .Returns(true);
        fallbackBuffer.Setup(x => x.Remaining)
            .Returns(() => errorToEncode.Length - returnedChars);
        fallbackBuffer.Setup(x => x.GetNextChar())
            .Returns(() => errorToEncode[returnedChars++]);

        var fallback = new Mock<EncoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new byte[10];

        int encoded = encoding.GetBytes(textString, output);

        Assert.That(encoding.EncodeCalls, Is.EqualTo(2)); // normal + 1 error
        Assert.That(output[0..encoded], Is.EquivalentTo(expectedResult));
        Assert.That(encoded, Is.EqualTo(expectedResult.Length));
    }

    [Test]
    public void EncodeFallbackReturningInvalidCharsThrows()
    {
        char errorChar = DummyEncoding.EncodingErrorSimpleCodePointChar;
        int errorIndex = 1;
        string textString = $"a{errorChar}o";

        string errorToEncode = $"{errorChar}";
        int returnedChars = 0;

        var fallbackBuffer = new Mock<EncoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(errorChar, errorIndex))
            .Returns(true);
        fallbackBuffer.Setup(x => x.Remaining)
            .Returns(() => errorToEncode.Length - returnedChars);
        fallbackBuffer.Setup(x => x.GetNextChar())
            .Returns(() => errorToEncode[returnedChars++]);

        var fallback = new Mock<EncoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new byte[10];

        Assert.That(() => encoding.GetBytes(textString, output), Throws.InstanceOf<FormatException>());
    }

    [Test]
    public void DecodeIncludesOutputFromDecoderFallback()
    {
        byte[] input = new byte[] {
            (byte)'h', (byte)'e',
            DummyEncoding.DecodingInvalidByte,
            (byte)'l', (byte)'o',
        };
        int errorIndex = 2;

        string errorToEncode = "?!?";
        int returnedChars = 0;

        char[] expectedResult = "he?!?lo".ToCharArray();

        byte[] expectedInvalidBytes = new byte[] { DummyEncoding.DecodingInvalidByte };
        var fallbackBuffer = new Mock<DecoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(expectedInvalidBytes, errorIndex))
            .Returns(true);
        fallbackBuffer.Setup(x => x.Remaining)
            .Returns(() => errorToEncode.Length - returnedChars);
        fallbackBuffer.Setup(x => x.GetNextChar())
            .Returns(() => errorToEncode[returnedChars++]);

        var fallback = new Mock<DecoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new char[10];

        int decoded = encoding.GetChars(input, output);

        Assert.That(encoding.DecodeCalls, Is.EqualTo(1));
        Assert.That(output[0..decoded], Is.EquivalentTo(expectedResult));
        Assert.That(decoded, Is.EqualTo(expectedResult.Length));
    }

    [Test]
    public void DecodeDoesNotIncludesOutputFromDecoderFallbackReturningFalse()
    {
        byte[] input = new byte[] {
            (byte)'h', (byte)'e',
            DummyEncoding.DecodingInvalidByte,
            (byte)'l', (byte)'o',
        };
        int errorIndex = 2;

        string errorToEncode = "?!?";
        int returnedChars = 0;

        char[] expectedResult = "helo".ToCharArray();

        byte[] expectedInvalidBytes = new byte[] { DummyEncoding.DecodingInvalidByte };
        var fallbackBuffer = new Mock<DecoderFallbackBuffer>();
        fallbackBuffer.Setup(x => x.Fallback(expectedInvalidBytes, errorIndex))
            .Returns(false);
        fallbackBuffer.Setup(x => x.Remaining)
            .Returns(() => errorToEncode.Length - returnedChars);
        fallbackBuffer.Setup(x => x.GetNextChar())
            .Returns(() => errorToEncode[returnedChars++]);

        var fallback = new Mock<DecoderFallback>();
        fallback.Setup(x => x.CreateFallbackBuffer())
            .Returns(fallbackBuffer.Object);

        var encoding = new DummyEncoding(fallback.Object);
        var output = new char[10];

        int decoded = encoding.GetChars(input, output);

        Assert.That(encoding.DecodeCalls, Is.EqualTo(1));
        Assert.That(output[0..decoded], Is.EquivalentTo(expectedResult));
        Assert.That(decoded, Is.EqualTo(expectedResult.Length));
    }

    // It's not possible to Moq methods that uses Span<T> or ReadOnlySpan<T>
    private sealed class DummyEncoding : SimpleSpanEncoding
    {
        public DummyEncoding()
            : base(999)
        {
        }

        public DummyEncoding(EncoderFallback encoderFallback)
            : base(999, encoderFallback, DecoderFallback.ExceptionFallback)
        {
        }

        public DummyEncoding(DecoderFallback decoderFallback)
            : base(999, EncoderFallback.ExceptionFallback, decoderFallback)
        {
        }

        public static char EncodingErrorSimpleCodePointChar => 'ñ';

        public static string EncodingErrorMultiCodePointText => "👀";

        public static byte DecodingInvalidByte => 0xCA;

        public int GetCharCountSpanCalls { get; private set; }

        public int GetByteCountSpanCalls { get; private set; }

        public int EncodeCalls { get; private set; }

        public int DecodeCalls { get; private set; }

        public string LastGetByteCountText { get; private set; }

        public byte[] LastGetCharCountData { get; private set; }

        public string LastEncodeText { get; private set; }

        public byte[] LastDecodeData { get; private set; }

        public override string EncodingName => nameof(DummyEncoding);

        public override int GetByteCount(ReadOnlySpan<char> chars)
        {
            GetByteCountSpanCalls++;
            LastGetByteCountText = new string(chars);
            return base.GetByteCount(chars);
        }

        public override int GetCharCount(ReadOnlySpan<byte> bytes)
        {
            GetCharCountSpanCalls++;
            LastGetCharCountData = bytes.ToArray();
            return base.GetCharCount(bytes);
        }

        public override int GetMaxByteCount(int charCount) => 1;

        public override int GetMaxCharCount(int byteCount) => 1;

        protected override void Decode(ReadOnlySpan<byte> bytes, SpanStream<char> buffer)
        {
            DecodeCalls++;
            LastDecodeData = bytes.ToArray();

            for (int i = 0; i < bytes.Length; i++) {
                if (bytes[i] == DecodingInvalidByte) {
                    DecodeUnknownBytes(buffer, i, bytes[i]);
                } else {
                    char decodedChar = (char)bytes[i]; // ~ASCII
                    buffer.Write(decodedChar);
                }
            }
        }

        protected override void Encode(ReadOnlySpan<char> chars, SpanStream<byte> buffer, bool isFallbackText = false)
        {
            EncodeCalls++;
            LastEncodeText = new string(chars);

            int index = 0;
            while (index < chars.Length) {
                var result = Rune.DecodeFromUtf16(chars[index..], out Rune rune, out int advanced);
                if (result != System.Buffers.OperationStatus.Done) {
                    throw new Exception("WTF, the input is invalid");
                }

                if (rune.Value == EncodingErrorSimpleCodePointChar) {
                    EncodeUnknownChar(buffer, rune.Value, index, isFallbackText);
                } else if (rune.ToString() == EncodingErrorMultiCodePointText) {
                    EncodeUnknownChar(buffer, rune.Value, index, isFallbackText);
                } else {
                    byte encodedChar = (byte)rune.Value; // ~ASCII
                    buffer.Write(encodedChar);
                }

                index += advanced;
            }
        }
    }
}
