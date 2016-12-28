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
    using FileSystem;
    using Mono.Addins;

    /// <summary>
    /// Node container format for unpack / pack files.
    /// </summary>
    [Extension]
    public class NodeContainerFormat : Format
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeContainerFormat"/> class.
        /// </summary>
        public NodeContainerFormat()
        {
            Root = new Node("NodeContainerRoot");
        }

        /// <summary>
        /// Gets the format name.
        /// </summary>
        /// <value>The format name.</value>
        public override string Name {
            get { return "libgame.nodecontainer"; }
        }

        /// <summary>
        /// Gets the root node containing the children.
        /// </summary>
        /// <value>The root node.</value>
        public Node Root {
            get;
            private set;
        }

        protected override void Dispose(bool freeManagedResourcesAlso)
        {
            base.Dispose(freeManagedResourcesAlso);
            if (freeManagedResourcesAlso)
                Root.Dispose();
        }
    }
}
