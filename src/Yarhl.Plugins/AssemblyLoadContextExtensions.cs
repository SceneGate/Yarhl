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
/// Extension methods to load assemblies from disk.
/// </summary>
public static class AssemblyLoadContextExtensions
{
    private static readonly string[] IgnoredLibraries = {
        "System.",
        "Microsoft.",
        "netstandard",
        "nuget",
        "nunit",
        "testhost",
    };

    /// <summary>
    /// Try to load the assemblies from the given file paths.
    /// </summary>
    /// <param name="loader">The load context to use to load.</param>
    /// <param name="paths">The list of assembly paths to load.</param>
    /// <returns>A collection of assemblies that could be loaded.</returns>
    /// <remarks>
    /// SECURITY NOTE: Ensure that you trust those assemblies. You may introduce
    /// a security risk by running arbitrary code.
    /// If an assembly fails to load it will be silently skipped.
    /// </remarks>
    public static IEnumerable<Assembly> TryLoadFromAssembliesPath(this AssemblyLoadContext loader, IEnumerable<string> paths)
    {
        // Skip libraries that match the ignored libraries to prevent loading dependencies.
        return paths
            .Select(p => new { Name = Path.GetFileName(p), Path = p })
            .Where(p => !Array.Exists(
                IgnoredLibraries,
                ign => p.Name.StartsWith(ign, StringComparison.OrdinalIgnoreCase)))
            .Select(p => p.Path)
            .Select(loader.TryLoadFromAssemblyPath)
            .Where(a => a is not null)
            .ToList()!; // force to run
    }

    /// <summary>
    /// Try to load every .NET assembly from the given directory.
    /// </summary>
    /// <param name="loader">The load context to use to load.</param>
    /// <param name="directory">The directory to find assemblies.</param>
    /// <param name="recursive">
    /// Value indicating whether it should search all directories or only the top directory.
    /// </param>
    /// <returns>A collection of assemblies that could be loaded.</returns>
    /// <remarks>
    /// SECURITY NOTE: Ensure that you trust those assemblies. You may introduce
    /// a security risk by running arbitrary code.
    /// If an assembly fails to load it will be silently skipped.
    /// </remarks>
    public static IEnumerable<Assembly> TryLoadFromDirectory(this AssemblyLoadContext loader, string directory, bool recursive)
    {
        var options = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] libraryAssemblies = Directory.GetFiles(directory, "*.dll", options);
        string[] programAssembly = Directory.GetFiles(directory, "*.exe");

        return TryLoadFromAssembliesPath(loader, programAssembly.Concat(libraryAssemblies));
    }

    /// <summary>
    /// Try to load every .NET assembly in the directory of the current process.
    /// </summary>
    /// <param name="loader">The load context to use to load.</param>
    /// <returns>A collection of assemblies that could be loaded.</returns>
    /// <remarks>
    /// SECURITY NOTE: Ensure that you trust those assemblies. You may introduce
    /// a security risk by running arbitrary code.
    /// If an assembly fails to load it will be silently skipped.
    /// </remarks>
    public static IEnumerable<Assembly> TryLoadFromExecutingDirectory(this AssemblyLoadContext loader)
    {
        string programDir = Path.GetDirectoryName(Environment.ProcessPath) ??
            throw new ArgumentException("Cannot determine process directory");

        string[] libraryAssemblies = Directory.GetFiles(programDir, "*.dll");
        string[] programAssembly = Directory.GetFiles(programDir, "*.exe");

        return TryLoadFromAssembliesPath(loader, programAssembly.Concat(libraryAssemblies));
    }

    /// <summary>
    /// Try to load the assembly from the given path.
    /// </summary>
    /// <param name="loader">The assembly load context.</param>
    /// <param name="path">Assembly to load.</param>
    /// <returns>The load assembly or null on error.</returns>
    public static Assembly? TryLoadFromAssemblyPath(this AssemblyLoadContext loader, string path)
    {
        try {
            return loader.LoadFromAssemblyPath(path);
        } catch (BadImageFormatException) {
            // Probably not a .NET assembly.
            return null;
        }
    }
}
