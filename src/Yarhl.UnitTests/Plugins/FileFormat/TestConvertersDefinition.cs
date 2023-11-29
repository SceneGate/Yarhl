namespace Yarhl.UnitTests.Plugins.FileFormat;

using System;
using Yarhl.FileFormat;

#pragma warning disable SA1649 // File name match type name

public class MySourceFormat : IFormat
{
}

public class MyDestFormat : IFormat
{
}

public class DerivedSourceFormat : MySourceFormat
{
}

public class DerivedDestFormat : IFormat
{
}

public class MyConverter : IConverter<MySourceFormat, MyDestFormat>
{
    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }
}

public class MyConverterParametrized : IConverter<MySourceFormat, MyDestFormat>
{
    public MyConverterParametrized(bool ignoreMe)
    {
    }

    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }
}

public class SingleOuterConverterExample : IConverter<string, uint>
{
    public uint Convert(string source)
    {
        return System.Convert.ToUInt32(source);
    }

    public class SingleInnerConverterExample : IConverter<string, ulong>
    {
        public ulong Convert(string source)
        {
            return System.Convert.ToUInt64(source);
        }
    }
}

public sealed class ConverterAndOtherInterface :
    IConverter<string, short>,
    IDisposable
{
    public short Convert(string source)
    {
        return System.Convert.ToInt16(source);
    }

    public void Dispose()
    {
        // Test dispose
    }
}

public class TwoConvertersExample :
    IConverter<string, int>, IConverter<int, string>
{
    public int Convert(string source)
    {
        return System.Convert.ToInt32(source);
    }

    public string Convert(int source)
    {
        return source.ToString();
    }
}

public class DuplicatedConverter1 :
    IConverter<string, sbyte>
{
    public sbyte Convert(string source)
    {
        return System.Convert.ToSByte(source);
    }
}

public class DuplicatedConverter2 :
    IConverter<string, sbyte>
{
    public sbyte Convert(string source)
    {
        return System.Convert.ToSByte(source);
    }
}

public abstract class BaseAbstractConverter : IConverter<string, long>
{
    public long Convert(string source)
    {
        return System.Convert.ToInt64(source);
    }
}

public class DerivedConverter : BaseAbstractConverter
{
}
