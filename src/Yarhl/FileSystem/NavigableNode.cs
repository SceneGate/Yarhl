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
    using System.Collections.Generic;

    /// <summary>
    /// Node with navigation features inside a FileSystem.
    /// </summary>
    /// <typeparam name="T">The implementation of NavigableNodes.</typeparam>
    public abstract class NavigableNode<T> : IDisposable
        where T : NavigableNode<T>
    {
        readonly List<T> children;
        readonly DefaultNavigableNodeComparer defaultComparer = new DefaultNavigableNodeComparer();

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
        public NavigableNodeCollection<T> Children { get; }

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

            // If the path of the node is fully inside our path, it's a parent.
            if (Path.StartsWith(node.Path, StringComparison.Ordinal))
                throw new ArgumentException("Cannot add one parent as child", nameof(node));

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
        /// Sorts the children nodes using the default comparer.
        /// </summary>
        /// <param name="recursive">If set to <see langword="true" /> sorts the children nodes recursively.</param>
        public void SortChildren(bool recursive = true)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            SortChildren(defaultComparer, recursive);
        }

        /// <summary>
        /// Sorts the children nodes using the specified comparer.
        /// </summary>
        /// <param name="comparer">The <see cref="System.Collections.Generic.IComparer{T}" /> implementation to use when comparing elements.</param>
        /// <param name="recursive">If set to <see langword="true" /> sorts the children nodes recursively.</param>
        public void SortChildren(IComparer<T> comparer, bool recursive = true)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            children.Sort(comparer);

            if (recursive) {
                foreach (T node in children) {
                    node.SortChildren(comparer);
                }
            }
        }

        /// <summary>
        /// Sorts the children nodes using the specified <see cref="System.Comparison{T}" />.
        /// </summary>
        /// <param name="comparison">The <see cref="System.Comparison{T}" /> to use when comparing elements.</param>
        /// <param name="recursive">If set to <see langword="true" /> sorts the children nodes recursively.</param>
        public void SortChildren(Comparison<T> comparison, bool recursive = true)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(NavigableNode<T>));

            children.Sort(comparison);

            if (recursive) {
                foreach (T node in children) {
                    node.SortChildren(comparison);
                }
            }
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

        private sealed class DefaultNavigableNodeComparer : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                // x and y cannot be null because Add methods don't allow null parameters.
                return string.Compare(x.Name, y.Name, StringComparison.CurrentCulture);
            }
        }
    }
}
