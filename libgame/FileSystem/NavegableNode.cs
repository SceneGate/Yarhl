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
    public abstract class NavegableNode
    {    
        readonly List<NavegableNode> children;

        protected NavegableNode(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            Tags = new Dictionary<string, dynamic>();
            children = new List<NavegableNode>();
            Children = new NavegableNodeCollection(children);
        }

        /// <summary>
        /// Gets the path separator.
        /// </summary>
        /// <value>The path separator.</value>
        public static string PathSeparator {
            get { return "/"; }
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
        /// <value>The path.</value>
        public string Path {
            get { return (Parent?.Path ?? string.Empty) + PathSeparator + Name; }
        }

        /// <summary>
        /// Gets the parent node.
        /// </summary>
        /// <value>The node parent.</value>
        public NavegableNode Parent {
            get;
            private set;
        }

        /// <summary>
        /// Gets a read-only list of children nodes.
        /// </summary>
        /// <value>The list of children.</value>
        public NavegableNodeCollection Children 
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
        public void Add(NavegableNode node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            // Update the parent of the child
            node.Parent = this;

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
        public void Add(IEnumerable<NavegableNode> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));

            foreach (NavegableNode node in nodes)
                Add(node);
        }

        /// <summary>
        /// Removes all the children from the node.
        /// </summary>
        public void RemoveChildren()
        {
            children.Clear();
        }
    }
}
