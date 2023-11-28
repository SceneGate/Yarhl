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
namespace Yarhl.Plugins.FileFormat;

using System.Collections.Generic;
using Yarhl.FileFormat;

/// <summary>
/// Locates converter types across assemblies and provide their information.
/// </summary>
public sealed class ConvertersLocator
{
    private static readonly object LockObj = new();
    private static ConvertersLocator? singleInstance;

    private readonly List<InterfaceImplementationInfo> formatsMetadata;
    private readonly List<ConverterTypeInfo> convertersMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertersLocator"/> class.
    /// </summary>
    private ConvertersLocator()
    {
        formatsMetadata = new List<InterfaceImplementationInfo>();
        Formats = formatsMetadata;

        convertersMetadata = new List<ConverterTypeInfo>();
        Converters = convertersMetadata;

        ScanAssemblies();
    }

    /// <summary>
    /// Gets the plugin manager instance.
    /// </summary>
    /// <remarks><para>It initializes the manager if needed.</para></remarks>
    public static ConvertersLocator Instance {
        get {
            if (singleInstance == null) {
                lock (LockObj) {
                    singleInstance ??= new ConvertersLocator();
                }
            }

            return singleInstance;
        }
    }

    /// <summary>
    /// Gets the list of Yarhl formats information from loaded assemblies.
    /// </summary>
    public IReadOnlyList<InterfaceImplementationInfo> Formats { get; }

    /// <summary>
    /// Gets the list of Yarhl converters information from loaded assemblies.
    /// </summary>
    public IReadOnlyList<ConverterTypeInfo> Converters { get; }

    /// <summary>
    /// Scan the assemblies from the load context to look for formats and converters.
    /// </summary>
    public void ScanAssemblies()
    {
        formatsMetadata.Clear();
        formatsMetadata.AddRange(
            TypeLocator.Instance.FindImplementationsOf(typeof(IFormat)));

        convertersMetadata.Clear();
        convertersMetadata.AddRange(
            TypeLocator.Instance
                .FindImplementationsOfGeneric(typeof(IConverter<,>))
                .Select(x => new ConverterTypeInfo(x)));
    }
}
