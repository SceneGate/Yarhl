﻿//
// NavegableNodeCollection.cs
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Read-only collection of NavegableNodes.
    /// </summary>
    /// <typeparam name="T">The implementation of NavegableNodes</typeparam>
    public class NavegableNodeCollection<T> : ReadOnlyCollection<T>
        where T : NavegableNode<T>
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:Yarhl.FileSystem.NavegableNodeCollection`1"/> class.
        /// </summary>
        /// <param name="list">Original list of nodes.</param>
        public NavegableNodeCollection(IList<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets the <see cref="T:Yarhl.FileSystem.NavegableNodeCollection`1"/>
        /// with the specified name.
        /// </summary>
        /// <param name="name">Node name.</param>
        /// <returns>The node with the same name or null if not found.</returns>
        public T this[string name] {
            get { return this.FirstOrDefault((node) => node.Name == name); }
        }
    }
}
