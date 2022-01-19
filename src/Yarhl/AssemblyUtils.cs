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
namespace Yarhl
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;

    /// <summary>
    /// Utilities to work with Assemblies.
    /// </summary>
    static class AssemblyUtils
    {
        /// <summary>
        /// Load assemblies.
        /// </summary>
        /// <param name="paths">List of assemblies to load.</param>
        /// <returns>The assemblies.</returns>
        public static IEnumerable<Assembly> LoadAssemblies(this IEnumerable<string> paths)
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (string path in paths) {
                try {
                    Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                    assemblies.Add(assembly);
                } catch (BadImageFormatException) {
                    // Bad IL. Skip.
                }
            }

            return assemblies;
        }
    }
}
