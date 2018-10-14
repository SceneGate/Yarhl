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
namespace Yarhl.FileSystem
{
    using System;
    using System.Composition;
    using FileFormat;

    /// <summary>
    /// Node container format for unpack / pack files.
    /// </summary>
    [Export(typeof(Format))]
    [Format("Yarhl.Common.NodeContainer")]
    public class NodeContainerFormat : Format
    {
        bool manageRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeContainerFormat"/> class.
        /// </summary>
        public NodeContainerFormat()
        {
            Root = new Node("NodeContainerRoot");
            manageRoot = true;
        }

        /// <summary>
        /// Gets the root node containing the children.
        /// </summary>
        /// <value>The root node.</value>
        public Node Root {
            get;
            private set;
        }

        /// <summary>
        /// Moves the children from this format to a <see cref="Node"/>.
        /// </summary>
        /// <remarks>
        /// The node will handle the lifecycle of the children.
        /// Disposing the format won't dispose the children.
        /// </remarks>
        /// <param name="newNode">Node that will contain the children.</param>
        public void MoveChildrenTo(Node newNode)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NodeContainerFormat));

            if (newNode == null)
                throw new ArgumentNullException(nameof(newNode));

            newNode.Add(Root.Children);
            Root = newNode;
            manageRoot = false;
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            base.Dispose(freeManagedResourcesAlso);
            if (freeManagedResourcesAlso && manageRoot)
                Root.Dispose();
        }
    }
}
