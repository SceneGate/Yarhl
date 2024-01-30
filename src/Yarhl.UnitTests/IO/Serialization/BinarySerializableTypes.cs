namespace Yarhl.UnitTests.IO.Serialization;

using Yarhl.IO;
using Yarhl.IO.Serialization.Attributes;

// Disable file may only contain a single class since we aren't going
// to create a file per test converter.
#pragma warning disable SA1649 // File name match type name
#pragma warning disable SA1124 // do not use regions - I would agree but too many types

public class SimpleType
{
    [BinaryOrder(0)]
    public int Value { get; set; }
}

public class InheritedType : SimpleType
{
    [BinaryOrder(1)]
    public ushort NewValue { get; set; }
}

public struct MultiPropertyStruct
{
    [BinaryOrder(0)]
    public int IntegerValue { get; set; }

    [BinaryOrder(1)]
    public long LongValue { get; set; }

    [BinaryOrder(2)]
    public string TextValue { get; set; }
}

public class TypeWithIgnoredProperties
{
    [BinaryOrder(0)]
    public long LongValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }
}

public class TypeWithNestedObject
{
    [BinaryOrder(0)]
    public int IntegerValue { get; set; }

    [BinaryOrder(1)]
    public NestedType ComplexValue { get; set; }

    [BinaryOrder(2)]
    public int AnotherIntegerValue { get; set; }

    public sealed class NestedType
    {
        [BinaryOrder(0)]
        public int NestedValue { get; set; }
    }
}

public class TypeWithEndiannessChanges
{
    [BinaryOrder(0)]
    [BinaryEndianness(EndiannessMode.LittleEndian)]
    public int LittleEndianInteger { get; set; }

    [BinaryOrder(1)]
    [BinaryEndianness(EndiannessMode.BigEndian)]
    public int BigEndianInteger { get; set; }

    [BinaryOrder(2)]
    public int DefaultEndianInteger { get; set; }
}

public class TypeWithNullable
{
    [BinaryOrder(0)]
    public int? NullValue { get; set; }
}

#region Integer types
public class TypeWithIntegers
{
    [BinaryOrder(0)]
    public char CharValue { get; set; }

    [BinaryOrder(1)]
    public byte ByteValue { get; set; }

    [BinaryOrder(2)]
    public sbyte SByteValue { get; set; }

    [BinaryOrder(3)]
    public ushort UShortValue { get; set; }

    [BinaryOrder(4)]
    public short ShortValue { get; set; }

    [BinaryOrder(5)]
    public uint UIntValue { get; set; }

    [BinaryOrder(6)]
    public int IntegerValue { get; set; }

    [BinaryOrder(7)]
    public ulong ULongValue { get; set; }

    [BinaryOrder(8)]
    public long LongValue { get; set; }
}

public class TypeWithDecimals
{
    [BinaryOrder(0)]
    public float SingleValue { get; set; }

    [BinaryOrder(1)]
    public double DoubleValue { get; set; }
}

public class TypeWithInt24
{
    [BinaryOrder(0)]
    [BinaryInt24]
    public int Int24Value { get; set; }
}
#endregion

#region Boolean types
public class TypeWithBooleanDefaultAttribute
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryBoolean]
    public bool BooleanValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithBooleanWithoutAttribute
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    public bool BooleanValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithBooleanDefinedValue
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryBoolean(UnderlyingType = typeof(short), TrueValue = (short)42, FalseValue = (short)-42)]
    public bool BooleanValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithBooleanTextValue
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryBoolean(UnderlyingType = typeof(string), TrueValue = "true", FalseValue = "false")]
    public bool BooleanValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}
#endregion

#region String types
public class TypeWithStringDefaultAttribute
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryString]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithStringWithoutAttribute
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithStringVariableSize
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryString(SizeType = typeof(ushort), Terminator = "")]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithStringFixedSize
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryString(FixedSize = 3, Terminator = "")]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithStringDefinedEncoding
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryString(CodePage = 932)]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}

public class TypeWithStringInvalidEncoding
{
    [BinaryOrder(0)]
    public int BeforeValue { get; set; }

    [BinaryOrder(1)]
    [BinaryString(CodePage = 666)]
    public string StringValue { get; set; }

    [BinaryOrder(2)]
    public int AfterValue { get; set; }
}
#endregion

#region Enum types
public class TypeWithEnumNoAttribute
{
    [BinaryOrder(0)]
    public SerializableEnum EnumValue { get; set; }
}

public class TypeWithEnumDefaultAttribute
{
    [BinaryOrder(0)]
    [BinaryEnum]
    public SerializableEnum EnumValue { get; set; }
}

public class TypeWithEnumWithOverwrittenType
{
    [BinaryOrder(0)]
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
