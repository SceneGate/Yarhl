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
    public class Node : NavegableNode<Node>, IDisposable
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
        /// Finalizes an instance of the <see cref="Node"/> class.
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Node"/> is reclaimed by garbage collection.
        /// </summary>
        ~Node()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="Format"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
        public bool Disposed {
            get;
            private set;
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

                format = value;

                // If now it's a container, add its children
                if (IsContainer)
                    AddContainerChildren();
            }
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
            return Format as T;
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="dst">Format to convert.</param>
        /// <param name="disposeOldFormat">
        /// If set to <c>true</c> dispose the previous format.
        /// </param>
        /// <param name="converter">The format converter to use.</param>
        public Node Transform(Type dst, bool disposeOldFormat = true, dynamic converter = null)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            Format newFormat;
            if (converter == null)
                newFormat = Format.ConvertTo(dst);
            else
                newFormat = FileFormat.Format.ConvertWith(Format, dst, converter);

            if (disposeOldFormat)
                Format.Dispose();

            Format = newFormat;

            return this;
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="disposeOldFormat">
        /// If set to <c>true</c> dispose the previous format.
        /// </param>
        /// <param name="converter">The format converter to use.</param>
        /// <typeparam name="T">The new node format.</typeparam>
        public Node Transform<T>(bool disposeOldFormat = true, dynamic converter = null)
            where T : Format
        {
            return Transform(typeof(T), disposeOldFormat, converter);
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Node"/>
        /// object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);              // Dispose me everything (L)
            GC.SuppressFinalize(this);  // Don't dispose again!
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Node"/>
        /// object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">If set to <c>true</c> free
        /// managed resources also.</param>
        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            if (Disposed)
                return;

            Disposed = true;

            if (freeManagedResourcesAlso)
                format?.Dispose();
        }

        void AddContainerChildren()
        {
            RemoveChildren();
            Add((Format as NodeContainerFormat).Children);
        }
    }
}
