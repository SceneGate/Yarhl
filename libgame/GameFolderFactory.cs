//
//  GameFolderFactory.cs
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
using System.IO;
using Libgame;
using Libgame.IO;
using Libgame.FileFormat;
using Libgame.FileSystem;

namespace Libgame
{
    public static class GameFolderFactory
    {
        public static GameFolder FromPath(string dir)
        {
            return FromPath(dir, Path.GetDirectoryName(dir));
        }

        public static GameFolder FromPath(string dir, string dirName)
        {
            GameFolder folder = new GameFolder(dirName);

            foreach (string filePath in Directory.GetFiles(dir)) {
                string filename = Path.GetFileName(filePath);
                DataStream stream = new DataStream(filePath, FileMode.Open, FileAccess.ReadWrite);
                folder.AddFile(new GameFile(filename, new BinaryFormat(stream)));
            }

            return folder;
        }
    }
}

