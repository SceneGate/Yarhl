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
namespace Yarhl.FileFormat;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <content>
/// Obsoleted methods in version 4.
/// To be removed before next major.
/// </content>
public static partial class ConvertFormat
{
    /// <summary>
    /// Converts the format using a converter with the specified type.
    /// </summary>
    /// <param name="src">Format to convert.</param>
    /// <typeparam name="TConv">Type of the converter.</typeparam>
    /// <returns>The new format.</returns>
    [Obsolete("Create the converter object or use the IFormat.ConvertWith() extension method.")]
    public static object With<TConv>(dynamic src)
        where TConv : IConverter, new()
    {
        var converter = new TConv();
        return With(converter, src);
    }

    /// <summary>
    /// Converts the format using a converter with the specified type
    /// and initialized with some parameters.
    /// </summary>
    /// <param name="param">Parameters to initialize the converter.</param>
    /// <param name="src">Format to convert.</param>
    /// <typeparam name="TConv">Type of the converter.</typeparam>
    /// <typeparam name="TParam">Type of the parameters.</typeparam>
    /// <returns>The new format.</returns>
    [Obsolete("Create the converter object or use the IFormat.ConvertWith() extension method.")]
    public static object With<TConv, TParam>(TParam param, dynamic src)
        where TConv : IConverter, IInitializer<TParam>, new()
    {
        var converter = new TConv();
        converter.Initialize(param);
        return With(converter, src);
    }

    /// <summary>
    /// Converts the format using the specified converter.
    /// </summary>
    /// <returns>The new format.</returns>
    /// <param name="converter">Converter to use.</param>
    /// <param name="src">Format to convert.</param>
    [Obsolete("Create the converter object or use the IFormat.ConvertWith() extension method.")]
    public static object With(IConverter converter, dynamic src)
    {
        ArgumentNullException.ThrowIfNull(converter);
        if (src is null) {
            throw new ArgumentNullException(nameof(src));
        }

        ValidateConverterType(converter.GetType(), src.GetType());

        return ((dynamic)converter).Convert(src);
    }
}
