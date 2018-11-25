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
    using System.Composition;
    using System.Composition.Convention;
    using System.Composition.Hosting;
    using System.IO;
    using System.Linq;
    using System.Reflection;
#if NETCOREAPP2_1
    using System.Runtime.Loader;
#endif
    using FileFormat;

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

        /// <summary>
        /// Finds all the extensions from the given base type and return their
        /// lazy type for initialization.
        /// </summary>
        /// <typeparam name="T">Type of the extension point.</typeparam>
        /// <returns>The lazy extensions.</returns>
        public IEnumerable<ExportFactory<T>> FindLazyExtensions<T>()
        {
            return container.GetExports<ExportFactory<T>>();
        }

        /// <summary>
        /// Finds all the extensions from the given base type and returns
        /// a factory to initialize the type.
        /// </summary>
        /// <param name="extension">Type of the extension point.</param>
        /// <returns>The extension factory.</returns>
        public IEnumerable<object> FindLazyExtensions(Type extension)
        {
            if (extension == null) {
                throw new ArgumentNullException(nameof(extension));
            }

            Type lazyType = typeof(ExportFactory<>).MakeGenericType(extension);
            return container.GetExports(lazyType);
        }

        /// <summary>
        /// Finds all the extensions from the given base type and returns
        /// a factory to initialize the type and its associated metadata.
        /// </summary>
        /// <typeparam name="T">Type of the extension point.</typeparam>
        /// <typeparam name="TMetadata">Type of the metadata.</typeparam>
        /// <returns>The extension factory.</returns>
        public IEnumerable<ExportFactory<T, TMetadata>> FindLazyExtensions<T, TMetadata>()
            where TMetadata : IExportMetadata
        {
            // Because of technical limitations / bugs there can be upto
            // 3 copies of the same extension. We filter by type.
            return container.GetExports<ExportFactory<T, TMetadata>>()
                .GroupBy(f => f.Metadata.Type)
                .Select(f => f.First());
        }

        /// <summary>
        /// Get a list of format extensions.
        /// </summary>
        /// <returns>Enumerable of lazy formats with metadata.</returns>
        public IEnumerable<ExportFactory<Format, FormatMetadata>> GetFormats()
        {
            return FindLazyExtensions<Format, FormatMetadata>();
        }

        /// <summary>
        /// Get a list of converter extensions.
        /// </summary>
        /// <returns>Enumerable of lazy converters with metadata.</returns>
        public IEnumerable<ExportFactory<IConverter, ConverterMetadata>> GetConverters()
        {
            return FindLazyExtensions<IConverter, ConverterMetadata>();
        }

        static void DefineFormatConventions(ConventionBuilder conventions)
        {
            conventions
                .ForTypesDerivedFrom<Format>()
                .Export<Format>(
                    export => export
                        .AddMetadata("Name", t => t.FullName)
                        .AddMetadata("Type", t => t))
                .SelectConstructor(ctors =>
                    ctors.OrderBy(ctor => ctor.GetParameters().Length)
                    .First());
        }

        static void DefineConverterConventions(ConventionBuilder conventions)
        {
            bool converterInterfaceFilter(Type i) =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition().IsEquivalentTo(typeof(IConverter<,>));

            // We export three types each converter:
            // 1.- Export the specific generic converter types
            // 2.- Export the IConverter interfaces with the interfaces metadata
            // 3.- Export again the IConverter interface to fill common metadata
            conventions
                .ForTypesDerivedFrom(typeof(IConverter<,>))
                .ExportInterfaces(converterInterfaceFilter)
                .ExportInterfaces(
                    converterInterfaceFilter,
                    (inter, export) => export
                        .AddMetadata("Sources", inter.GenericTypeArguments[0])
                        .AddMetadata("Destinations", inter.GenericTypeArguments[1])
                        .AsContractType<IConverter>())
                .Export<IConverter>(
                    export => export
                    .AddMetadata("Name", t => t.FullName)
                    .AddMetadata("Type", t => t))
                .SelectConstructor(ctors =>
                    ctors.OrderBy(ctor => ctor.GetParameters().Length)
                    .First());
        }

        void InitializeContainer()
        {
            var conventions = new ConventionBuilder();
            DefineFormatConventions(conventions);
            DefineConverterConventions(conventions);

            var containerConfig = new ContainerConfiguration()
                .WithDefaultConventions(conventions);

            // Assemblies from the program directory (including this one).
            var programDir = AppDomain.CurrentDomain.BaseDirectory;
            var programAssemblies = Directory.GetFiles(programDir, "*.dll")
#if NETCOREAPP2_1
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);
#else
                .Select(Assembly.LoadFile);
#endif
            containerConfig.WithAssemblies(programAssemblies);

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
