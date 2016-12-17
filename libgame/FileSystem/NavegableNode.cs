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
    using System.Collections.Generic;

    public abstract class NavegableNode
    {    
        readonly List<NavegableNode> children;

        protected NavegableNode(string name)
        {
            Name = name;
            Tags = new Dictionary<string, dynamic>();
            children = new List<NavegableNode>();
            Children = new NavegableNodeCollection(children);
        }

        /// <summary>
        /// Gets the path separator.
        /// </summary>
        /// <value>The path separator.</value>
        public static char PathSeparator {
            get { return '/'; }
        }

        /// <summary>
        /// Gets the folder name.
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
            get { return (Parent?.Path ?? "") + PathSeparator + Name; }
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
        /// Gets the list of children nodes.
        /// </summary>
        /// <value>The list of children.</value>
        public NavegableNodeCollection Children 
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tags.
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
        /// The parent of the child node is update to match this instance.
        /// If the node already contains a child with the same name it will be replaced.
        /// Otherwise the node is added.
        /// </remarks>
        /// <param name="child">Node to add.</param>
        public void AddChild(NavegableNode child)
        {
            // Update the parent of the child
            child.Parent = this;

            // If we have already a child with the same, replace it. Otherwise add.
            int index = children.FindIndex((node) => node.Name == child.Name);
            if (index == -1)
                children.Add(child);
            else
                children[index] = child;
        }
        
        /// <summary>
        /// Add a list of nodes.
        /// </summary>
        /// <param name="children">List of nodes to add.</param>
        public void AddChildren(IEnumerable<NavegableNode> children)
        {
            foreach (NavegableNode child in children)
                AddChild(child);
        }

        public void RemoveChildren()
        {
            children.Clear();
        }
    }
}
