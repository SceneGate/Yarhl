//
// Navigator.cs
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
namespace Libgame.FileSystem
{
    using System;
    using System.Collections.Generic;

    public class Navigator
    {
        public Navigator(NavegableNode node)
        {
            Node = node;
        }

        public NavegableNode Node {
            get;
            private set;
        }

        /// <summary>
        /// Search a node by path.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// <returns>Node or null if not found.</returns>
        public NavegableNode SearchFile(string path)
        {
            if (!path.StartsWith(Node.Path, StringComparison.InvariantCulture))
                return null;

            var queue = new Queue<NavegableNode>();
            queue.Enqueue(Node);

            while (queue.Count > 0) {
                NavegableNode currentNode = queue.Dequeue();
                if (path == currentNode.Path)
                    return currentNode;

                foreach (NavegableNode child in currentNode.Children)
                    queue.Enqueue(child);
            }

            return null;
        }

        public IEnumerable<NavegableNode> GetFilesRecursive()
        {
            foreach (NavegableNode child in Node.Children)
                yield return child;
        }
    }
}
