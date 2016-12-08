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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Libgame
{
    public class FileInfoCollection
    {
        readonly Dictionary<string, FileInfo> treasureMap;

        public FileInfoCollection()
        {
            treasureMap = new Dictionary<string, FileInfo>();
        }

        public static FileInfoCollection FromXml(XDocument xmlGame)
        {
            var collection = new FileInfoCollection();
            XElement files = xmlGame.Root.Element("Files");

            foreach (XElement fileInfo in files.Elements("FileInfo")) {
                var info = new FileInfo();
                info.Path = fileInfo.Element("Path").Value;
                info.Type = fileInfo.Element("Type").Value;
                info.Parameters = fileInfo.Element("Parameters");
                fileInfo.Elements("DependsOn")
                    .InDocumentOrder()
                    .All(d => { info.AddDependency(d.Value); return true; });
                collection.AddFileInfo(info);
            }

            return collection;
        }

        public bool Contains(string path)
        {
            return treasureMap.ContainsKey(path);
        }

        public void AddFileInfo(FileInfo info)
        {
            treasureMap.Add(info.Path, info);
        }

        public FileInfo GetFileInfo(string path)
        {
            return treasureMap[path];
        }

        public FileInfo this[string path] {
            get {
                return treasureMap[path];
            }

            set {
                treasureMap.Add(path, value);
            }
        }
    }
}

