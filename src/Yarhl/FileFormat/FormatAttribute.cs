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
namespace Yarhl.FileFormat
{
    using System;
    using Mono.Addins;

    /// <summary>
    /// Class attribute for format extensions.
    /// It allows to give a name to the format.
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FormatAttribute : CustomExtensionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatAttribute"/> class.
        /// </summary>
        public FormatAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormatAttribute"/> class.
        /// </summary>
        /// <param name="name">The format name.</param>
        public FormatAttribute([NodeAttribute("Name")] string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the format name.
        /// </summary>
        /// <value>The format name.</value>
        [NodeAttribute]
        public string Name { get; set; }
    }
}