//
// NodeContainerFormat.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2016 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Libgame.FileFormat
{
    using System;
    using System.Collections.Generic;
    using FileSystem;
    using Mono.Addins;

    /// <summary>
    /// Node container format for unpack / pack files.
    /// </summary>
    [Extension]
    public class NodeContainerFormat : Format
    {
        readonly Node root;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeContainerFormat"/> class.
        /// </summary>
        public NodeContainerFormat()
        {
            root = new Node("NodeContainerRoot");
            Children = root.Children;
        }

        /// <summary>
        /// Gets the format name.
        /// </summary>
        /// <value>The format name.</value>
        public override string Name {
            get { return "libgame.nodecontainer"; }
        }

        /// <summary>
        /// Gets a read-only list of children nodes.
        /// </summary>
        /// <value>The list of children.</value>
        public NavegableNodeCollection<Node> Children {
            get;
            private set;
        }

        /// <summary>
        /// Add a node.
        /// </summary>
        /// <remarks>
        /// Updates the parent of the child node to match this instance.
        /// If the node already contains a child with the same name it will be replaced.
        /// Otherwise the node is added.
        /// </remarks>
        /// <param name="node">Node to add.</param>
        public void Add(Node node)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NodeContainerFormat));

            root.Add(node);
        }

        /// <summary>
        /// Add a list of nodes.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public void Add(IEnumerable<Node> nodes)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NodeContainerFormat));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (Node node in nodes)
                Add(node);
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            base.Dispose(freeManagedResourcesAlso);
            if (freeManagedResourcesAlso)
                root.Dispose();
        }
    }
}
