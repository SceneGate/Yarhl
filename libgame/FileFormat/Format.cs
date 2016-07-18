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
    using System.Reflection;
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
            var converterType = Assembly.GetExecutingAssembly().GetTypes()
                .Single(type =>               
                    type.IsClass &&
                    type.GetInterfaces().Any(inter =>
                        inter.IsGenericType &&
                        inter.GetGenericTypeDefinition().Equals(typeof(IConverter<,>)) &&
                        inter.GenericTypeArguments[0] == srcType &&
                        inter.GenericTypeArguments[1] == dstType));

            dynamic converter = Activator.CreateInstance(converterType);
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
