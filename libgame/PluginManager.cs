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
    public sealed class PluginManager : IDisposable
    {
        const string AddinFolder = ".addins";

        static readonly object LockObj = new object();
        static PluginManager singleInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginManager" />
        /// class from being created.
        /// </summary>
        PluginManager()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(AddinFolder);
                AddinManager.Registry.Update();
            }

            // Make the addin folder hidden for Windows.
            var addinDir = new DirectoryInfo(AddinFolder);
            addinDir.Attributes |= FileAttributes.Hidden;
        }

        ~PluginManager()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the plugin manager instance.
        /// </summary>
        /// <remarks>It initializes the manager if needed.</remarks>
        /// <value>The plugin manager instance.</value>
        public static PluginManager Instance {
            get {
                if (singleInstance == null) {
                    lock (LockObj) {
                        if (singleInstance == null)
                            singleInstance = new PluginManager();
                    }
                }

                return singleInstance;
            }
        }

        /// <summary>
        /// Shutdown the plugin manager.
        /// </summary>
        public static void Shutdown()
        {
            Dispose(true);
        }

        /// <summary>
        /// Finds all the extensions from the given base type.
        /// </summary>
        /// <returns>The extensions.</returns>
        /// <typeparam name="T">Type of the extension point.</typeparam>
        public IEnumerable<Type> FindExtensions<T>()
        {
            return FindExtensions(typeof(T));
        }

        /// <summary>
        /// Finds all the extensions from the given base type.
        /// </summary>
        /// <returns>The extensions.</returns>
        /// <param name="extension">Type of the extension point.</param>
        public IEnumerable<Type> FindExtensions(Type extension)
        {
            return AddinManager
                .GetExtensionNodes<TypeExtensionNode>(extension)
                .Select(node => node.Type);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="PluginManager"/> object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        static void Dispose(bool freeManaged)
        {
            lock (LockObj) {
                singleInstance = null;
                if (freeManaged && AddinManager.AddinEngine.IsInitialized) {
                    // Due to a bug in Mono.Addins it may throw an exception in
                    // a multi-thread context
                    try {
                        AddinManager.Shutdown();
                    } catch (InvalidOperationException) {
                    }
                }
            }
        }
    }
}
