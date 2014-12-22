//
//  FileInfoCollection.cs
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
using System.Collections.Generic;
using System.Xml.Linq;

namespace Libgame
{
	public class FileInfoCollection
	{
		private Dictionary<string, FileInfo> treasureMap;

		public FileInfoCollection()
		{
			this.treasureMap = new Dictionary<string, FileInfo>();
		}

		public static FileInfoCollection FromXml(XDocument xmlGame)
		{
			FileInfoCollection collection = new FileInfoCollection();
			XElement files = xmlGame.Root.Element("Files");

			foreach (XElement fileInfo in files.Elements("FileInfo")) {
				FileInfo info = new FileInfo();
				info.Path = fileInfo.Element("Path").Value;
				info.Type = fileInfo.Element("Type").Value;
				info.Parameters = fileInfo.Element("Parameters");
				collection.AddFileInfo(info);
			}

			return collection;
		}

		public bool Contains(string path)
		{
			return this.treasureMap.ContainsKey(path);
		}

		public void AddFileInfo(FileInfo info)
		{
			this.treasureMap.Add(info.Path, info);
		}

		public FileInfo GetFileInfo(string path)
		{
			return this.treasureMap[path];
		}

		public FileInfo this[string path] {
			get {
				return this.treasureMap[path];
			}

			set {
				this.treasureMap.Add(path, value);
			}
		}
	}
}

