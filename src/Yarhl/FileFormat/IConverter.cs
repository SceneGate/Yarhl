﻿// Copyright (c) 2019 SceneGate

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
namespace Yarhl.FileFormat
{
    /// <summary>
    /// Non-generic converter interface.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1040:AvoidEmptyInterfaces",
        Justification = "Provides a minimal of type checking when we don't know the generics")]
    public interface IConverter
    {
    }

    /// <summary>
    /// Format converter interface.
    /// </summary>
    /// <typeparam name="TSrc">Source format.</typeparam>
    /// <typeparam name="TDst">Destination format.</typeparam>
    public interface IConverter<in TSrc, out TDst> : IConverter
    {
        /// <summary>
        /// Converts the specified source into the given type.
        /// </summary>
        /// <returns>The converted source.</returns>
        /// <param name="source">Source format to convert.</param>
        TDst Convert(TSrc source);
    }
}
