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

            // Call the convert method, this operations is unsafe because
            // we are calling code from plugins.
            return converter.Convert(src);
        }

        public T ConvertTo<T>()
        {
            return Format.ConvertTo<T>(this);
        }

        public T ConvertWith<T>(dynamic converter)
        {
            if (!converter.GetType().GetInterfaces().Contains(typeof(IConverter<,>)))
                throw new ArgumentException("Invalid converter");

            if (converter.GetType().GenericTypeArguments[0] != this.GetType())
                throw new ArgumentException("Converter cannot convert from this type");

            if (converter.GetType().GenericTypeArguments[1] != typeof(T))
                throw new ArgumentException("Converter cannot convert to that type");

            return converter.Convert(this);
        }
    }
}
