namespace Yarhl.UnitTests.IO.Serialization;

using System;
using System.Linq;
using NUnit.Framework;
using Yarhl.IO;
using Yarhl.IO.Serialization;
using Yarhl.IO.Serialization.Attributes;

[TestFixture]
public class BinarySerializerTests
{
    [Test]
    public void SerializeMultipleProperties()
    {
        var obj = new ComplexObject {
            IntegerValue = 1,
            LongValue = 2,
            IgnoredIntegerValue = 3,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeNestedObject()
    {
        var obj = new NestedObject() {
            IntegerValue = 10,
            ComplexValue = new ComplexObject {
                IntegerValue = 1,
                LongValue = 2,
                IgnoredIntegerValue = 3,
                AnotherIntegerValue = 4,
            },
            AnotherIntegerValue = 20,
        };

        byte[] expected = {
            0x0A, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeBooleanType()
    {
        var obj = new ObjectWithDefaultBooleanAttribute() {
            IntegerValue = 1,
            BooleanValue = false,
            IgnoredIntegerValue = 3,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeBooleanWithCustomFalseValue()
    {
        var obj = new ObjectWithCustomBooleanAttribute() {
            IntegerValue = 1,
            BooleanValue = false,
            IgnoredIntegerValue = 5,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x66, 0x61, 0x6C, 0x73, 0x65, 0x00, // "false"
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void TrySerializeBooleanWithoutAttributeThrowsException()
    {
        var obj = new ObjectWithoutBooleanAttribute() {
            IntegerValue = 1,
            BooleanValue = true,
            IgnoredIntegerValue = 3,
            AnotherIntegerValue = 4,
        };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);

        _ = Assert.Throws<FormatException>(() => serializer.Serialize(obj));
    }

    [Test]
    public void SerializeStringWithoutAttributeUsesDefaultWriterSettings()
    {
        var obj = new ObjectWithoutStringAttribute {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 3,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeStringWithDefaultAttributeUsesDefaultWriterSettings()
    {
        var obj = new ObjectWithDefaultStringAttribute() {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 3,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82, 0xE3, 0x82, 0xA2, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeStringWithSizeType()
    {
        var obj = new ObjectWithCustomStringAttributeSizeUshort() {
            IntegerValue = 1,
            StringValue = "あ",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x03, 0x00, 0xE3, 0x81, 0x82,
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeStringWithFixedSize()
    {
        var obj = new ObjectWithCustomStringAttributeFixedSize() {
            IntegerValue = 1,
            StringValue = "あ",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0xE3, 0x81, 0x82,
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeStringWithDifferentEncoding()
    {
        var obj = new ObjectWithCustomStringAttributeCustomEncoding() {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 4,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x82, 0xA0, 0x83, 0x41, 0x00,
            0x04, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void TrySerializeStringWithUnknownEncodingThrowsException()
    {
        var obj = new ObjectWithCustomStringAttributeUnknownEncoding() {
            IntegerValue = 1,
            StringValue = "あア",
            IgnoredIntegerValue = 2,
            AnotherIntegerValue = 4,
        };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);

        _ = Assert.Throws<NotSupportedException>(() => serializer.Serialize(obj));
    }

    [Test]
    public void SerializeObjectWithSpecificEndianness()
    {
        var obj = new ObjectWithForcedEndianness() {
            LittleEndianInteger = 1,
            BigEndianInteger = 2,
            DefaultEndianInteger = 3,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x02,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeEnum()
    {
        var obj = new ObjectWithEnum() {
            EnumValue = Enum1.Value2,
        };

        byte[] expected = { 0x01 };

        AssertSerialization(obj, expected);
    }

    private static void AssertSerialization<T>(T obj, byte[] expected)
    {
        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);

        serializer.Serialize(obj);

        AssertBinary(stream, expected);
    }

    private static void AssertBinary(DataStream actual, byte[] expected)
    {
        Assert.That(expected.Length, Is.EqualTo(actual.Length));

        byte[] actualData = new byte[expected.Length];
        Assert.Multiple(() => {
            actual.Position = 0;
            int read = actual.Read(actualData);

            Assert.That(read, Is.EqualTo(expected.Length));
            Assert.That(actualData, Is.EquivalentTo(actualData));
        });
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

        [BinaryBoolean(WriteAs = typeof(string), TrueValue = "true", FalseValue = "false")]
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

    private enum Enum1
    {
        Value1,
        Value2,
        Value3,
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithEnum
    {
        [BinaryEnum(WriteAs = typeof(byte))]
        public Enum1 EnumValue { get; set; }
    }

    [Yarhl.IO.Serialization.Attributes.Serializable]
    private class ObjectWithInt24
    {
        [BinaryInt24]
        public int Int24Value { get; set; }
    }
}
