//
//  Navegable.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
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
namespace Libgame.FileSystem
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Node with navigation features inside a FileSystem.
    /// </summary>
    /// <typeparam name="T">The implementation of NavegableNodes</typeparam>
    public abstract class NavegableNode<T>
        where T : NavegableNode<T>
    {    
        readonly List<T> children;

        protected NavegableNode(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (name.Contains(NodeSystem.PathSeparator)) {
                throw new ArgumentException(
                    "Name contains invalid characters",
                    nameof(name));
            }

            Name = name;
            Tags = new Dictionary<string, dynamic>();
            children = new List<T>();
            Children = new NavegableNodeCollection<T>(children);
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
        /// It includes the names of all the parent nodes and this node.
        /// </remarks>
        /// <value>The path.</value>
        public string Path {
            get {
                return (Parent?.Path ?? string.Empty) + NodeSystem.PathSeparator + Name;
            }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The node parent.</value>
        public T Parent {
            get;
            private set;
        }

        /// <summary>
        /// Gets a read-only list of children nodes.
        /// </summary>
        /// <value>The list of children.</value>
        public NavegableNodeCollection<T> Children
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the dictionary of tags.
        /// </summary>
        /// <value>The tags.</value>
        public IDictionary<string, dynamic> Tags {
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
        public void Add(T node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            // Update the parent of the child
            node.Parent = (T)this;

            // If we have already a child with the same, replace it. Otherwise add.
            int index = children.FindIndex((child) => child.Name == node.Name);
            if (index == -1)
                children.Add(node);
            else
                children[index] = node;
        }
        
        /// <summary>
        /// Add a list of nodes.
        /// </summary>
        /// <param name="nodes">List of nodes to add.</param>
        public void Add(IEnumerable<T> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (T node in nodes)
                Add(node);
        }

        /// <summary>
        /// Removes all the children from the node.
        /// </summary>
        public virtual void RemoveChildren()
        {
            children.Clear();
        }
    }
}
