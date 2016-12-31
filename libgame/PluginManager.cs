//
//  PluginManager.cs
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
namespace Libgame
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Mono.Addins;

    /// <summary>
    /// Manager for LibGame plugins.
    /// </summary>
    class PluginManager
    {
        const string AddinFolder = ".addins";

        static readonly object lockObj = new object();
        static PluginManager singleInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Libgame.PluginManager"/> class.
        /// </summary>
        PluginManager()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(AddinFolder);
                AddinManager.Registry.Update();
            }

            // Make the addin folder hidden for Windows.
            var addinDir = new DirectoryInfo(AddinFolder);
            if (!addinDir.Attributes.HasFlag(FileAttributes.Hidden))
                addinDir.Attributes |= FileAttributes.Hidden;
        }

        ~PluginManager()
        {
            Shutdown();
        }

        /// <summary>
        /// Gets the plugin manager instance.
        /// </summary>
        /// <remarks>It initializes the manager if needed.</remarks>
        /// <value>The plugin manager instance.</value>
        public static PluginManager Instance {
            get {
                if (singleInstance == null)
                    Initialize();
                return singleInstance;
            }
        }

        /// <summary>
        /// Initialize the plugin manager.
        /// </summary>
        public static void Initialize()
        {
            if (singleInstance == null) {
                lock (lockObj) {
                    if (singleInstance == null)
                        singleInstance = new PluginManager();
                }
            }
        }

        /// <summary>
        /// Shutdown the plugin manager.
        /// </summary>
        /// <remarks>
        /// This is usually unnecessary since the destructor will do it too.
        /// </remarks>
        public static void Shutdown()
        {
            if (AddinManager.IsInitialized) {
                AddinManager.Shutdown();
            }

            singleInstance = null;
        }

        public IEnumerable<Type> FindExtensions<T>()
        {
            return FindExtensions(typeof(T));
        }

        public IEnumerable<Type> FindExtensions(Type extension)
        {
            return AddinManager
                .GetExtensionNodes<TypeExtensionNode>(extension)
                .Select(node => node.Type);
        }

        public IEnumerable<Type> FindGenericExtensions(
                Type extension,
                params Type[] genericTypeArguments)
        {
            return FindExtensions(extension)
                   .Where(type =>
                       type.GetInterfaces().Any(inter =>
                            genericTypeArguments.SequenceEqual(
                                inter.GenericTypeArguments,
                                new TypeParamComparer())));
        }

        class TypeParamComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.IsAssignableFrom(y);
            }

            public int GetHashCode(Type obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
