//
//  IConverter.cs
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
    using Mono.Addins;

    /// <summary>
    /// Format converter interface.
    /// </summary>
    [TypeExtensionPoint]
    public interface IConverter<TSrc, TDst>
    {
        /// <summary>
        /// Converts the specified source into the given type.
        /// </summary>
        /// <returns>The converted source.</returns>
        /// <param name="source">Source format to convert.</param>
        TDst Convert(TSrc source);
    }
}
