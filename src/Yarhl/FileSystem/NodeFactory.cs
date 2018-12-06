﻿//
//  NodeFactory.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
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
namespace Yarhl.FileSystem
{
    using System;
    using System.IO;
    using FileFormat;

    /// <summary>
    /// Node factory.
    /// </summary>
    public static class NodeFactory
    {
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
        /// Creates the missing parent nodes to contain the child and add it.
        /// </summary>
        /// <param name="root">The root node that will contain the nodes.</param>
        /// <param name="path">
        /// The path for the child. It doesn't contain the root or child names.</param>
        /// <param name="child">The child to add to root with the path.</param>
        public static void CreateContainersForChild(Node root, string path, Node child)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            // Replace wrong slashes to support native Windows paths
            path = path.Replace("\\", NodeSystem.PathSeparator);

            string[] parentNames = path.Split(
                new[] { NodeSystem.PathSeparator[0] },
                StringSplitOptions.RemoveEmptyEntries);

            Node currentNode = root;
            foreach (string name in parentNames) {
                Node subParent = currentNode.Children[name];
                if (subParent == null) {
                    subParent = CreateContainer(name);
                    currentNode.Add(subParent);
                }

                currentNode = subParent;
            }

            currentNode.Add(child);
        }

        /// <summary>
        /// Creates a Node with a new stream from memory.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <returns>The new node.</returns>
        public static Node FromMemory(string name)
        {
            return new Node(name, new BinaryFormat());
        }

        /// <summary>
        /// Creates a Node from a file.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="filePath">File path.</param>
        public static Node FromFile(string filePath)
        {
            string filename = Path.GetFileName(filePath);
            return FromFile(filePath, filename);
        }

        /// <summary>
        /// Creates a Node from a file.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="nodeName">Node name.</param>
        public static Node FromFile(string filePath, string nodeName)
        {
            // We need to catch if the node creation fails
            // for instance for null names, to dispose the stream.
            var format = new BinaryFormat(filePath);
            Node node;
            try {
                node = new Node(nodeName, format);
            } catch {
                format.Dispose();
                throw;
            }

            return node;
        }

        /// <summary>
        /// Creates a Node containing all the files from the directory.
        /// </summary>
        /// <returns>The container node.</returns>
        /// <param name="dirPath">Directory path.</param>
        /// <param name="filter">Filter for files in directory.</param>
        public static Node FromDirectory(string dirPath, string filter = "*")
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentNullException(nameof(dirPath));

            if (dirPath[dirPath.Length - 1] == Path.DirectorySeparatorChar)
                dirPath = dirPath.Remove(dirPath.Length - 1);

            string dirName = Path.GetFileName(dirPath);
            return FromDirectory(dirPath, filter, dirName);
        }

        /// <summary>
        /// Creates a Node containing all the files from the directory.
        /// </summary>
        /// <returns>The container node.</returns>
        /// <param name="dirPath">Directory path.</param>
        /// <param name="filter">Filter for files in directory.</param>
        /// <param name="nodeName">Node name.</param>
        /// <param name="subDirectories">
        /// If <c>true</c> it searchs recursively in subdirectories.
        /// </param>
        public static Node FromDirectory(
            string dirPath,
            string filter,
            string nodeName,
            bool subDirectories = false)
        {
            var options = subDirectories ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            Node folder = CreateContainer(nodeName);
            foreach (string filePath in Directory.GetFiles(dirPath, filter, options)) {
                string relParent = Path.GetDirectoryName(filePath)
                                       .Replace(dirPath, string.Empty);
                CreateContainersForChild(folder, relParent, FromFile(filePath));
            }

            return folder;
        }
    }
}
