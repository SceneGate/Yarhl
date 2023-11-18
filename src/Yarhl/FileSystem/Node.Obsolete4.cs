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
namespace Yarhl.FileSystem;

using System;
using Yarhl.FileFormat;

/// <content>
/// Obsoleted methods in version 4.
/// To be removed before next major.
/// </content>
public partial class Node
{
    /// <summary>
    /// Transform the node format to another format with a given converter
    /// initialized with parameters.
    /// </summary>
    /// <returns>This node.</returns>
    /// <typeparam name="TConv">The type of the converter to use.</typeparam>
    /// <typeparam name="TParam">The type for initializing the converter.</typeparam>
    /// <param name="param">Parameters to initialize the converter.</param>
    [Obsolete("IInitialize is obsolete. Use the converter constructor and TransformWith(converter)")]
    public Node TransformWith<TConv, TParam>(TParam param)
        where TConv : IConverter, IInitializer<TParam>, new()
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(Node));

        if (Format == null) {
            throw new InvalidOperationException(
                "Cannot transform a node without format");
        }

        object result = ConvertFormat.With<TConv, TParam>(param, Format);
        CastAndChangeFormat(result);

        return this;
    }
}
