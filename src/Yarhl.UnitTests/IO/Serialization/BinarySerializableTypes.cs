namespace Yarhl.UnitTests.IO.Serialization;

using Yarhl.IO;
using Yarhl.IO.Serialization.Attributes;

// Disable file may only contain a single class since we aren't going
// to create a file per test converter.
#pragma warning disable SA1649 // File name match type name

public class ClassTypeWithIntegerProperties
{
    public char CharValue { get; set; }

    public byte ByteValue { get; set; }

    public sbyte SByteValue { get; set; }

    public ushort UShortValue { get; set; }

    public short ShortValue { get; set; }

    public uint UIntValue { get; set; }

    public int IntegerValue { get; set; }

    public ulong ULongValue { get; set; }

    public long LongValue { get; set; }
}

public class ClassTypeWithDecimalProperties
{
    public float SingleValue { get; set; }

    public double DoubleValue { get; set; }
}

public struct MultiPropertyStruct
{
    public int IntegerValue { get; set; }

    public long LongValue { get; set; }

    public string TextValue { get; set; }
}

public class TypeWithIgnoredProperties
{
    public long LongValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }
}

public class TypeWithNestedObject
{
    public int IntegerValue { get; set; }

    public NestedType ComplexValue { get; set; }

    public int AnotherIntegerValue { get; set; }

    public sealed class NestedType
    {
        public int NestedValue { get; set; }
    }
}

public class TypeWithBooleanDefaultAttribute
{
    public int BeforeValue { get; set; }

    [BinaryBoolean]
    public bool BooleanValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithBooleanWithoutAttribute
{
    public int BeforeValue { get; set; }

    public bool BooleanValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithBooleanDefinedValue
{
    public int BeforeValue { get; set; }

    [BinaryBoolean(UnderlyingType = typeof(short), TrueValue = (short)42, FalseValue = (short)-42)]
    public bool BooleanValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithBooleanTextValue
{
    public int BeforeValue { get; set; }

    [BinaryBoolean(UnderlyingType = typeof(string), TrueValue = "true", FalseValue = "false")]
    public bool BooleanValue { get; set; }

    public int AfterValue { get; set; }
}

public class ObjectWithDefaultStringAttribute
{
    public int IntegerValue { get; set; }

    [BinaryString]
    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithoutStringAttribute
{
    public int IntegerValue { get; set; }

    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithCustomStringAttributeSizeUshort
{
    public int IntegerValue { get; set; }

    [BinaryString(SizeType = typeof(ushort), Terminator = "")]
    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithCustomStringAttributeFixedSize
{
    public int IntegerValue { get; set; }

    [BinaryString(FixedSize = 3, Terminator = "")]
    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithCustomStringAttributeCustomEncoding
{
    public int IntegerValue { get; set; }

    [BinaryString(CodePage = 932)]
    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithCustomStringAttributeUnknownEncoding
{
    public int IntegerValue { get; set; }

    [BinaryString(CodePage = 666)]
    public string StringValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithForcedEndianness
{
    [BinaryForceEndianness(EndiannessMode.LittleEndian)]
    public int LittleEndianInteger { get; set; }

    [BinaryForceEndianness(EndiannessMode.BigEndian)]
    public int BigEndianInteger { get; set; }

    public int DefaultEndianInteger { get; set; }
}

public class TypeWithEnumNoAttribute
{
    public SerializableEnum EnumValue { get; set; }
}

public class TypeWithEnumDefaultAttribute
{
    [BinaryEnum]
    public SerializableEnum EnumValue { get; set; }
}

public class TypeWithEnumWithOverwrittenType
{
    [BinaryEnum(UnderlyingType = typeof(uint))]
    public SerializableEnum EnumValue { get; set; }
}

public class ObjectWithInt24
{
    [BinaryInt24]
    public int Int24Value { get; set; }
}

public class ObjectWithNullable
{
    public int? NullValue { get; set; }
}

[System.Diagnostics.CodeAnalysis.SuppressMessage("", "S2344", Justification = "Test type")]
public enum SerializableEnum : short
{
    None = 1,
    Value42 = 42,
}
