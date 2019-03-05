// Node.cs
//
// Copyright (c) 2019 SceneGate Team
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
        public Node(string name, IFormat initialFormat)
            : this(name)
        {
            ChangeFormat(initialFormat);
        }

        /// <summary>
        /// Gets the current format of the node.
        /// </summary>
        /// <value>The current format.</value>
        public IFormat Format {
            get;
            private set;
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
        /// Gets a value indicating whether the format is a container of nodes.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the format is a container; otherwise,
        /// <see langword="false"/>.
        /// </value>
        public bool IsContainer {
            get { return Format is NodeContainerFormat; }
        }

        /// <summary>
        /// Gets the format as the specified type.
        /// </summary>
        /// <returns>The format casted to the type or null if not possible.</returns>
        /// <typeparam name="T">The format type.</typeparam>
        public T GetFormatAs<T>()
            where T : class, IFormat
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            return Format as T;
        }

        /// <summary>
        /// Change the format of the current node.
        /// </summary>
        /// <remarks>
        /// <para>If the previous format was a container, this method will
        /// remove the children of the node.
        /// If the new format is a container, this method will add the format
        /// children to the node.</para>
        /// </remarks>
        /// <param name="newFormat">The new format to assign.</param>
        /// <param name="disposePreviousFormat">
        /// If <see langword="true" /> the method will dispose the previous
        /// format.
        /// </param>
        public void ChangeFormat(IFormat newFormat, bool disposePreviousFormat = true)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            // If it was a container, clean children
            if (IsContainer) {
                RemoveChildren();
            }

            if (disposePreviousFormat) {
                (Format as IDisposable)?.Dispose();
            }

            Format = newFormat;

            // If now it's a container, add its children
            if (IsContainer) {
                AddContainerChildren();
            }
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <typeparam name="TDst">Format to convert.</typeparam>
        /// <returns>This node.</returns>
        public Node TransformTo<TDst>()
            where TDst : IFormat
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ChangeFormat(ConvertFormat.ConvertTo<TDst>(Format));
            return this;
        }

        /// <summary>
        /// Transforms the node format to the specified format.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="dst">Format to convert. It must implement IFormat.</param>
        public Node TransformTo(Type dst)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (dst == null)
                throw new ArgumentNullException(nameof(dst));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ChangeFormat((IFormat)ConvertFormat.ConvertTo(dst, Format));
            return this;
        }

        /// <summary>
        /// Transform the node format to another format with a given converter.
        /// </summary>
        /// <returns>This node.</returns>
        /// <typeparam name="TConv">The type of the converter to use.</typeparam>
        public Node TransformWith<TConv>()
            where TConv : IConverter, new()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ChangeFormat((IFormat)ConvertFormat.ConvertWith<TConv>(Format));
            return this;
        }

        /// <summary>
        /// Transform the node format to another format with a given converter
        /// initialized with parameters.
        /// </summary>
        /// <returns>This node.</returns>
        /// <typeparam name="TConv">The type of the converter to use.</typeparam>
        /// <typeparam name="TParam">The type for initializing the converter.</typeparam>
        /// <param name="param">Parameters to initialize the converter.</param>
        public Node TransformWith<TConv, TParam>(TParam param)
            where TConv : IConverter, IInitializer<TParam>, new()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            var dst = ConvertFormat.ConvertWith<TConv, TParam>(param, Format);
            ChangeFormat((IFormat)dst);
            return this;
        }

        /// <summary>
        /// Transforms the node format with the specified converter.
        /// </summary>
        /// <returns>This node.</returns>
        /// <param name="converterType">The type of the converter to use.</param>
        public Node TransformWith(Type converterType)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (converterType == null)
                throw new ArgumentNullException(nameof(converterType));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ChangeFormat((IFormat)ConvertFormat.ConvertWith(converterType, Format));
            return this;
        }

        /// <summary>
        /// Transform the node format to another format using a converter.
        /// </summary>
        /// <param name="converter">Convert to use.</param>
        /// <typeparam name="TSrc">The type of the source format.</typeparam>
        /// <typeparam name="TDst">The type of the destination format.</typeparam>
        /// <returns>This node.</returns>
        public Node TransformWith<TSrc, TDst>(IConverter<TSrc, TDst> converter)
            where TSrc : IFormat
            where TDst : IFormat
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (Format == null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ChangeFormat(converter.Convert((TSrc)Format));
            return this;
        }

        /// <summary>
        /// Releases all resource used by the <see cref="Node"/>
        /// object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">
        /// If set to <see langword="true"/> free managed resources also.
        /// </param>
        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            base.Dispose(freeManagedResourcesAlso);
            if (freeManagedResourcesAlso) {
                (Format as IDisposable)?.Dispose();
            }
        }

        void AddContainerChildren()
        {
            RemoveChildren();
            GetFormatAs<NodeContainerFormat>().MoveChildrenTo(this);
        }
    }
}
