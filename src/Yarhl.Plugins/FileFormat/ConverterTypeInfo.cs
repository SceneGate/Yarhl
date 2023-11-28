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
namespace Yarhl.Plugins.FileFormat;

using System;

/// <summary>
/// Provides information from a type that implements a converter.
/// </summary>
public record ConverterTypeInfo(
    string Name,
    Type Type,
    Type InterfaceImplemented,
    IReadOnlyList<Type> GenericTypes)
    : GenericInterfaceImplementationInfo(Name, Type, InterfaceImplemented, GenericTypes)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterTypeInfo"/> class.
    /// </summary>
    /// <param name="info">The generic implementor information.</param>
    public ConverterTypeInfo(GenericInterfaceImplementationInfo info)
        : this(info.Name, info.Type, info.InterfaceImplemented, info.GenericTypes)
    {
        if (info.GenericTypes.Count != 2) {
            throw new ArgumentException("Invalid number of generics. Expected 2.");
        }
    }

    /// <summary>
    /// Gets the source type the converter can convert from.
    /// </summary>
    public Type SourceType => GenericTypes[0];

    /// <summary>
    /// Gets the destination type the converter can convert to.
    /// </summary>
    public Type DestinationType => GenericTypes[1];

    /// <summary>
    /// Check if this converter type can convert from the given source type.
    /// It checks applying covariance rules.
    /// </summary>
    /// <param name="source">Source type for conversion.</param>
    /// <returns>If this converter can realize the operation.</returns>
    public bool CanConvert(Type source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return SourceType.IsAssignableFrom(source);
    }

    /// <summary>
    /// Check if this converter type can convert from the given source type
    /// into the given desination type. It checks applying covariance and
    /// contravariance rules.
    /// </summary>
    /// <param name="source">Source type for conversion.</param>
    /// <param name="dest">Destination type for conversion.</param>
    /// <returns>If this converter can realize the operation.</returns>
    public bool CanConvert(Type source, Type dest)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(dest);

        bool matchSource = SourceType.IsAssignableFrom(source);
        bool matchDest = dest.IsAssignableFrom(DestinationType);
        return matchSource && matchDest;
    }
}
