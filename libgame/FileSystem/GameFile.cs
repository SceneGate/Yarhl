//
//  GameFile.cs
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
namespace Libgame.FileSystem
{
    using System;
    using FileFormat;

    /// <summary>
    /// File with an associated format.
    /// </summary>
    public class GameFile : FileContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameFile"/> class.
        /// </summary>
        /// <param name="name">File name.</param>
        public GameFile(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GameFile"/> class.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="format">File format.</param>
        public GameFile(string name, Format format)
            : this(name)
        {
            Format = format;
        }

        /// <summary>
        /// Gets or sets the current format of the file.
        /// </summary>
        /// <value>The current format.</value>
        public Format Format {
            get;
            set;
        }

        /// <summary>
        /// Transforms the file format to the specified format.
        /// </summary>
        /// <returns>This file.</returns>
        /// <param name="disposeOldFormat">
        /// If set to <c>true</c> dispose the previous format.
        /// </param>
        /// <typeparam name="T">The new file format.</typeparam>
        public GameFile TransformTo<T>(bool disposeOldFormat = true)
            where T : Format
        {
            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a file without format");
            }

            Format newFormat = Format.ConvertTo<T>();
            if (disposeOldFormat)
                Format.Dispose();

            Format = newFormat;
            return this;
        }

        /// <summary>
        /// Transforms the file format with the specified converter to a format.
        /// </summary>
        /// <returns>This file.</returns>
        /// <param name="converter">The format converter to use.</param>
        /// <param name="disposeOldFormat">
        /// If set to <c>true</c> dispose the previous format.
        /// </param>
        /// <typeparam name="T">The new file format.</typeparam>
        public GameFile TransformWith<T>(dynamic converter, bool disposeOldFormat = true)
            where T : Format
        {
            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a file without format");
            }

            Format newFormat = Format.ConvertWith<T>(converter);
            if (disposeOldFormat)
                Format.Dispose();

            Format = newFormat;
            return this;
        }
    }
}
