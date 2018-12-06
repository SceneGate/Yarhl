//
//  AssemblyUtils.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;

    /// <summary>
    /// Utilities to work with Assemblies in different frameworks.
    /// </summary>
    static class AssemblyUtils
    {
        /// <summary>
        /// Load assemblies in different .NET implementations.
        /// </summary>
        /// <param name="paths">List of assemblies to load.</param>
        /// <returns>The assemblies.</returns>
        public static IEnumerable<Assembly> LoadAssemblies(this IEnumerable<string> paths)
        {
            string framework = RuntimeInformation.FrameworkDescription;
            if (framework.StartsWith(".NET Core")) {
                return LoadAssembliesNetCore(paths);
            } else {
                return paths.Select(Assembly.LoadFile);
            }
        }

        /// <summary>
        /// Load assemblies from .NET Core.
        /// </summary>
        /// <remarks>
        /// In .NET Core for some bugs / features we can't use the method
        /// Assembly.LoadFile because two identical types can return false in
        /// an equality. For that reason we need to load the assemblies with
        /// the AssemblyLoadContext which is only available in .NET Core.
        /// </remarks>
        /// <param name="paths">List of assemblies paths.</param>
        /// <returns>The load assemblies.</returns>
        static IEnumerable<Assembly> LoadAssembliesNetCore(
            IEnumerable<string> paths)
        {
            return paths.Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
        }
    }
}
