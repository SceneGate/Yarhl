namespace Yarhl.UnitTests.FileFormat;
using Yarhl.FileFormat;

public interface IInterface
{
    int Z { get; }
}

public class InterfaceImpl : IInterface
{
    public int Z { get; set; }
}

public class ConverterInterface :
    IConverter<IInterface, int>
{
    public int Convert(IInterface source)
    {
        return source.Z;
    }
}

public class BaseFormat
{
    public ushort X { get; set; }
}

public class DerivedFormat : BaseFormat
{
    public ushort Y { get; set; }
}

public class ConvertDerivedFormat :
    IConverter<ushort, DerivedFormat>, IConverter<DerivedFormat, ushort>
{
    public DerivedFormat Convert(ushort source)
    {
        return new DerivedFormat {
            X = source,
            Y = (ushort)(source + 1),
        };
    }

    public ushort Convert(DerivedFormat source)
    {
        return source.Y;
    }
}

public class ConvertBaseFormat :
    IConverter<int, BaseFormat>, IConverter<BaseFormat, int>
{
    public BaseFormat Convert(int source)
    {
        return new BaseFormat {
            X = (ushort)(source + 2),
        };
    }

    public int Convert(BaseFormat source)
    {
        return source.X + 5;
    }
}
