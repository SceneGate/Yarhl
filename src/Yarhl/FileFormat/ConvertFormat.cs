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
namespace Yarhl.FileFormat
{
    using System;

    /// <summary>
    /// Convert formats with converters.
    /// </summary>
    public static partial class ConvertFormat
    {
        /// <summary>
        /// Converts the format using a converter with the specified type.
        /// </summary>
        /// <param name="converterType">Type of the converter.</param>
        /// <param name="src">Format to convert.</param>
        /// <param name="args">
        /// Arguments for the constructor of the type if any.
        /// </param>
        /// <returns>The new format.</returns>
        public static object With(Type converterType, dynamic src, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(converterType);
            if (src is not null) {
                ValidateConverterType(converterType, src.GetType());
            }

            dynamic converter = Activator.CreateInstance(converterType, args)
                ?? throw new InvalidOperationException("Invalid converter type");

            return converter.Convert(src);
        }

        /// <summary>
        /// Validates the type implements the expected interface for the input.
        /// </summary>
        /// <param name="converterType">The type of the converter.</param>
        /// <param name="inputType">The type of the input.</param>
        /// <exception cref="InvalidOperationException">
        /// The converter does not implement the interface.
        /// </exception>
        internal static void ValidateConverterType(Type converterType, Type inputType)
        {
            Type[] converterInterfaces = converterType.GetInterfaces();
            bool implementConverter = Array.Exists(converterInterfaces, i =>
                i.IsGenericType &&
                i.GetGenericTypeDefinition() == typeof(IConverter<,>));
            if (!implementConverter) {
                throw new InvalidOperationException(
                    "Converter doesn't implement IConverter<,>");
            }

            bool canConvert = Array.Exists(converterInterfaces, i =>
                i.IsGenericType &&
                i.GenericTypeArguments.Length == 2 &&
                i.GenericTypeArguments[0].IsAssignableFrom(inputType));
            if (!canConvert) {
                throw new InvalidOperationException(
                    $"Converter cannot convert the type: {inputType.FullName}");
            }
        }
    }
}
