namespace Yarhl.UnitTests.FileFormat;

using System;
using System.Composition;
using System.Globalization;
using Yarhl.FileFormat;

// Disable file may only contain a single class since we aren't going
// to create a file per test converter.
public class ConverterWithoutGenericInterface : IConverter, IInitializer<int>
{
    public void Initialize(int parameters)
    {
        // Test initialize
    }
}

public sealed class StringFormat : IFormat, IDisposable
{
    public StringFormat()
    {
    }

    public StringFormat(string str)
    {
        Value = str;
    }

    public string Value { get; set; }

    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Disposed = true;
    }
}

public sealed class IntFormat : IFormat, IDisposable
{
    public IntFormat()
    {
    }

    public IntFormat(int val)
    {
        Value = val;
    }

    public int Value { get; set; }

    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Disposed = true;
    }
}

public sealed class IntNonDisposableFormat : IFormat
{
    public IntNonDisposableFormat()
    {
    }

    public IntNonDisposableFormat(int val)
    {
        Value = val;
    }

    public int Value { get; set; }
}

public class NoFormat
{
}

public class NullSource : IFormat
{
}

public class NullDestination : IFormat
{
}

public class StringFormat2IntFormat : IConverter<StringFormat, IntFormat>
{
    public IntFormat Convert(StringFormat source)
    {
        return new IntFormat(System.Convert.ToInt32(source.Value));
    }
}

[PartNotDiscoverable]
public class StringFormatConverterWithConstructor : IConverter<StringFormat, IntFormat>
{
    private readonly NumberStyles style;
    private readonly int delta;

    public StringFormatConverterWithConstructor(NumberStyles style, int delta)
    {
        this.style = style;
        this.delta = delta;
    }

    public IntFormat Convert(StringFormat source)
    {
        return new IntFormat(int.Parse(source.Value, style) + delta);
    }
}

[PartNotDiscoverable]
public class StringFormatConverterWithSeveralConstructors :
    IConverter<StringFormat, IntFormat>
{
    private readonly NumberStyles style;
    private readonly int delta;

    public StringFormatConverterWithSeveralConstructors(NumberStyles style)
    {
        this.style = style;
        delta = 0;
    }

    public StringFormatConverterWithSeveralConstructors(int delta)
    {
        style = NumberStyles.AllowParentheses;
        this.delta = delta;
    }

    public IntFormat Convert(StringFormat source)
    {
        return new IntFormat(int.Parse(source.Value, style) + delta);
    }
}

public class StringFormatConverterWithInitializerInterface :
    IConverter<StringFormat, IntFormat>,
    IInitializer<NumberStyles>
{
    private NumberStyles style;

    public IntFormat Convert(StringFormat source)
    {
        return new IntFormat(int.Parse(source.Value, style));
    }

    public void Initialize(NumberStyles parameters)
    {
        style = parameters;
    }
}

public class IntFormat2StringFormat :
    IConverter<IntFormat, StringFormat>,
    IInitializer<int>
{
    public StringFormat Convert(IntFormat source)
    {
        return new StringFormat(source.Value.ToString());
    }

    public void Initialize(int parameters)
    {
        // Test
    }
}

public sealed class IntFormatDisposableConverter :
    IConverter<IntFormat, StringFormat>,
    IDisposable
{
    public bool Disposed { get; private set; }

    public StringFormat Convert(IntFormat source)
    {
        return new StringFormat(source.Value.ToString());
    }

    public void Dispose()
    {
        Disposed = true;
        GC.SuppressFinalize(this);
    }
}

public class IntNonDisposableFormatConverter :
    IConverter<IntNonDisposableFormat, StringFormat>
{
    public StringFormat Convert(IntNonDisposableFormat source)
    {
        return new StringFormat(source.Value.ToString());
    }
}

public class StringFormat2NoFormat :
    IConverter<StringFormat, NoFormat>,
    IInitializer<int>
{
    public void Initialize(int parameters)
    {
        // Test initialize
    }

    public NoFormat Convert(StringFormat source)
    {
        return new NoFormat();
    }
}

public class NullConverter :
    IConverter<NullSource, NullDestination>,
    IInitializer<object>
{
    public void Initialize(object parameters)
    {
        // Test initialize
    }

    public NullDestination Convert(NullSource source)
    {
        return null;
    }
}

public class ConverterWithConstructorException :
    IConverter<StringFormat, ushort>
{
    public ConverterWithConstructorException()
    {
        throw new Exception();
    }

    public ConverterWithConstructorException(string dummy)
    {
        // This one doesn't throw
        Dummy = dummy;
    }

    public string Dummy { get; }

    public ushort Convert(StringFormat source)
    {
        return ushort.Parse(source.Value);
    }
}
