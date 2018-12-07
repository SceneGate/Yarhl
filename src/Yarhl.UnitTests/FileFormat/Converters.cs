// Converters.cs
//
// Author:
//      Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.UnitTests.FileFormat
{
    using System;
    using Yarhl.FileFormat;

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

    public class DerivedConverter : BaseConverter
    {
    }

    public class StringFormatTest : Format
    {
        public StringFormatTest()
        {
        }

        public StringFormatTest(string str)
        {
            Value = str;
        }

        public string Value { get; set; }
    }

    public class IntFormatTest : Format
    {
        public IntFormatTest()
        {
        }

        public IntFormatTest(int val)
        {
            Value = val;
        }

        public int Value { get; set; }
    }

    public class StringFormatTest2IntConverter : IConverter<StringFormatTest, int>
    {
        public int Convert(StringFormatTest test)
        {
            return System.Convert.ToInt32(test.Value);
        }
    }

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
