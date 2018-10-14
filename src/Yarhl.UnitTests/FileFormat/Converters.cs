//
//  Converters.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using System.Composition;
    using Yarhl.FileFormat;

    [Export(typeof(IConverter<string, uint>))]
    public class SingleOuterConverterExample : IConverter<string, uint>
    {
        public uint Convert(string source)
        {
            return System.Convert.ToUInt32(source);
        }

        [Export(typeof(IConverter<string, ulong>))]
        public class SingleInnerConverterExample : IConverter<string, ulong>
        {
            public ulong Convert(string source)
            {
                return System.Convert.ToUInt64(source);
            }
        }
    }

    [Export(typeof(IConverter<string, int>))]
    [Export(typeof(IConverter<int, string>))]
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

    [Export(typeof(IConverter<string, short>))]
    public class ConverterAndOtherInterface :
        IConverter<string, short>, IDisposable
    {
        public short Convert(string source)
        {
            return System.Convert.ToInt16(source);
        }

        public void Dispose()
        {
        }
    }

    public abstract class BaseConverter : IConverter<string, ushort>
    {
        public ushort Convert(string source)
        {
            return System.Convert.ToUInt16(source);
        }
    }

    [Export(typeof(IConverter<string, ushort>))]
    public class DerivedConverter : BaseConverter
    {
    }

    [Format("Yarhl.UnitTests.StringFormat")]
    public class StringFormatTest : Format
    {
        public StringFormatTest(string str)
        {
            Value = str;
        }

        public string Value { get; private set; }
    }

    [Format]
    public class IntFormatTest : Format
    {
        public IntFormatTest(int val)
        {
            Value = val;
        }

        public int Value { get; private set; }
    }

    [Export(typeof(IConverter<StringFormatTest, int>))]
    public class StringFormatTest2IntConverter : IConverter<StringFormatTest, int>
    {
        public int Convert(StringFormatTest test)
        {
            return System.Convert.ToInt32(test.Value);
        }
    }

    [Export(typeof(IConverter<StringFormatTest, IntFormatTest>))]
    [Export(typeof(IConverter<IntFormatTest, StringFormatTest>))]
    public class StringFormatTest2IntFormatTestConverter :
        IConverter<StringFormatTest, IntFormatTest>,
        IConverter<IntFormatTest, StringFormatTest>
    {
        public IntFormatTest Convert(StringFormatTest test)
        {
            return new IntFormatTest(System.Convert.ToInt32(test.Value));
        }

        public StringFormatTest Convert(IntFormatTest test)
        {
            return new StringFormatTest(test.Value.ToString());
        }
    }
}
