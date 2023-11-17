// Copyright (c) 2019 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using System.Composition;
    using System.Globalization;
    using Yarhl.FileFormat;

    // Disable file may only contain a single class since we aren't going
    // to create a file per test converter.
#pragma warning disable SA1402, SA1649

    public interface IInterface
    {
        int Z { get; }
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

    public sealed class ConverterAndOtherInterface :
        IConverter<string, short>,
        IDisposable,
        IInitializer<int>
    {
        public short Convert(string source)
        {
            return System.Convert.ToInt16(source);
        }

        public void Initialize(int parameters)
        {
            // Test initialize
        }

        public void Dispose()
        {
            // Test dispose
        }
    }

    public class ConverterWithoutGenericInterface : IConverter, IInitializer<int>
    {
        public void Initialize(int parameters)
        {
            // Test initialize
        }
    }

    public abstract class BaseConverter : IConverter<string, ushort>
    {
        public ushort Convert(string source)
        {
            return System.Convert.ToUInt16(source);
        }
    }

    public class DerivedConverter : BaseConverter
    {
    }

    public sealed class StringFormatTest : IFormat, IDisposable
    {
        public StringFormatTest()
        {
        }

        public StringFormatTest(string str)
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

    public sealed class IntFormatTest : IFormat, IDisposable
    {
        public IntFormatTest()
        {
        }

        public IntFormatTest(int val)
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

    public class NoFormat
    {
    }

    public class NullSource : IFormat
    {
    }

    public class NullDestination : IFormat
    {
    }

    public class ConverterWithInitializerInterface :
        IConverter<StringFormatTest, IntFormatTest>,
        IInitializer<NumberStyles>
    {
        private NumberStyles style;

        public IntFormatTest Convert(StringFormatTest source)
        {
            return new IntFormatTest(int.Parse(source.Value, style));
        }

        public void Initialize(NumberStyles parameters)
        {
            style = parameters;
        }
    }

    public class StringFormatTest2IntFormatTestConverter :
        IConverter<StringFormatTest, IntFormatTest>,
        IConverter<IntFormatTest, StringFormatTest>
    {
        public IntFormatTest Convert(StringFormatTest source)
        {
            return new IntFormatTest(System.Convert.ToInt32(source.Value));
        }

        public StringFormatTest Convert(IntFormatTest source)
        {
            return new StringFormatTest(source.Value.ToString());
        }
    }

    public class StringFormatTest2NoFormat :
        IConverter<StringFormatTest, NoFormat>,
        IInitializer<int>
    {
        public void Initialize(int parameters)
        {
            // Test initialize
        }

        public NoFormat Convert(StringFormatTest source)
        {
            return new NoFormat();
        }
    }

    public class NullConverter :
        IConverter<NullSource, NullDestination>,
        IInitializer<int>
    {
        public void Initialize(int parameters)
        {
            // Test initialize
        }

        public NullDestination Convert(NullSource source)
        {
            return null;
        }
    }

    public class FormatTestDuplicatedConverter1 :
        IConverter<StringFormatTest, short>
    {
        public short Convert(StringFormatTest source)
        {
            return System.Convert.ToInt16(source.Value);
        }
    }

    public class FormatTestDuplicatedConverter2 :
        IConverter<StringFormatTest, short>
    {
        public short Convert(StringFormatTest source)
        {
            return System.Convert.ToInt16(source.Value);
        }
    }

    public class FormatTestBadConstructor :
        IConverter<StringFormatTest, ushort>
    {
        public FormatTestBadConstructor()
        {
            throw new Exception();
        }

        public FormatTestBadConstructor(string dummy)
        {
            // This one doesn't throw
            Dummy = dummy;
        }

        public string Dummy { get; }

        public ushort Convert(StringFormatTest source)
        {
            return ushort.Parse(source.Value);
        }
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
#pragma warning restore SA1402, SA1649

}
