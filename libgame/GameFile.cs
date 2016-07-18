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
    using System.Collections.Generic;
    using System.Linq;
    using Libgame.IO;
    using Libgame.FileFormat;

    /// <summary>
    /// Description of GameFile.
    /// </summary>
    public class GameFile : FileContainer
    {
        private readonly IList<Format> formats;

        public GameFile(string name, Format format)
            : base(name)
        {
            this.Format = format;
            this.formats = new List<Format>();
        }

        public GameFile(string name, DataStream stream)
            : this(name, new BinaryFormat(stream))
        {
        }

        public GameFile(string name, System.IO.Stream stream, long offset, long length)
            : this(name, new DataStream(stream, offset, length))
        {
        }

        public DataStream Stream {
            get {
                var bin = formats.FirstOrDefault(f => f is BinaryFormat);
                return (bin as BinaryFormat)?.Stream;
            }
        }

        public Format Format {
            get { return formats.LastOrDefault(); }
            set { formats.Add(value); }
        }

        public GameFile Transform<T>()
            where T : Format
        {
            Format = Format?.ConvertTo<T>();
            return this;
        }

        public GameFile TransformWith<T>(dynamic converter)
            where T : Format
        {
            Format = Format?.ConvertWith<T>(converter);
            return this;
        }
    }
}
