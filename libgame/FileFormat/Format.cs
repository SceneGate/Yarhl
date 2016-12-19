//
//  Format.cs
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
namespace Libgame.FileFormat
{
    using System;
    using System.Linq;
    using Mono.Addins;

    /// <summary>
    /// Abstract file format.
    /// </summary>
    [TypeExtensionPoint]
    public abstract class Format : IDisposable
    {
        /// <summary>
        /// Finalizes an instance of the <see cref="Format"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Format"/> is reclaimed by garbage collection.
        /// </summary>
        ~Format()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the name of the format.
        /// </summary>
        /// <value>The format name.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Format"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed { get; private set; }

        /// <summary>
        /// Converts the format to the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to convert.</param>
        /// <param name="dstType">Destination format type.</param>
        /// <typeparam name="TSrc">The type of the source format.</typeparam>
        public static dynamic ConvertFrom<TSrc>(TSrc source, Type dstType)
        {
            return Convert(typeof(TSrc), source, dstType);
        }

        /// <summary>
        /// Converts the format to the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to convert.</param>
        /// <typeparam name="TDst">The destination format type.</typeparam>
        public static TDst ConvertTo<TDst>(dynamic source)
        {
            return Convert(source.GetType(), source, typeof(TDst));
        }

        /// <summary>
        /// Converts the format to the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to convert.</param>
        /// <param name="dstType">The destination format type.</param>
        public static dynamic ConvertTo(dynamic source, Type dstType)
        {
            return Convert(source.GetType(), source, dstType);
        }

        /// <summary>
        /// Converts the format into the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to convert.</param>
        /// <typeparam name="TSrc">The source format type.</typeparam>
        /// <typeparam name="TDst">The destination format type.</typeparam>
        public static TDst Convert<TSrc, TDst>(TSrc source)
        {
            return Convert(typeof(TSrc), source, typeof(TDst));
        }

        /// <summary>
        /// Converts the format into the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="srcType">Type of the format to convert.</param>
        /// <param name="src">Format to convert.</param>
        /// <param name="dstType">Type of the destination format.</param>
        public static dynamic Convert(Type srcType, dynamic src, Type dstType)
        {
            // Search the converter for the giving types and create an instance
            dynamic converter;
            try {
                Type converterType = PluginManager.Instance
                    .FindGenericExtensions(typeof(IConverter<,>), srcType, dstType)
                    .Single();
                converter = Activator.CreateInstance(converterType);
            } catch (InvalidOperationException ex) {
                throw new InvalidOperationException(
                    "No single converter for " + srcType + " -> " + dstType,
                    ex);
            } catch (System.Reflection.TargetInvocationException ex) {
                throw new InvalidOperationException(
                    "Exception in converter constructor",
                    ex);
            } catch (MissingMemberException ex) {
                throw new InvalidOperationException(
                    "The converter has no constructor without arguments.\n" +
                    "Create the converter object and use ConvertWith<T>.",
                    ex);
            }

            return converter.Convert(src);
        }

        /// <summary>
        /// Converts the format using the specified converter.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="src">Format to convert.</param>
        /// <param name="converter">Convert to use.</param>
        /// <typeparam name="T">The type of the destination format type.</typeparam>
        public static T ConvertWith<T>(dynamic src, dynamic converter)
        {
            return Format.ConvertWith(src, typeof(T), converter);
        }

        /// <summary>
        /// Converts the format using the specified converter.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="src">Format to convert.</param>
        /// <param name="dstType">Type of the destination format.</param>
        /// <param name="converter">Converter to use.</param>
        public static dynamic ConvertWith(dynamic src, Type dstType, dynamic converter)
        {
            Type[] converterInterfaces = converter.GetType().GetInterfaces();
            bool implementConverter = converterInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IConverter<,>));
            if (!implementConverter)
                throw new ArgumentException(
                    "Converter doesn't implement IConverter<,>",
                    nameof(converter));

            bool canConvert = converterInterfaces.Any(i =>
                i.IsGenericType &&
                i.GenericTypeArguments.Length == 2 &&
                i.GenericTypeArguments[0] == src.GetType() &&
                i.GenericTypeArguments[1] == dstType);
            if (!canConvert)
                throw new ArgumentException(
                    "Converter cannot convert from/to the type",
                    nameof(converter));

            return converter.Convert(src); 
        }

        /// <summary>
        /// Converts into the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <typeparam name="T">The type of the destination format.</typeparam>
        public T ConvertTo<T>()
        {
            return ConvertTo<T>(this);
        }

        /// <summary>
        /// Converts into the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="dstType">The type of the destination format.</param>
        public dynamic ConvertTo(Type dstType)
        {
            return ConvertTo(this, dstType);
        }

        /// <summary>
        /// Converts using the specified converter.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="converter">Converter to use.</param>
        /// <typeparam name="T">The type of the destination format.</typeparam>
        public T ConvertWith<T>(dynamic converter)
        {
            return ConvertWith<T>(this, converter);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Format"/>
        /// object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);              // Dispose me everything (L)
            GC.SuppressFinalize(this);  // Don't dispose again!
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Format"/>
        /// object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">If set to <c>true</c> free
        /// managed resources also.</param>
        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            if (Disposed)
                return;

            Disposed = true;
        }
    }
}
