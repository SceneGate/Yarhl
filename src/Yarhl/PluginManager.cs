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
namespace Yarhl
{
    using System;
    using System.Collections.Generic;
    using System.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Plugin manager.
    /// </summary>
    /// <remarks>
    /// Plugin assemblies are loaded from the directory with the Yarhl
    /// assembly and the 'Plugins' subfolder with its children.
    /// </remarks>
    public sealed class PluginManager
    {
        static readonly object LockObj = new object();
        static PluginManager singleInstance;

        ContainerConfiguration containerConfig;
        CompositionHost container;

        /// <summary>
        /// Prevents a default instance of the <see cref="PluginManager" />
        /// class from being created.
        /// </summary>
        PluginManager()
        {
            InitializeContainer();
        }

        /// <summary>
        /// Name of the plugins directory.
        /// </summary>
        /// <value>The name of the plugins directory.</value>
        public static string PluginDirectory => "Plugins";

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
        /// Finds all the extensions from the given base type.
        /// </summary>
        /// <returns>The extensions.</returns>
        /// <typeparam name="T">Type of the extension point.</typeparam>
        public IEnumerable<T> FindExtensions<T>()
        {
            return container.GetExports<T>();
        }

        /// <summary>
        /// Finds all the extensions from the given base type.
        /// </summary>
        /// <returns>The extensions.</returns>
        /// <param name="extension">Type of the extension point.</param>
        public IEnumerable<object> FindExtensions(Type extension)
        {
            if (extension == null)
                throw new ArgumentNullException(nameof(extension));

            return container.GetExports(extension);
        }

        void InitializeContainer()
        {
            // Assemblies from the program directory
            var programDir = AppDomain.CurrentDomain.BaseDirectory;
            var programAssemblies = Directory.GetFiles(programDir, "*.dll")
                .Select(Assembly.LoadFile);
            
            containerConfig = new ContainerConfiguration()
                .WithAssembly(this.GetType().Assembly)
                .WithAssemblies(programAssemblies);

            // Assemblies from the Plugin directory and subfolders
            string pluginDir = Path.Combine(programDir, PluginDirectory);
            if (Directory.Exists(pluginDir)) {
                var pluginFiles = Directory.GetFiles(
                    pluginDir,
                    "*.dll",
                    SearchOption.AllDirectories);
                var pluginAssemblies = pluginFiles.Select(Assembly.LoadFile);
                containerConfig.WithAssemblies(pluginAssemblies);
            }

            container = containerConfig.CreateContainer();
        }
    }
}
