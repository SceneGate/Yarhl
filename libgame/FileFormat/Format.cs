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

    [TypeExtensionPoint]
    /// <summary>
    /// Abstract format.
    /// </summary>
    public abstract class Format : IDisposable
    {
        ~Format()
        {
            this.Dispose(false);
        }

        public abstract string Name {
            get;
        }

        public void Dispose()
        {
            this.Dispose(true);         // Dispose me everything (L)
            GC.SuppressFinalize(this);  // Don't dispose again!
        }

        protected abstract void Dispose(bool freeManagedResourcesAlso);

        public static dynamic ConvertFrom<TSrc>(TSrc source, Type dstType)
        {
            return Convert(typeof(TSrc), source, dstType);
        }

        public static TDst ConvertTo<TDst>(dynamic source)
        {
            return Convert(source.GetType(), source, typeof(TDst));
        }

        public static TDst Convert<TSrc, TDst>(TSrc source)
        {
            return Convert(typeof(TSrc), source, typeof(TDst));
        }

        public static dynamic Convert(Type srcType, dynamic src, Type dstType)
        {
            // Search the converter for the giving types and create an instance
            dynamic converter;
            try {
                Type converterType = AddinManager
                    .GetExtensionNodes<TypeExtensionNode>(typeof(IConverter<,>))
                    .Single(node =>
                        node.Type.GetInterfaces().Any(type =>
                            type.GenericTypeArguments.Length == 2 &&
                            type.GenericTypeArguments[0] == srcType &&
                            type.GenericTypeArguments[1] == dstType))
                    .Type;

                converter = Activator.CreateInstance(converterType);
            } catch (InvalidOperationException ex) {
                throw new InvalidOperationException(
                    "No unique converter for " + srcType + " -> " + dstType,
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
            } catch (MemberAccessException ex) {
                throw new InvalidOperationException(
                    "The converter constructor is not public",
                    ex);
            }

            return converter.Convert(src);
        }

        public static T ConvertWith<T>(dynamic src, dynamic converter)
        {
            return Format.ConvertWith(src, typeof(T), converter);
        }

        public static dynamic ConvertWith(dynamic src, Type dstType, dynamic converter)
        {
            Type[] converterInterfaces = converter.GetType().GetInterfaces();
            bool isConverter = converterInterfaces
                .Any(i => i.GetGenericTypeDefinition() == (typeof(IConverter<,>)));
            if (!isConverter)
                throw new ArgumentException("Invalid converter");

            bool canConvert = converterInterfaces.Any(i =>
                i.IsGenericType &&
                i.GenericTypeArguments.Length == 2 &&
                i.GenericTypeArguments[0] == src.GetType() &&
                i.GenericTypeArguments[1] == dstType);
            if (!canConvert)
                throw new ArgumentException("Converter cannot convert from/to the type");

            return converter.Convert(src); 
        }

        public T ConvertTo<T>()
        {
            return Format.ConvertTo<T>(this);
        }

        public T ConvertWith<T>(dynamic converter)
        {
            return Format.ConvertWith<T>(this, converter);
        }
    }
}
