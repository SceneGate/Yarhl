namespace Yarhl.UnitTests.IO.Serialization;

using System;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.IO;
using Yarhl.IO.Serialization;
using Yarhl.IO.Serialization.Attributes;

[TestFixture]
public class BinaryDeserializerTests
{
    [Test]
    public void DeserializeFullObject()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };
        var expected = new ComplexObject {
            IntegerValue = 1,
            LongValue = 2L,
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeNestedObject()
    {
        byte[] data = {
            0x0A, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x00, 0x00,
        };
        var expected = new NestedObject {
            IntegerValue = 10,
            ComplexValue = new ComplexObject {
                IntegerValue = 1,
                LongValue = 2L,
                IgnoredIntegerValue = 0,
                AnotherIntegerValue = 3,
            },
            AnotherIntegerValue = 20,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeBoolean()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithDefaultBooleanAttribute {
            IntegerValue = 1,
            BooleanValue = false,
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeBooleanWithCustomTrueValue()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x74, 0x72, 0x75, 0x65, 0x00, // "true"
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithCustomBooleanAttribute {
            IntegerValue = 1,
            BooleanValue = true,
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void TryDeserializeBooleanWithoutAttributeThrowsException()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);
        Assert.That(
            () => deserializer.Deserialize<ObjectWithoutBooleanAttribute>(),
            Throws.InstanceOf<FormatException>());
    }

    [Test]
    public void DeserializeInt24()
    {
        byte[] data = {
            0x01, 0x00, 0x00,
        };

        var expected = new ObjectWithInt24 {
            Int24Value = 1,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeStringWithoutAttributeUsesDefaultReaderSettings()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithoutStringAttribute {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeStringWithDefaultAttributeUsesDefaultReaderSettings()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithDefaultStringAttribute {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeStringWithSizeType()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x03, 0x00, 0xE3, 0x81, 0x82,
            0x04, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithCustomStringAttributeSizeUshort {
            IntegerValue = 1,
            StringValue = "あ",
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 4,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeStringWithFixedSize()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithCustomStringAttributeFixedSize {
            IntegerValue = 1,
            StringValue = "あ",
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeStringWithDifferentEncoding()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x82, 0xA0, 0x83, 0x41, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithCustomStringAttributeCustomEncoding {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 0,
            AnotherIntegerValue = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void TryDeserializeStringWithUnknownEncodingThrowsException()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x82, 0xA0, 0x83, 0x41, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);

        Assert.That(
            () => deserializer.Deserialize<ObjectWithCustomStringAttributeUnknownEncoding>(),
            Throws.InstanceOf<NotSupportedException>());
    }

    [Test]
    public void DeserializeObjectWithSpecificEndianness()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x02,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new ObjectWithForcedEndianness {
            LittleEndianInteger = 1,
            BigEndianInteger = 2,
            DefaultEndianInteger = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeEnum()
    {
        byte[] data = { 0x01 };

        var expected = new ObjectWithEnum { EnumValue = Enum1.Value2 };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void TryDeserializeNullableThrowsException()
    {
        var stream = new DataStream();
        stream.Write(new byte[4]);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);

        Assert.That(
            () => deserializer.Deserialize<ObjectWithNullable>(),
            Throws.InstanceOf<FormatException>());
    }

    private static void AssertDeserialization<T>(byte[] data, T expected)
    {
        var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);
        T obj = deserializer.Deserialize<T>();

        _ = obj.Should().BeEquivalentTo(expected);
    }
}
