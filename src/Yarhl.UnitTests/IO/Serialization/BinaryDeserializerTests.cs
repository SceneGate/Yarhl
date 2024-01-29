namespace Yarhl.UnitTests.IO.Serialization;

using System;
using FluentAssertions;
using NUnit.Framework;
using Yarhl.IO;
using Yarhl.IO.Serialization;

[TestFixture]
public class BinaryDeserializerTests
{
    [Test]
    public void DeserializeByGenericType()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var expected = new SimpleType { Value = 10 };
        using var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);
        SimpleType obj = deserializer.Deserialize<SimpleType>();

        _ = obj.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DeserializeByTypeArg()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var expected = new SimpleType { Value = 10 };
        using var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);
        object obj = deserializer.Deserialize(typeof(SimpleType));

        _ = obj.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DeserializeStaticByGenericType()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var expected = new SimpleType { Value = 10 };
        using var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        SimpleType obj = BinaryDeserializer.Deserialize<SimpleType>(stream);

        _ = obj.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DeserializeStaticByTypeArg()
    {
        byte[] data = { 0x0A, 0x00, 0x00, 0x00 };
        var expected = new SimpleType { Value = 10 };
        using var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        object obj = BinaryDeserializer.Deserialize(stream, typeof(SimpleType));

        _ = obj.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void DeserializeIncludesInheritedFields()
    {
        byte[] data = { 0xFE, 0xCA, 0x0A, 0x00, 0x00, 0x00 };
        var obj = new InheritedType { Value = 0x0A, NewValue = 0xCAFE };

        AssertDeserialization(data, obj);
    }

    [Test]
    public void DeserializeIntegerTypes()
    {
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
        var expected = new TypeWithIntegers {
            CharValue = 'Ω',
            ByteValue = 0x84,
            SByteValue = -12,
            UShortValue = 0x807F,
            ShortValue = -16,
            UIntValue = 0x12345678,
            IntegerValue = -42,
            ULongValue = 0x800000000000002A,
            LongValue = -2L,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeDecimalTypes()
    {
        byte[] data = {
            0xC3, 0xF5, 0x48, 0x40,
            0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0xC0,
        };
        var obj = new TypeWithDecimals {
            SingleValue = 3.14f,
            DoubleValue = -3.14d,
        };

        AssertDeserialization(data, obj);
    }

    [Test]
    public void DeserializeMultiPropertyStruct()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            (byte)'y', (byte)'a', (byte)'r', (byte)'h', (byte)'l', (byte)'\0',
        };
        var expected = new MultiPropertyStruct {
            IntegerValue = 1,
            LongValue = 2L,
            TextValue = "yarhl",
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeNestedObject()
    {
        byte[] data = {
            0x0A, 0x00, 0x00, 0x00,
            0x01, 0x00, 0x00, 0x00,
            0x14, 0x00, 0x00, 0x00,
        };
        var expected = new TypeWithNestedObject {
            IntegerValue = 10,
            ComplexValue = new TypeWithNestedObject.NestedType {
                NestedValue = 1,
            },
            AnotherIntegerValue = 20,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeIgnorePropertiesViaAttribute()
    {
        byte[] data = {
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        };
        var expected = new TypeWithIgnoredProperties {
            LongValue = 2L,
            IgnoredIntegerValue = 0,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeBooleanType()
    {
        byte[] data = {
            0x01, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        var expected = new TypeWithBooleanDefaultAttribute {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };

        AssertDeserialization(data, expected);

        data[4] = 0x01;
        expected.BooleanValue = true;
        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeBooleanWithDefinedValues()
    {
        var falseObj = new TypeWithBooleanDefinedValue() {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };
        byte[] serializedFalse = {
            0x01, 0x00, 0x00, 0x00,
            0xD6, 0xFF,
            0x03, 0x00, 0x00, 0x00,
        };

        var trueObj = new TypeWithBooleanDefinedValue() {
            BeforeValue = 1,
            BooleanValue = true,
            AfterValue = 3,
        };
        byte[] serializedTrue = {
            0x01, 0x00, 0x00, 0x00,
            0x2A, 0x00,
            0x03, 0x00, 0x00, 0x00,
        };

        AssertDeserialization(serializedFalse, falseObj);
        AssertDeserialization(serializedTrue, trueObj);
    }

    [Test]
    public void DeserializeBooleanWithTextValues()
    {
        var falseObj = new TypeWithBooleanTextValue() {
            BeforeValue = 1,
            BooleanValue = false,
            AfterValue = 3,
        };
        byte[] serializedFalse = {
            0x01, 0x00, 0x00, 0x00,
            (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e', (byte)'\0',
            0x03, 0x00, 0x00, 0x00,
        };

        var trueObj = new TypeWithBooleanTextValue() {
            BeforeValue = 1,
            BooleanValue = true,
            AfterValue = 3,
        };
        byte[] serializedTrue = {
            0x01, 0x00, 0x00, 0x00,
            (byte)'t', (byte)'r', (byte)'u', (byte)'e', (byte)'\0',
            0x03, 0x00, 0x00, 0x00,
        };

        AssertDeserialization(serializedFalse, falseObj);
        AssertDeserialization(serializedTrue, trueObj);
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
            () => deserializer.Deserialize<TypeWithBooleanWithoutAttribute>(),
            Throws.InstanceOf<FormatException>());
    }

    [Test]
    public void DeserializeInt24()
    {
        byte[] data = {
            0x01, 0x00, 0x00,
        };

        var expected = new TypeWithInt24 {
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

        var expected = new TypeWithStringWithoutAttribute {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 3,
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

        var expected = new TypeWithStringDefaultAttribute {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 3,
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

        var expected = new TypeWithStringVariableSize {
            BeforeValue = 1,
            StringValue = "あ",
            AfterValue = 4,
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

        var expected = new TypeWithStringFixedSize {
            BeforeValue = 1,
            StringValue = "あ",
            AfterValue = 3,
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

        var expected = new TypeWithStringDefinedEncoding {
            BeforeValue = 1,
            StringValue = "あア",
            AfterValue = 3,
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
            () => deserializer.Deserialize<TypeWithStringInvalidEncoding>(),
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

        var expected = new TypeWithEndiannessChanges {
            LittleEndianInteger = 1,
            BigEndianInteger = 2,
            DefaultEndianInteger = 3,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeEnumNoAttribute()
    {
        byte[] data = { 0x2A, 0x00, };

        var expected = new TypeWithEnumNoAttribute {
            EnumValue = SerializableEnum.Value42,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeEnumDefaultAttribute()
    {
        byte[] data = { 0x2A, 0x00, };

        var expected = new TypeWithEnumDefaultAttribute {
            EnumValue = SerializableEnum.Value42,
        };

        AssertDeserialization(data, expected);
    }

    [Test]
    public void DeserializeEnumOverwritingType()
    {
        byte[] data = { 0x01, 0x00, 0x00, 0x00 };

        var expected = new TypeWithEnumWithOverwrittenType {
            EnumValue = SerializableEnum.None,
        };

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
            () => deserializer.Deserialize<TypeWithNullable>(),
            Throws.InstanceOf<FormatException>());
    }

    private static void AssertDeserialization<T>(byte[] data, T expected)
    {
        using var stream = new DataStream();
        stream.Write(data);

        stream.Position = 0;
        var deserializer = new BinaryDeserializer(stream);
        T obj = deserializer.Deserialize<T>();

        _ = obj.Should().BeEquivalentTo(expected);
    }
}
