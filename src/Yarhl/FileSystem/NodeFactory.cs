// Copyright (c) 2019 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.FileSystem
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Yarhl.IO;

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
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
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
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
        public static Node FromMemory(string name)
        {
            return new Node(name, new BinaryFormat());
        }

        /// <summary>
        /// Creates a Node from a part of a stream.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="source">The source / parent stream.</param>
        /// <param name="offset">
        /// The offset in the source stream where the node starts.
        /// </param>
        /// <param name="length">The length of the data in the node.</param>
        /// <returns>The new node.</returns>
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
        public static Node FromSubstream(
            string name,
            DataStream source,
            long offset,
            long length)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            var binary = new BinaryFormat(source, offset, length);
            return new Node(name, binary);
        }

        /// <summary>
        /// Creates a Node from a file.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="filePath">File path.</param>
        public static Node FromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            string filename = Path.GetFileName(filePath);
            return FromFile(filePath, filename);
        }

        /// <summary>
        /// Creates a Node from a file.
        /// </summary>
        /// <returns>The node.</returns>
        /// <param name="filePath">File path.</param>
        /// <param name="nodeName">Node name.</param>
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
        public static Node FromFile(string filePath, string nodeName)
        {
            // We need to catch if the node creation fails
            // for instance for null names, to dispose the stream.
            FileOpenMode mode = FileOpenMode.ReadWrite;
            var format = new BinaryFormat(DataStreamFactory.FromFile(filePath, mode));
            Node node;
            try {
                node = new Node(nodeName, format)
                {
                    Tags = { ["FileInfo"] = new FileInfo(filePath) },
                };
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
        /// If <see langword="true" /> it searchs recursively in subdirectories.
        /// </param>
        [SuppressMessage(
            "Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Ownserhip dispose transferred")]
        public static Node FromDirectory(
            string dirPath,
            string filter,
            string nodeName,
            bool subDirectories = false)
        {
            var options = subDirectories ?
                SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            // This sanitizes the path and remove double slashes
            dirPath = Path.GetFullPath(dirPath);

            Node folder = CreateContainer(nodeName);
            folder.Tags["DirectoryInfo"] = new DirectoryInfo(dirPath);

            foreach (string filePath in Directory.GetFiles(dirPath, filter, options)) {
                string relParent = Path.GetDirectoryName(filePath)
                                       .Replace(dirPath, string.Empty);
                CreateContainersForChild(folder, relParent, FromFile(filePath));
            }

            foreach (Node node in Navigator.IterateNodes(folder)) {
                if (!node.IsContainer || node.Tags.ContainsKey("DirectoryInfo"))
                    continue;

                int rootPathLength = $"{NodeSystem.PathSeparator}{nodeName}".Length;
                string nodePath = Path.GetFullPath(string.Concat(dirPath, node.Path.Substring(rootPathLength)));
                node.Tags["DirectoryInfo"] = new DirectoryInfo(nodePath);
            }

            return folder;
        }
    }
}
