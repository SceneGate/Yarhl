//
//  NodeFactory.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
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
    using System.IO;
    using FileFormat;
    using IO;

    public static class NodeFactory
    {
        /// <summary>
        /// Creates a new <see cref="Node"/> with a new NodeContainer format.
        /// </summary>
        /// <returns>The new node.</returns>
        /// <param name="name">Node name.</param>
        public static Node CreateContainer(string name)
        {
            return new Node(name, new NodeContainerFormat());
        }

        public static Node FromPath(string dirPath)
        {
            return FromPath(dirPath, Path.GetDirectoryName(dirPath));
        }

        public static Node FromPath(string dirPath, string dirName)
        {
            Node folder = CreateContainer(dirName);

            foreach (string filePath in Directory.GetFiles(dirPath)) {
                string filename = Path.GetFileName(filePath);
                DataStream stream = new DataStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                folder.Add(new Node(filename, new BinaryFormat(stream)));
            }

            return folder;
        }
    }
}
