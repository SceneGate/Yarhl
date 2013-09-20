//-----------------------------------------------------------------------
// <copyright file="FileContainer.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>11/06/2013</date>
//-----------------------------------------------------------------------
namespace Libgame
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    
    /// <summary>
    /// Description of FileContainer.
    /// </summary>
    public abstract class FileContainer
    {      
		private FileContainer previousContainer;
        private List<FileContainer> files;
        private List<FileContainer> folders;
		private Dictionary<string, string> tags = new Dictionary<string, string>();
        
        /// <summary>
        /// Initializes a new instance of the <see cref="GameFolder" /> class.
        /// </summary>
        /// <param name="name">Folder name</param>
        /// <param name="id">Folder ID</param>
        public FileContainer(string name)
        {
            this.Name = name;
            
            this.files   = new List<FileContainer>();
            this.folders = new List<FileContainer>();
        }
        
        #region Properties
        
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
            set;
        }
        
		/// <summary>
		/// Gets or sets the path.
		/// </summary>
		/// <value>The path.</value>
		public string Path {
			get;
			private set;
		}

        /// <summary>
        /// Gets the list of files.
        /// </summary>
        public ReadOnlyCollection<FileContainer> Files 
        {
            get { return new ReadOnlyCollection<FileContainer>(this.files); }
        }
        
        /// <summary>
        /// Gets the list of folders.
        /// </summary>
        public ReadOnlyCollection<FileContainer> Folders
        {
            get { return new ReadOnlyCollection<FileContainer>(this.folders); }
        }
        
		/// <summary>
		/// Gets the FileContainers that contains this instance.
		/// </summary>
		/// <value>The previous file container.</value>
		public FileContainer PreviousContainer 
		{
			get { return this.previousContainer; }
		}

		/// <summary>
		/// Gets the tags of this container.
		/// </summary>
		/// <value>The tags.</value>
		public IDictionary<string, string> Tags {
			get { return this.tags; }
		}

        #endregion
        
        /// <summary>
        /// Add a file to the folder.
        /// </summary>
        /// <param name="file">File to add.</param>
        public void AddFile(FileContainer file)
        {
			this.AddContainer(file, this.files);
        }
        
        /// <summary>
        /// Add a list of files to the folder.
        /// </summary>
        /// <param name="files">List of files to add.</param>
        public void AddFiles(IEnumerable<FileContainer> files)
        {
			foreach (FileContainer file in files) {
				this.AddFile(file);
			}
        }
       
		/// <summary>
		/// Add a subfolder to the folder.
		/// </summary>
		/// <param name="folder">Folder to add.</param>
		public void AddFolder(FileContainer folder)
		{
			this.AddContainer(folder, this.folders);
		}

		/// <summary>
		/// Add a list of subfolders to the folder.
		/// </summary>
		/// <param name="folders">List of folders to add.</param>
		public void AddFolders(IEnumerable<FileContainer> folders)
		{
			foreach (FileContainer folder in folders) {
				this.AddFolder(folder);
			}
		} 

		private void AddContainer(FileContainer element, List<FileContainer> list)
		{
			string elementPath = this.Path + PathSeparator + element.Name;
			element.previousContainer = this;

			// For each child, update it's path variable
			Queue<FileContainer> queue = new Queue<FileContainer>();
			queue.Enqueue(element);
			while (queue.Count > 0) {
				FileContainer child = queue.Dequeue();
				child.Path = elementPath + child.Path;

				foreach (FileContainer subchild in child.files)
					queue.Enqueue(subchild);

				foreach (FileContainer subchild in child.folders)
					queue.Enqueue(subchild);
			}

			// If the name matches, replace
			for (int i = 0; i < list.Count; i++) {
				if (list[i].Name == element.Name) {
					list[i] = element;
					return;
				}
			}

			list.Add(element);
		}

        /// <summary>
        /// Search an element by path.
        /// </summary>
        /// <param name="path">Path to search.</param>
        /// <returns>File/Folder or null if not found.</returns>
        public FileContainer SearchFile(string path)
        {            
			if (!path.StartsWith(this.Path))
				return null;

			if (path == this.Path)
				return this;
            
			foreach (FileContainer f in this.files) {
				FileContainer el = f.SearchFile(path);
				if (el != null)
					return el;
			}

			foreach (FileContainer f in this.folders) {
				FileContainer el = f.SearchFile(path);
				if (el != null)
					return el;
			}

			return null;
        }
        
        public void Clear()
        {
            this.files.Clear();
            this.folders.Clear();
        }

		public FileContainer[] GetFilesRecursive()
		{
			List<FileContainer> list = new List<FileContainer>();

			foreach (FileContainer f in this.files) {
				list.Add(f);
				list.AddRange(f.GetFilesRecursive());
			}

			foreach (FileContainer f in this.folders) {
				list.AddRange(f.GetFilesRecursive());
			}

			return list.ToArray();
		}
    }
}
