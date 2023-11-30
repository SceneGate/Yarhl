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
public sealed class ConverterLocator
{
    private static readonly object LockObj = new();
    private static ConverterLocator? singleInstance;

    private readonly TypeLocator locator;
    private readonly List<InterfaceImplementationInfo> formatsMetadata;
    private readonly List<ConverterTypeInfo> convertersMetadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterLocator"/> class.
    /// </summary>
    /// <param name="locator">The type locator to use internally.</param>
    public ConverterLocator(TypeLocator locator)
    {
        this.locator = locator;

        formatsMetadata = new List<InterfaceImplementationInfo>();
        Formats = formatsMetadata;

        convertersMetadata = new List<ConverterTypeInfo>();
        Converters = convertersMetadata;

        ScanAssemblies();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConverterLocator"/> class.
    /// </summary>
    private ConverterLocator()
        : this(TypeLocator.Default)
    {
    }

    /// <summary>
    /// Gets the singleton instance using the default TypeLocator.
    /// </summary>
    /// <remarks><para>It initializes the manager if needed.</para></remarks>
    public static ConverterLocator Default {
        get {
            if (singleInstance == null) {
                lock (LockObj) {
                    singleInstance ??= new ConverterLocator();
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
    /// <remarks>
    /// This method is already called when the instance is created. Only needed
    /// after loading additional assemblies.
    /// </remarks>
    public void ScanAssemblies()
    {
        formatsMetadata.Clear();
        convertersMetadata.Clear();

        formatsMetadata.AddRange(
            locator.FindImplementationsOf(typeof(IFormat)));

        convertersMetadata.AddRange(
            locator.FindImplementationsOfGeneric(typeof(IConverter<,>))
                .Select(x => new ConverterTypeInfo(x)));
    }
}
