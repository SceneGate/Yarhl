//
//  FileInfo.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2014 Benito Palacios Sánchez
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
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Libgame
{
    public class FileInfo
    {
        private List<string> dependencies;

        public FileInfo()
        {
            this.dependencies = new List<string>();
        }

        public string Path {
            get;
            set;
        }

        public string Type {
            get;
            set;
        }

        public XElement Parameters {
            get;
            set;
        }

        public ReadOnlyCollection<string> Dependencies {
            get { return new ReadOnlyCollection<string>(dependencies); }
        }

        public void AddDependency(string path)
        {
            this.dependencies.Add(path);
        }

        public void RemoveDependency(string path)
        {
            this.dependencies.Remove(path);
        }
    }
}

