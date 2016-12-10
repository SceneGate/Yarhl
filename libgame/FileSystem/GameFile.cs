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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using FileFormat;

    /// <summary>
    /// File with an associated format.
    /// </summary>
    public class GameFile : FileContainer
    {
        readonly IList<Format> formats;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Libgame.GameFile"/> class.
        /// </summary>
        /// <param name="name">File name.</param>
        public GameFile(string name)
            : base(name)
        {
            formats = new List<Format>();
            FormatHistory = new ReadOnlyCollection<Format>(formats);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Libgame.GameFile"/> class.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="format">File format.</param>
        public GameFile(string name, Format format)
            : this(name)
        {
            formats.Add(format);
        }

        /// <summary>
        /// Gets a list with the format history.
        /// </summary>
        /// <remarks>
        /// The format history contains all the formats the file had from its origin
        /// to the current state and it's the equivalent of all its conversions.
        /// </remarks>
        /// <value>The format history.</value>
        public ReadOnlyCollection<Format> FormatHistory {
            get;
            private set;
        }

        /// <summary>
        /// Gets the current format of the file.
        /// </summary>
        /// <value>The current format.</value>
        public Format Format {
            get { return formats.Count > 0 ? formats[formats.Count - 1] : null; }
        }

        /// <summary>
        /// Transforms the file format to the specified format.
        /// </summary>
        /// <remarks>
        /// The previous format will be stored in the format history.
        /// </remarks>
        /// <returns>This file.</returns>
        /// <typeparam name="T">The new file format.</typeparam>
        public GameFile TransformTo<T>()
            where T : Format
        {
            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a file without format");
            }

            formats.Add(Format.ConvertTo<T>());
            return this;
        }

        /// <summary>
        /// Transforms the file format with the specified converter to a format.
        /// </summary>
        /// <remarks>
        /// The previous format will be stored in the format history.
        /// </remarks>
        /// <returns>This file.</returns>
        /// <param name="converter">The format converter to use.</param>
        /// <typeparam name="T">The new file format.</typeparam>
        public GameFile TransformWith<T>(dynamic converter)
            where T : Format
        {
            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a file without format");
            }

            formats.Add(Format.ConvertWith<T>(converter));
            return this;
        }

        /// <summary>
        /// Sets the file format.
        /// </summary>
        /// <remarks>
        /// It will reset the format history.
        /// </remarks>
        /// <param name="newFormat">New format.</param>
        /// <param name="disposePreviousFormats">
        /// If set to <c>true</c> dispose previous formats.
        /// </param>
        public void SetFormat(Format newFormat, bool disposePreviousFormats)
        {
            CleanFormats(disposePreviousFormats);
            if (newFormat != null)
                formats.Add(newFormat);
        }

        /// <summary>
        /// Cleans the format history and the current format.
        /// </summary>
        /// <param name="disposeFormats">If set to <c>true</c> dispose formats.</param>
        public void CleanFormats(bool disposeFormats)
        {
            if (disposeFormats) {
                foreach (Format format in FormatHistory) {
                    format.Dispose();
                }
            }

            formats.Clear();
        }
    }
}
