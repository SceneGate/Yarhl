// Navigable.cs
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
    using System.Collections.Generic;

    /// <summary>
    /// Node with navigation features inside a FileSystem.
    /// </summary>
    /// <typeparam name="T">The implementation of NavigableNodes.</typeparam>
    public abstract class NavigableNode<T> : IDisposable
        where T : NavigableNode<T>
    {
        readonly List<T> children;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="NavigableNode{T}"/> class.
        /// </summary>
        /// <param name="name">Node name.</param>
        protected NavigableNode(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if (name.Contains(NodeSystem.PathSeparator)) {
                throw new ArgumentException(
                    "Name contains invalid characters",
                    nameof(name));
            }

            Name = name;
            Tags = new Dictionary<string, dynamic>();
            children = new List<T>();
            Children = new NavigableNodeCollection<T>(children);
        }

        /// <summary>
        /// Gets the node name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <remarks>
        /// <para>It includes the names of all the parent nodes and this node.</para>
        /// </remarks>
        public string Path {
            get {
                return (Parent?.Path ?? string.Empty) + NodeSystem.PathSeparator + Name;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        public T Parent {
            get;
            private set;
        }

        /// <summary>
        /// Gets a read-only list of children nodes.
        /// </summary>
        public NavigableNodeCollection<T> Children
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the dictionary of tags.
        /// </summary>
        public IDictionary<string, dynamic> Tags {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether this node is disposed.
        /// </summary>
        public bool Disposed {
            get;
            private set;
        }

        /// <summary>
        /// Add a node.
        /// </summary>
        /// <remarks>
        /// <para>Updates the parent of the child node to match this instance.
        /// If the node already contains a child with the same name it will be replaced.
        /// Otherwise the node is added.</para>
        /// </remarks>
        /// <param name="node">Node to add.</param>
        public void Add(T node)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            // Update the parent of the child
            node.Parent = (T)this;

            // If we have already a child with the same, replace it. Otherwise add.
            int index = children.FindIndex((child) => child.Name == node.Name);
            if (index == -1) {
                children.Add(node);
            } else {
                children[index].Dispose();
                children[index] = node;
            }
        }

        /// <summary>
        /// Add a list of nodes.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public void Add(IEnumerable<T> nodes)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (T node in nodes)
                Add(node);
        }

        /// <summary>
        /// Remove a node.
        /// </summary>
        /// <param name="node">Node reference to remove.</param>
        /// <remarks>
        /// <para>This method does NOT dispose the removed node.</para>
        /// </remarks>
        /// <returns>Whether the node was found and removed successfully.</returns>
        public bool Remove(T node)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            if (node == null)
                throw new ArgumentNullException(nameof(node));

            bool result = children.Remove(node);
            if (result) {
                node.Parent = null;
            }

            return result;
        }

        /// <summary>
        /// Remove a node with the specified name.
        /// </summary>
        /// <param name="name">The name of the node to remove.</param>
        /// <remarks>
        /// <para>This method <strong>does</strong> dispose the removed node.
        /// If you don't want to dispose it, search the node and call the
        /// overload with the node argument.</para>
        /// </remarks>
        /// <returns>Whether the node was found and removed successfully.</returns>
        public bool Remove(string name)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            int index = children.FindIndex(child => child.Name == name);
            if (index == -1) {
                return false;
            }

            children[index].Dispose();
            children.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Removes and dispose all the children from the node.
        /// </summary>
        public void RemoveChildren()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            foreach (var child in Children)
                child.Dispose();
            children.Clear();
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
        /// Releases all resource used by the
        /// <see cref="Yarhl.FileSystem.NavigableNode{T}"/> object.
        /// </summary>
        /// <param name="freeManagedResourcesAlso">If set to
        /// <see langword="true" /> free managed resources also.</param>
        protected virtual void Dispose(bool freeManagedResourcesAlso)
        {
            if (Disposed)
                return;

            if (freeManagedResourcesAlso)
                RemoveChildren();

            Disposed = true;
        }
    }
}
