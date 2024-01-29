namespace Yarhl.UnitTests.IO.Serialization;

using System;
using NUnit.Framework;
using Yarhl.IO;
using Yarhl.IO.Serialization;

[TestFixture]
public class BinarySerializerTests
{
    [Test]
    public void SerializeByGenericType()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var obj = new SimpleType { Value = 0x0A, };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);
        serializer.Serialize<SimpleType>(obj);

        AssertBinary(stream, data);
    }

    [Test]
    public void SerializeByTypeArg()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var obj = new SimpleType { Value = 0x0A, };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);
        serializer.Serialize(typeof(SimpleType), obj);

        AssertBinary(stream, data);
    }

    [Test]
    public void SerializeByStaticGenericType()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var obj = new SimpleType { Value = 0x0A, };

        using var stream = new DataStream();
        BinarySerializer.Serialize<SimpleType>(stream, obj);

        AssertBinary(stream, data);
    }

    [Test]
    public void SerializeByStaticTypeArg()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var obj = new SimpleType { Value = 0x0A, };

        using var stream = new DataStream();
        BinarySerializer.Serialize(stream, typeof(SimpleType), obj);

        AssertBinary(stream, data);
    }

    [Test]
    public void SerializeIncludesInheritedFields()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00, 0xFE, 0xCA };
        var obj = new InheritedType { Value = 0x0A, NewValue = 0xCAFE };

        AssertSerialization(obj, data);
    }

    [Test]
    public void SerializeBaseType()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var obj = new InheritedType { Value = 0x0A, NewValue = 0xCAFE };

        using var stream = new DataStream();
        BinarySerializer.Serialize(stream, typeof(SimpleType), obj);

        AssertBinary(stream, data);
    }

    [Test]
    public void SerializeIntegerTypes()
    {
        var obj = new TypeWithIntegers {
            CharValue = 'Ω',
            ByteValue = 0x84,
            SByteValue = -12,
            UShortValue = 0x807F,
            ShortValue = -16,
            UIntValue = 0x12345678,
            IntegerValue = -42,
            ULongValue = 0x8000000000002A,
            LongValue = -2L,
        };

        byte[] data = {
            0xCE, 0xA9, // char un UTF-8 (default encoding)
            0x84,
            0xF4,
            0x7F, 0x80,
            0xF0, 0xFF,
            0x78, 0x56, 0x34, 0x12,
            0xD6, 0xFF, 0xFF, 0xFF,
            0x2A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80,
            0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
        };

        AssertSerialization(obj, data);
    }

    [Test]
    public void SerializeDecimalTypes()
    {
        byte[] data = {
            0xC3, 0xF5, 0x48, 0x40,
            0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0xC0,
        };
        var obj = new TypeWithDecimals {
            SingleValue = 3.14f,
            DoubleValue = -3.14d,
        };

        AssertSerialization(obj, data);
    }

    [Test]
    public void SerializeStruct()
    {
        var obj = new MultiPropertyStruct {
            IntegerValue = 1,
            LongValue = 2,
            TextValue = "yarhl",
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            (byte)'y', (byte)'a', (byte)'r', (byte)'h', (byte)'l', (byte)'\0',
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeNestedObject()
    {
        var obj = new TypeWithNestedObject() {
            IntegerValue = 10,
            ComplexValue = new TypeWithNestedObject.NestedType {
                NestedValue = 1,
            },
            AnotherIntegerValue = 20,
        };

        byte[] expected = {
            0x0A, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeIgnorePropertiesViaAttribute()
    {
        var obj = new TypeWithIgnoredProperties {
            LongValue = 2,
            IgnoredIntegerValue = 42,
        };

        byte[] expected = {
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeBooleanType()
    {
        var obj = new TypeWithBooleanDefaultAttribute() {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };

        byte[] expected = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(obj, expected);

        obj.BooleanValue = true;
        expected[4] = 0x01;
        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeBooleanWithDefinedValues()
    {
        var falseObj = new TypeWithBooleanDefinedValue() {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };
        byte[] expectedFalse = {
            0x01, 0x00, 0x00, 0x00,
            0xD6, 0xFF,
            0x03, 0x00, 0x00, 0x00,
        };

        var trueObj = new TypeWithBooleanDefinedValue() {
            BeforeValue = 1,
            BooleanValue = true,
            AfterValue = 3,
        };
        byte[] expectedTrue = {
            0x01, 0x00, 0x00, 0x00,
            0x2A, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(falseObj, expectedFalse);
        AssertSerialization(trueObj, expectedTrue);
    }

    [Test]
    public void SerializeBooleanWithTextValues()
    {
        var falseObj = new TypeWithBooleanTextValue() {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };
        byte[] expectedFalse = {
            0x01, 0x00, 0x00, 0x00,
            (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', (byte)'\0',
            0x03, 0x00, 0x00, 0x00,
        };

        var trueObj = new TypeWithBooleanTextValue() {
            BeforeValue = 1,
            BooleanValue = true,
            AfterValue = 3,
        };
        byte[] expectedTrue = {
            0x01, 0x00, 0x00, 0x00,
            (byte)'t', (byte)'r', (byte)'u', (byte)'e', (byte)'\0',
            0x03, 0x00, 0x00, 0x00,
        };

        AssertSerialization(falseObj, expectedFalse);
        AssertSerialization(trueObj, expectedTrue);
    }

    [Test]
    public void TrySerializeBooleanWithoutAttributeThrowsException()
    {
        var obj = new TypeWithBooleanWithoutAttribute() {
            BeforeValue = 1,
            BooleanValue = true,
            AfterValue = 3,
        };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);

        _ = Assert.Throws<FormatException>(() => serializer.Serialize(obj));
    }

    [Test]
    public void SerializeInt24()
    {
        var obj = new TypeWithInt24 {
            Int24Value = 0x7F_FC0FFE,
        };

        byte[] expected = {
            0xFE, 0x0F, 0xFC,
        };

        AssertSerialization(obj, expected);
    }

    [Test]
    public void SerializeStringWithoutAttributeUsesDefaultWriterSettings()
    {
        var obj = new TypeWithStringWithoutAttribute {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 3,
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
        var obj = new TypeWithStringDefaultAttribute() {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 3,
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
        var obj = new TypeWithStringVariableSize() {
            BeforeValue = 1,
            StringValue = "あ",
            AfterValue = 4,
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
        var obj = new TypeWithStringFixedSize() {
            BeforeValue = 1,
            StringValue = "あ",
            AfterValue = 4,
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
        var obj = new TypeWithStringDefinedEncoding() {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 4,
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
        var obj = new TypeWithStringInvalidEncoding() {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 4,
        };

        using var stream = new DataStream();
        var serializer = new BinarySerializer(stream);

        _ = Assert.Throws<NotSupportedException>(() => serializer.Serialize(obj));
    }

    [Test]
    public void SerializeObjectWithSpecificEndianness()
    {
        var obj = new TypeWithEndiannessChanges() {
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
    public void SerializeEnumNoAttribute()
    {
        byte[] data = { 0x2A, 0x00, };

        var obj = new TypeWithEnumNoAttribute {
            EnumValue = SerializableEnum.Value42,
        };

        AssertSerialization(obj, data);
    }

    [Test]
    public void SerializeEnumDefaultAttribute()
    {
        byte[] data = { 0x2A, 0x00, };

        var obj = new TypeWithEnumDefaultAttribute {
            EnumValue = SerializableEnum.Value42,
        };

        AssertSerialization(obj, data);
    }

    [Test]
    public void SerializeEnumOverwritingType()
    {
        byte[] data = { 0x01, 0x00, 0x00, 0x00 };

        var obj = new TypeWithEnumWithOverwrittenType {
            EnumValue = SerializableEnum.None,
        };

        AssertSerialization(obj, data);
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
        Assert.That(actual.Length, Is.EqualTo(expected.Length), "Stream size mismatch");

        byte[] actualData = new byte[expected.Length];
        Assert.Multiple(() => {
            actual.Position = 0;
            int read = actual.Read(actualData);

            Assert.That(read, Is.EqualTo(expected.Length), "Read mismatch");
            Assert.That(actualData, Is.EquivalentTo(expected));
        });
    }
}
