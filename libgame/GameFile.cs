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
namespace Libgame
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

        public GameFile(string name)
            : base(name)
        {
            formats = new List<Format>();
            FormatHistory = new ReadOnlyCollection<Format>(formats);
        }

        public GameFile(string name, Format format)
            : this(name)
        {
            formats.Add(format);
        }

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

        public void OverrideFormat(Format format, bool disposePreviousFormats)
        {
            CleanFormatHistory(disposePreviousFormats);
            if (format != null)
                formats.Add(format);
        }

        public void CleanFormatHistory(bool disposeFormats)
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
