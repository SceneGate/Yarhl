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

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ComplexObject
    {
        public int IntegerValue { get; set; }

        public long LongValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class NestedObject
    {
        public int IntegerValue { get; set; }

        public ComplexObject ComplexValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithDefaultBooleanAttribute
    {
        public int IntegerValue { get; set; }

        [BinaryBoolean]
        public bool BooleanValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithoutBooleanAttribute
    {
        public int IntegerValue { get; set; }

        public bool BooleanValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithCustomBooleanAttribute
    {
        public int IntegerValue { get; set; }

        [BinaryBoolean(ReadAs = typeof(string), TrueValue = "true")]
        public bool BooleanValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithDefaultStringAttribute
    {
        public int IntegerValue { get; set; }

        [BinaryString]
        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithoutStringAttribute
    {
        public int IntegerValue { get; set; }

        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithCustomStringAttributeSizeUshort
    {
        public int IntegerValue { get; set; }

        [BinaryString(SizeType = typeof(ushort), Terminator = "")]
        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithCustomStringAttributeFixedSize
    {
        public int IntegerValue { get; set; }

        [BinaryString(FixedSize = 3, Terminator = "")]
        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithCustomStringAttributeCustomEncoding
    {
        public int IntegerValue { get; set; }

        [BinaryString(CodePage = 932)]
        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithCustomStringAttributeUnknownEncoding
    {
        public int IntegerValue { get; set; }

        [BinaryString(CodePage = 666)]
        public string StringValue { get; set; }

        [BinaryIgnore]
        public int IgnoredIntegerValue { get; set; }

        public int AnotherIntegerValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithForcedEndianness
    {
        [BinaryForceEndianness(EndiannessMode.LittleEndian)]
        public int LittleEndianInteger { get; set; }

        [BinaryForceEndianness(EndiannessMode.BigEndian)]
        public int BigEndianInteger { get; set; }

        public int DefaultEndianInteger { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithEnum
    {
        [BinaryEnum(ReadAs = typeof(byte))]
        public Enum1 EnumValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithInt24
    {
        [BinaryInt24]
        public int Int24Value { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithNullable
    {
        public int? NullValue { get; set; }
    }

    private enum Enum1
    {
        Value1,
        Value2,
        Value3,
    }
}
