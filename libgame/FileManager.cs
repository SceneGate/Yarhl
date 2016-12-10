//-----------------------------------------------------------------------
// <copyright file="FileManager.cs" company="none">
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
// <date>22/09/2013</date>
//-----------------------------------------------------------------------
namespace Libgame
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Mono.Addins;
    using Libgame.FileFormat;
    using Libgame.FileSystem;

    public class FileManager
    {
        private static FileManager Instance;

        private FileManager(FileContainer root, FileInfoCollection infoCollection)
        {
            this.Root = root;
            this.InfoCollection = infoCollection;
            InitializeAddins();
        }
            
        public FileContainer Root {
            get;
            private set;
        }

        public FileInfoCollection InfoCollection {
            get;
            private set;
        }

        public static FileManager GetInstance()
         {
            if (!IsInitialized())
                throw new Exception("The class has not been initalized.");
            return Instance;
        }

        private static void InitializeAddins()
        {
            if (!AddinManager.IsInitialized) {
                AddinManager.Initialize(".addins");
                AddinManager.Registry.Update();
                new System.IO.DirectoryInfo(".addins").Attributes |= System.IO.FileAttributes.Hidden;
            }
        }

        public static void Initialize(FileContainer rootDir, FileInfoCollection infoCollection)
        {
            Instance = new FileManager(rootDir, infoCollection);
        }

        public static bool IsInitialized()
        {
            return Instance != null;
        }

        public static Format GetFormat(string name)
        {
            InitializeAddins();
            return AddinManager.GetExtensionObjects<Format>(false).
                Where(f => f.Name == name).
                ToArray()[0];
        }

        public static FormatValidation AssignBestFormat(GameFile file)
        {
            if (file.Format != null)
                return null;

            InitializeAddins();
            FormatValidation validation = AddinManager.GetExtensionObjects<FormatValidation>(false)
                .OrderByDescending((validat) => {validat.RunTests(file); return validat.Result;})
                .FirstOrDefault();

            if (validation != null) {
                validation.AutosetFormat = true;
                validation.RunTests(file);
            }

            return validation;
        }

        public GameFile RescueFile(string gameFilePath)
        {
            if (this.InfoCollection.Contains(gameFilePath))
                return this.RescueFileInfo(gameFilePath);
            else
                return this.RescueFileNoInfo(gameFilePath);
        }

        private GameFile RescueFileNoInfo(string gameFilePath)
        {
            // 1.- Gets dependencies
            // Since no info of dependencies is given, it will search them in two steps.
            List<GameFile> depends = new List<GameFile>();

            // 1.1.- Gets dependencies to get file data.
            // It will be the previous GameFile that contains that file.
            // Reading that file it's expected to get file data.
            string prevContainer = gameFilePath.GetPreviousPath();
            if (!string.IsNullOrEmpty(prevContainer)) {
                GameFile dependency = this.RescueFile(prevContainer);
                if (dependency != null) {
                    // TODO: dependency.Format.Read();
                    depends.Add(dependency);
                }
            }

            // We should be able to get the file now
            FileContainer searchFile = this.Root.SearchFile(gameFilePath);
            GameFile file =  searchFile as GameFile;
            // If we're trying to get the dependency and found a folder, pass its the "dependency"
            if (file == null && searchFile is GameFolder) {
                if (depends.Count > 0)
                    return depends[0];
                else
                    return null;    // Folder without dependencies
            } else if (file == null) {
                throw new Exception("File not found.");
            }
                
            // 1.2.- Gets dependencies to be able to parse data.
            // It will try to guess the file type using FormatValidation classes.
            // If one of the matches, it will provide the dependencies.
            FormatValidation validation = AssignBestFormat(file);
            if (validation != null) {
                foreach (string dependencyPath in validation.Dependencies) {
                    GameFile dependency = this.RescueFile(dependencyPath);
                    depends.Add(dependency);
                    // TODO: dependency.Format.Read();
                }
            }

            // Set dependencies
            // TODO: file.AddDependencies(depends.ToArray());

            return file;
        }

        private GameFile RescueFileInfo(string gameFilePath)
        {
            FileInfo info = this.InfoCollection[gameFilePath];

            // Resolve dependencies
            List<GameFile> depends = new List<GameFile>();

            foreach (string dependencyPath in info.Dependencies) {
                GameFile dependency = this.RescueFile(dependencyPath);
                depends.Add(dependency);
                // TODO: dependency.Format.Read();
            }

            // Get file
            FileContainer searchFile = this.Root.SearchFile(gameFilePath);
            GameFile file =  searchFile as GameFile;
            if (file == null) {
                throw new Exception("File not found.");
            }

            // Add dependencies
            // TODO: file.AddDependencies(depends.ToArray());


            if (file.Format == null) {
                string typeName = info.Type;    // Get type from info
                XElement parameters = info.Parameters;    // Get "Initialize" parameters

                file.SetFormat(FileManager.GetFormat(typeName), true);
                // TODO: file.Format.Initialize(file, parameters);
            }

            return file;
        }
    }
}

