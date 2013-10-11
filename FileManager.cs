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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Addins;

namespace Libgame
{
	public class FileManager
	{
		private static FileManager Instance;
		private XDocument xmlGame;

		private FileManager(FileContainer root, XDocument xmlGame)
		{
			this.Root = root;
			this.xmlGame = xmlGame;
		}

		public FileContainer Root {
			get;
			private set;
		}

		public static FileManager GetInstance()
		 {
			if (Instance == null)
				throw new Exception("The class has not been initalized.");
			return Instance;
		}

		public static void Initialize(FileContainer rootDir, XDocument xmlGame)
		{
			Instance = new FileManager(rootDir, xmlGame);
		}

		public Format GetFormat(string name)
		{
			return AddinManager.GetExtensionObjects<Format>(false).
				Where(f => f.FormatName == name).
			    ToArray()[0];
		}

		public GameFile RescueFile(string gameFilePath)
		{
			XElement fileInfo = this.GetFileInfo(gameFilePath);
			if (fileInfo != null)
				return this.RescueFileInfo(gameFilePath, fileInfo);
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
					dependency.Format.Read();
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
					return null;	// Folder without dependencies
			} else if (file == null) {
				throw new Exception("File not found.");
			}

			// 1.2.- Gets dependencies to be able to parse data.
			// It will try to guess the file type using FormatValidation classes.
			// If one of the matches, it will provide the dependencies.
			foreach (FormatValidation validation in AddinManager.GetExtensionObjects<FormatValidation>(false)) {
				validation.AutosetFormat = true;	// If it matches set format to the file.
				validation.RunTests(file);

				if (validation.Result) {
					foreach (string dependencyPath in validation.Dependencies) {
						GameFile dependency = this.RescueFile(dependencyPath);
						depends.Add(dependency);
						dependency.Format.Read();
					}
					break;
				}
			}

			// Set dependencies
			file.AddDependencies(depends.ToArray());

			return file;
		}

		private GameFile RescueFileInfo(string gameFilePath, XElement fileInfo)
		{
			// Resolve dependencies
			List<GameFile> depends = new List<GameFile>();

			foreach (XElement xmlDepend in fileInfo.Elements("DependsOn")) {
				GameFile dependency = this.RescueFile(xmlDepend.Value);
				depends.Add(dependency);
				dependency.Format.Read();
			}

			// Get file
			FileContainer searchFile = this.Root.SearchFile(gameFilePath);
			GameFile file =  searchFile as GameFile;
			if (file == null) {
				throw new Exception("File not found.");
			}

			// Add dependencies
			file.AddDependencies(depends.ToArray());


			if (file.Format == null) {
				string typeName = fileInfo.Element("Type").Value;		// Get type from info
				XElement parameters = fileInfo.Element("Parameters");	// Get "Initialize" parameters

				file.Format = this.GetFormat(typeName);
				file.Format.Initialize(file, parameters);
			}

			return file;
		}

		private XElement GetFileInfo(string path)
		{
			XElement files = xmlGame.Root.Element("Files");
			foreach (XElement fileInfo in files.Elements("FileInfo")) {
				if (fileInfo.Element("Path").Value == path)
					return fileInfo;
			}

			return null;
		}
	}
}

