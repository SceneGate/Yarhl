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

public class SingleOuterConverter : IConverter<MySourceFormat, MyDestFormat>
{
    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }

    public class SingleInnerConverter : IConverter<MySourceFormat, MyDestFormat>
    {
        public MyDestFormat Convert(MySourceFormat source)
        {
            return new MyDestFormat();
        }
    }
}

public sealed class ConverterAndOtherInterface :
    IConverter<MySourceFormat, MyDestFormat>,
    IDisposable
{
    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}

public class TwoConverters :
    IConverter<MySourceFormat, MyDestFormat>,
    IConverter<MyDestFormat, MySourceFormat>
{
    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }

    public MySourceFormat Convert(MyDestFormat source)
    {
        return new MySourceFormat();
    }
}

public abstract class BaseAbstractConverter : IConverter<MySourceFormat, MyDestFormat>
{
    public MyDestFormat Convert(MySourceFormat source)
    {
        return new MyDestFormat();
    }
}

public class DerivedConverter : BaseAbstractConverter
{
}
