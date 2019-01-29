// Node.cs
//
// Author:
//      Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
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
namespace Yarhl.FileSystem
{
    using System;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Node in the FileSystem with an associated format.
    /// </summary>
    public class Node : NavigableNode<Node>
    {
        Format format;

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Node name.</param>
        public Node(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class.
        /// </summary>
        /// <param name="name">Node name.</param>
        /// <param name="initialFormat">Node format.</param>
        public Node(string name, Format initialFormat)
            : this(name)
        {
            Format = initialFormat;
        }

        /// <summary>
        /// Gets or sets the current format of the node.
        /// </summary>
        /// <value>The current format.</value>
        public Format Format {
            get {
                return format;
            }

            set {
                if (Disposed)
                    throw new ObjectDisposedException(nameof(Node));

                // If it was a container, clean children
                if (IsContainer)
                    RemoveChildren();

                format?.Dispose();
                format = value;

                // If now it's a container, add its children
                if (IsContainer)
                    AddContainerChildren();
            }
        }

        /// <summary>
        /// Gets the node associated DataStream if the format is BinaryFormat.
        /// </summary>
        /// <value>
        /// DataStream if the format is BinaryFormat, null otherwise.
        /// </value>
        public DataStream Stream {
            get { return GetFormatAs<BinaryFormat>()?.Stream; }
        }

        /// <summary>
        /// Gets a value indicating whether the format is a container of subnodes.
        /// </summary>
        /// <value><c>true</c> if is container; otherwise, <c>false</c>.</value>
        public bool IsContainer {
            get { return Format is NodeContainerFormat; }
        }

        /// <summary>
        /// Gets the format as the specified type.
        /// </summary>
        /// <returns>The format casted to the type or null if not possible.</returns>
        /// <typeparam name="T">The format type.</typeparam>
        public T GetFormatAs<T>()
            where T : Format
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            return Format as T;
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="dst">Format to convert.</param>
        /// <param name="converter">The format converter to use.</param>
        public Node Transform(Type dst, dynamic converter = null)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            if (converter == null)
                Format = (Format)Format.ConvertTo(dst);
            else
                Format = (Format)Format.ConvertWith(converter, dst);

            return this;
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <typeparam name="T">The new node format.</typeparam>
        public Node Transform<T>()
            where T : Format
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            Format = Format.ConvertTo<T>(Format);
            return this;
        }

        /// <summary>
        /// Transform the node format to another format with a converter of that type.
        /// </summary>
        /// <returns>This node.</returns>
        /// <typeparam name="TConv">The type of the converter to use.</typeparam>
        /// <typeparam name="TSrc">The type of the current format.</typeparam>
        /// <typeparam name="TDst">The type of the new format.</typeparam>
        public Node Transform<TConv, TSrc, TDst>()
            where TSrc : Format
            where TDst : Format
            where TConv : IConverter<TSrc, TDst>, new()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            Format = Format.ConvertWith<TConv, TSrc, TDst>();
            return this;
        }

        /// <summary>
        /// Transform the node format to another format using a converter.
        /// </summary>
        /// <param name="converter">Convert to use.</param>
        /// <typeparam name="TSrc">The type of the source format.</typeparam>
        /// <typeparam name="TDst">The type of the destination format.</typeparam>
        /// <returns>This node.</returns>
        public Node Transform<TSrc, TDst>(IConverter<TSrc, TDst> converter)
            where TSrc : Format
            where TDst : Format
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            Format = Format.ConvertWith(converter);
            return this;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Node"/>
        /// object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">If set to <c>true</c> free
        /// managed resources also.</param>
        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            base.Dispose(freeManagedResourcesAlso);
            if (freeManagedResourcesAlso) {
                format?.Dispose();
            }
        }

        void AddContainerChildren()
        {
            RemoveChildren();
            GetFormatAs<NodeContainerFormat>().MoveChildrenTo(this);
        }
    }
}
