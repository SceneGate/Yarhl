//
//  Node.cs
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
    /// Node in the FileSystem with an associated format.
    /// </summary>
    public class Node : FileContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        public Node(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="format">Format.</param>
        public Node(string name, Format format)
            : this(name)
        {
            Format = format;
        }

        /// <summary>
        /// Gets or sets the current format of the node.
        /// </summary>
        /// <value>The current format.</value>
        public Format Format {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the format is a container of subnodes.
        /// </summary>
        /// <value><c>true</c> if is container; otherwise, <c>false</c>.</value>
        public bool IsContainer {
            get { return Format is NodeContainerFormat; }
        }

        /// <summary>
        /// Creates a new <see cref="Node"/> with a new NodeContainer format.
        /// </summary>
        /// <returns>The new node.</returns>
        /// <param name="name">Node name.</param>
        public static Node CreateContainer(string name)
        {
            return new Node(name, new NodeContainerFormat());
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="disposeOldFormat">
        /// <param name="converter">The format converter to use.</param>
        /// If set to <c>true</c> dispose the previous format.
        /// </param>
        /// <typeparam name="T">The new node format.</typeparam>
        public Node Transform<T>(bool disposeOldFormat = true, dynamic converter = null)
            where T : Format
        {
            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            Format newFormat;
            if (converter == null)
                newFormat = Format.ConvertTo<T>();
            else
                newFormat = Format.ConvertWith<T>(converter);

            if (disposeOldFormat)
                Format.Dispose();

            Format = newFormat;
            return this;
        }
    }
}
