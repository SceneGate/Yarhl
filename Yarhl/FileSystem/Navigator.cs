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
namespace Yarhl.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// FileSystem navigator.
    /// Search for nodes and iterate over them.
    /// </summary>
    public static class Navigator
    {
        /// <summary>
        /// Search a node by path.
        /// </summary>
        /// <param name="rootNode">The root node to start the search.</param>
        /// <param name="path">Path to search.</param>
        /// <returns>Node or null if not found.</returns>
        /// <typeparam name="T">NavegableNode type</typeparam>
        public static T SearchFile<T>(T rootNode, string path)
            where T : NavegableNode<T>
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!path.StartsWith(rootNode.Path, StringComparison.InvariantCulture))
                return null;

            var queue = new Queue<T>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0) {
                T currentNode = queue.Dequeue();
                if (path == currentNode.Path)
                    return currentNode;

                foreach (T child in currentNode.Children)
                    queue.Enqueue(child);
            }

            return null;
        }

        /// <summary>
        /// Iterates the nodes recursively.
        /// </summary>
        /// <param name="rootNode">The root node to start iterating.</param>
        /// <param name="mode">The navigation mode.</param>
        /// <returns>The nodes.</returns>
        /// <typeparam name="T">NavegableNode type</typeparam>
        public static IEnumerable<T> IterateNodes<T>(
            T rootNode,
            NavigationMode mode = NavigationMode.BreadthFirst)
            where T : NavegableNode<T>
        {
            if (rootNode == null)
                throw new ArgumentNullException(nameof(rootNode));

            if (mode == NavigationMode.BreadthFirst)
                return IterateBreadthFirst(rootNode);
            else if (mode == NavigationMode.DepthFirst)
                return IterateDepthFirst(rootNode);
            else
                throw new ArgumentOutOfRangeException(nameof(mode));
        }

        static IEnumerable<T> IterateBreadthFirst<T>(T rootNode)
            where T : NavegableNode<T>
        {
            var queue = new Queue<T>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0) {
                T currentNode = queue.Dequeue();

                foreach (var child in currentNode.Children) {
                    queue.Enqueue(child);
                    yield return child;
                }
            }
        }

        static IEnumerable<T> IterateDepthFirst<T>(T rootNode)
            where T : NavegableNode<T>
        {
            var stack = new Stack<T>(rootNode.Children.Reverse());

            while (stack.Count > 0) {
                T currentNode = stack.Pop();
                yield return currentNode;

                foreach (var child in currentNode.Children.Reverse())
                    stack.Push(child);
            }
        }
    }
}
