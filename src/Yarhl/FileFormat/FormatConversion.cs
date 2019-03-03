// FormatConversion.cs
//
// Copyright (c) 2019 SceneGate Team
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace Yarhl.FileFormat
{
    using System;
    using System.Linq;

    /// <summary>
    /// Convert formats with converters.
    /// </summary>
    public static class FormatConversion
    {
                /// <summary>
        /// Converts the format to the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to convert.</param>
        /// <typeparam name="TDst">The destination format type.</typeparam>
        public static TDst ConvertTo<TDst>(dynamic source)
        {
            return ConvertTo(typeof(TDst), source);
        }

        /// <summary>
        /// Converts the format to the specified type.
        /// </summary>
        /// <param name="source">Format to convert.</param>
        /// <typeparam name="TDst">The destination format type.</typeparam>
        /// <typeparam name="TSrc">The source format type.</typeparam>
        /// <returns>The new format.</returns>
        public static TDst ConvertTo<TDst, TSrc>(TSrc source)
        {
            return ConvertTo(typeof(TDst), source);
        }

        /// <summary>
        /// Converts the format into the specified type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="dstType">Type of the destination format.</param>
        /// <param name="src">Format to convert.</param>
        public static dynamic ConvertTo(Type dstType, dynamic src)
        {
            if (dstType == null)
                throw new ArgumentNullException(nameof(dstType));

            if (src == null)
                throw new ArgumentNullException(nameof(src));

            // Search the converter for the giving types.
            Type srcType = src.GetType();
            var extensions = PluginManager.Instance.GetConverters()
                .Where(e => e.Metadata.CanConvert(srcType, dstType));

            if (!extensions.Any()) {
                throw new InvalidOperationException(
                    $"Cannot find converter for: {srcType} -> {dstType}");
            } else if (extensions.Skip(1).Any()) {
                throw new InvalidOperationException(
                    $"Multiple converters for: {srcType} -> {dstType}");
            }

            dynamic extension = extensions.First();
            return extension.CreateExport().Value.Convert(src);
        }

        /// <summary>
        /// Converts the format using the specified converter type.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="source">Format to converter.</param>
        /// <typeparam name="TConv">Type of the converter.</typeparam>
        /// <typeparam name="TSrc">Type of the source format.</typeparam>
        /// <typeparam name="TDst">Type of the destination format.</typeparam>
        public static TDst ConvertWith<TConv, TSrc, TDst>(TSrc source)
            where TConv : IConverter<TSrc, TDst>, new()
        {
            TConv converter = new TConv();
            return converter.Convert(source);
        }

        /// <summary>
        /// Converts the format using the specified converter.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="converter">Convert to use.</param>
        /// <param name="src">Format to convert.</param>
        /// <typeparam name="TSrc">Type of the source format.</typeparam>
        /// <typeparam name="TDst">The type of the destination format type.</typeparam>
        public static TDst ConvertWith<TSrc, TDst>(
            IConverter<TSrc, TDst> converter,
            TSrc src)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            return converter.Convert(src);
        }

        /// <summary>
        /// Converts the format using the specified converter.
        /// </summary>
        /// <returns>The new format.</returns>
        /// <param name="converter">Converter to use.</param>
        /// <param name="src">Format to convert.</param>
        /// <param name="dstType">Type of the destination format.</param>
        public static object ConvertWith(dynamic converter, dynamic src, Type dstType)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (dstType == null)
                throw new ArgumentNullException(nameof(dstType));

            Type[] converterInterfaces = converter.GetType().GetInterfaces();
            bool implementConverter = converterInterfaces.Any(i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IConverter<,>));

            if (!implementConverter) {
                throw new ArgumentException(
                        "Converter doesn't implement IConverter<,>",
                        nameof(converter));
            }

            bool canConvert = converterInterfaces.Any(i =>
                i.IsGenericType &&
                i.GenericTypeArguments.Length == 2 &&
                i.GenericTypeArguments[0] == src.GetType() &&
                i.GenericTypeArguments[1] == dstType);

            if (!canConvert) {
                throw new ArgumentException(
                        "Converter cannot convert from/to the type",
                        nameof(converter));
            }

            return converter.Convert(src);
        }
    }
}