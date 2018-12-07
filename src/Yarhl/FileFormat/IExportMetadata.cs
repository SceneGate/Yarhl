// IExportMetadata.cs
//
// Author:
//      Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
// Copyright (c) 2018 Benito Palacios Sánchez
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

    /// <summary>
    /// Base metadata associated to a exported type.
    /// </summary>
    public interface IExportMetadata
    {
        /// <summary>
        /// Gets or sets the name of the extension.
        /// Usually it's the FullName property of Type.
        /// </summary>
        /// <value>Name of the extension.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the extension.
        /// </summary>
        /// <value>The type of the extension.</value>
        Type Type { get; set; }
    }
}
