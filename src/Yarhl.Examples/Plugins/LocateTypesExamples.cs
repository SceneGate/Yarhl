// Copyright (c) 2023 SceneGate

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
namespace Yarhl.Examples.Plugins;

using Yarhl.FileFormat;
using Yarhl.Plugins;

public static class LocateTypesExamples
{
    public static void FindFormats()
    {
        #region FindFormats
        TypeImplementationInfo[] formatsInfo = TypeLocator.Default
            .FindImplementationsOf(typeof(IFormat))
            .ToArray();

        Console.WriteLine(formatsInfo[0].Name); // e.g. Yarhl.IO.BinaryFormat
        Console.WriteLine(formatsInfo[0].Type); // e.g. Type object for BinaryFormat
        #endregion
    }

    public static void FindConverters()
    {
        #region FindConverters
        GenericTypeImplementationInfo[] convertersInfo = TypeLocator.Default
            .FindImplementationsOfGeneric(typeof(IConverter<,>))
            .ToArray();

        Console.WriteLine(convertersInfo[0].Name); // e.g. Yarhl.Media.Text.Binary2Po
        Console.WriteLine(convertersInfo[0].Type); // e.g. Type object for Yarhl.Media.Text.Binary2Po
        Console.WriteLine(convertersInfo[0].GenericBaseType); // e.g. Type IConverter<BinaryFormat, Po>
        Console.WriteLine(convertersInfo[0].GenericTypeParameters); // e.g. [BinaryFormat, Po]
        #endregion
    }
}
