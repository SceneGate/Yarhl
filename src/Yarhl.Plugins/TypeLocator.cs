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
namespace Yarhl.Plugins;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

/// <summary>
/// Type locator. Find implementation of a given interface across loaded assemblies.
/// </summary>
public sealed class TypeLocator
{
    private static readonly object LockObj = new();
    private static TypeLocator? singleInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeLocator"/> class.
    /// </summary>
    private TypeLocator()
    {
        LoadContext = AssemblyLoadContext.Default;
    }

    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    /// <remarks>
    /// <para>It initializes the type if needed on the first call.</para>
    /// </remarks>
    public static TypeLocator Instance {
        get {
            if (singleInstance == null) {
                lock (LockObj) {
                    singleInstance ??= new TypeLocator();
                }
            }

            return singleInstance;
        }
    }

    /// <summary>
    /// Gets the assembly load context containing the assemblies to scan.
    /// </summary>
    /// <remarks>
    /// Use the returned instance to load new assemblies.
    /// </remarks>
    public AssemblyLoadContext LoadContext { get; }

    /// <summary>
    /// Finds and returns a collection of types that implements the given
    /// base type across all the loaded assemblies.
    /// </summary>
    /// <param name="baseType">The base type to find implementors.</param>
    /// <returns>A collection of types implementing the base type.</returns>
    public IEnumerable<InterfaceImplementationInfo> FindImplementationsOf(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType);

        return LoadContext.Assemblies
            .Where(a => !a.IsDynamic) // don't support iterating through types in .NET 6
            .SelectMany(assembly => FindImplementationsOf(baseType, assembly));
    }

    /// <summary>
    /// Finds and returns a collection of types that implements the given
    /// base type in the assembly.
    /// </summary>
    /// <param name="baseType">The base type to find implementors.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>A collection of types implementing the base type.</returns>
    public IEnumerable<InterfaceImplementationInfo> FindImplementationsOf(Type baseType, Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(baseType);
        ArgumentNullException.ThrowIfNull(assembly);

        if (baseType.IsGenericTypeDefinition) {
            throw new ArgumentException(
                "Generic type definition doesn't work on this method. " +
                $"Use {nameof(FindImplementationsOfGeneric)} instead.",
                nameof(baseType));
        }

        return assembly.ExportedTypes
            .Where(baseType.IsAssignableFrom)
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(type => new InterfaceImplementationInfo(type.FullName!, type, baseType));
    }

    /// <summary>
    /// Finds and returns a collection of types that implements the given
    /// generic interface across all the loaded assemblies.
    /// </summary>
    /// <param name="baseType">The generic interface type to find implementors.</param>
    /// <returns>A collection of types implementing the interface.</returns>
    /// <remarks>
    /// The list may contain several times the same if it implements the same interface
    /// multiple types with different generic types.
    /// </remarks>
    public IEnumerable<GenericInterfaceImplementationInfo> FindImplementationsOfGeneric(Type baseType)
    {
        ArgumentNullException.ThrowIfNull(baseType);

        return LoadContext.Assemblies
            .Where(a => !a.IsDynamic) // don't support iterating through types in .NET 6
            .SelectMany(assembly => FindImplementationsOfGeneric(baseType, assembly));
    }

    /// <summary>
    /// Finds and returns a collection of types that implements the given
    /// generic type in the assembly.
    /// </summary>
    /// <param name="baseType">The generic type to find implementors.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>A collection of types implementing the base type.</returns>
    /// <remarks>
    /// The list may contain several entries for the same implementation type
    /// if it implements several type the generic with different parameters.
    /// </remarks>
    public IEnumerable<GenericInterfaceImplementationInfo> FindImplementationsOfGeneric(
        Type baseType,
        Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(baseType);
        ArgumentNullException.ThrowIfNull(assembly);

        bool ValidImplementationInterface(Type type) =>
            type.IsGenericType
            && type.GetGenericTypeDefinition().IsEquivalentTo(baseType);

        return assembly.ExportedTypes
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => Array.Exists(t.GetInterfaces(), ValidImplementationInterface))
            .SelectMany(type => type.GetInterfaces() // A class may implement a generic interface multiple times
                .Where(ValidImplementationInterface)
                .Select(implementedInterface =>
                    new GenericInterfaceImplementationInfo(
                        type.FullName!,
                        type,
                        implementedInterface,
                        implementedInterface.GenericTypeArguments)));
    }
}
