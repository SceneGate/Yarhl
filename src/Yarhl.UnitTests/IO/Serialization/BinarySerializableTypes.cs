namespace Yarhl.UnitTests.IO.Serialization;

using Yarhl.IO;
using Yarhl.IO.Serialization.Attributes;

// Disable file may only contain a single class since we aren't going
// to create a file per test converter.
#pragma warning disable SA1649 // File name match type name
#pragma warning disable SA1124 // do not use regions - I would agree but too many types

public class SimpleType
{
    public int Value { get; set; }
}

public class InheritedType : SimpleType
{
    public ushort NewValue { get; set; }
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

public class TypeWithEndiannessChanges
{
    [BinaryEndianness(EndiannessMode.LittleEndian)]
    public int LittleEndianInteger { get; set; }

    [BinaryEndianness(EndiannessMode.BigEndian)]
    public int BigEndianInteger { get; set; }

    public int DefaultEndianInteger { get; set; }
}

public class TypeWithNullable
{
    public int? NullValue { get; set; }
}

#region Integer types
public class TypeWithIntegers
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

public class TypeWithDecimals
{
    public float SingleValue { get; set; }

    public double DoubleValue { get; set; }
}

public class TypeWithInt24
{
    [BinaryInt24]
    public int Int24Value { get; set; }
}
#endregion

#region Boolean types
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
#endregion

#region String types
public class TypeWithStringDefaultAttribute
{
    public int BeforeValue { get; set; }

    [BinaryString]
    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithStringWithoutAttribute
{
    public int BeforeValue { get; set; }

    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithStringVariableSize
{
    public int BeforeValue { get; set; }

    [BinaryString(SizeType = typeof(ushort), Terminator = "")]
    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithStringFixedSize
{
    public int BeforeValue { get; set; }

    [BinaryString(FixedSize = 3, Terminator = "")]
    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithStringDefinedEncoding
{
    public int BeforeValue { get; set; }

    [BinaryString(CodePage = 932)]
    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}

public class TypeWithStringInvalidEncoding
{
    public int BeforeValue { get; set; }

    [BinaryString(CodePage = 666)]
    public string StringValue { get; set; }

    public int AfterValue { get; set; }
}
#endregion

#region Enum types
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

[System.Diagnostics.CodeAnalysis.SuppressMessage("", "S2344", Justification = "Test type")]
public enum SerializableEnum : short
{
    None = 1,
    Value42 = 42,
}
#endregion
