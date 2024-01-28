namespace Yarhl.UnitTests.IO.Serialization;

using Yarhl.IO;
using Yarhl.IO.Serialization.Attributes;

// Disable file may only contain a single class since we aren't going
// to create a file per test converter.
#pragma warning disable SA1649 // File name match type name

public class ComplexObject
{
    public int IntegerValue { get; set; }

    public long LongValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class NestedObject
{
    public int IntegerValue { get; set; }

    public ComplexObject ComplexValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithDefaultBooleanAttribute
{
    public int IntegerValue { get; set; }

    [BinaryBoolean]
    public bool BooleanValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithoutBooleanAttribute
{
    public int IntegerValue { get; set; }

    public bool BooleanValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
}

public class ObjectWithCustomBooleanAttribute
{
    public int IntegerValue { get; set; }

    [BinaryBoolean(ReadAs = typeof(string), WriteAs = typeof(string), TrueValue = "true", FalseValue = "false")]
    public bool BooleanValue { get; set; }

    [BinaryIgnore]
    public int IgnoredIntegerValue { get; set; }

    public int AnotherIntegerValue { get; set; }
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

public class ObjectWithEnum
{
    [BinaryEnum(ReadAs = typeof(byte), WriteAs = typeof(short))]
    public Enum1 EnumValue { get; set; }
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

public enum Enum1
{
    Value1,
    Value2,
    Value3,
}
