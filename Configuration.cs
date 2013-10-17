//-----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="none">
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
// <date>12/06/2013</date>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Libgame
{
	public class Configuration
	{
		private static Configuration Instance;

		private XDocument xmlEdit;
		private Dictionary<string, string> relativePaths;
		private Dictionary<string, char[,]> tables;
		private string ellipsis;
		private char[] quotes;
		private char[] furigana;

		private string osName;

		private Configuration(XDocument xmlEdit)
		{
			this.osName = Environment.OSVersion.Platform.ToString();

			this.xmlEdit = xmlEdit;
			this.ReadConfig();
		}

		public XElement XEdit {
			get { return this.xmlEdit.Root; }
		}

		public ReadOnlyDictionary<string, char[,]> Tables {
			get { return new ReadOnlyDictionary<string, char[,]>(this.tables); }
		}

		public string Ellipsis {
			get { return this.ellipsis; }
		}

		public char[] QuoteMarks {
			get { return this.quotes; }
		}

		public char[] FuriganaMarks {
			get { return this.furigana; }
		}

		public static Configuration GetInstance()
		{
			if (Instance == null)
				throw new Exception("The class has not been initialized.");
			return Instance;
		}

		public static void Initialize(XDocument xmlEdit)
		{
			Instance = new Configuration(xmlEdit);
		}

		public string ResolvePath(string path)
		{
			if (!path.Contains("{$") || string.IsNullOrEmpty(path))
				return path;

			int pos = path.IndexOf('{');
			while (pos != -1) {
				// Get variable
				string variable = path.Substring(pos + 2, path.IndexOf('}', pos) - (pos + 2));
				path = path.Replace("{$" + variable + "}", this.relativePaths[variable]);

				pos = path.IndexOf('{', pos);
			}

			if (this.osName != "Unix") {
				if (path.StartsWith("./"))
					path = Path.Combine(Environment.CurrentDirectory, path.Substring(2));

				path = path.Replace('/', '\\');
			}

			return path;
		}

		private void ReadConfig()
		{
			XElement root = this.xmlEdit.Root;

			// Get relative paths
			this.relativePaths = new Dictionary<string, string>();
			foreach (XElement xrel in root.Element("RelativePaths").Elements("Path")) {
				string variable = xrel.Element("Variable").Value;
				string path = xrel.Element("Location").Value;

				path = ResolvePath(path);
				this.relativePaths.Add(variable, path);
			}

			// FUTURE: What about adding a "Encoding" field?

			// Get tables
			this.tables = new Dictionary<string, char[,]>();
			foreach (XElement xtbl in root.Element("CharTables").Elements("Table")) {
				string name = xtbl.Attribute("name").Value;
				char[,] table = new char[xtbl.Elements().Count(), 2];	// Chars to replace: original <-> new

				int i = 0;
				foreach (XElement entry in xtbl.Elements()) {
					// FUTURE: Entries by unicode number instead of char
					if (entry.Name == "Char") {
						table[i, 0] = entry.Attribute("original").Value[0];
						table[i, 1] = entry.Attribute("new").Value[0];
					}

					i++;
				}

				this.tables[name] = table;
			}

			// Get special chars
			XElement xSpecialChars = root.Element("SpecialChars");
			this.ellipsis = xSpecialChars.Element("Ellipsis").Value;
			this.quotes = new char[2] { 
				xSpecialChars.Element("QuoteOpen").Value[0],
				xSpecialChars.Element("QuoteClose").Value[0]
			 };
			this.furigana = new char[2] {
				xSpecialChars.Element("FuriganaOpen").Value[0],
				xSpecialChars.Element("FuriganaClose").Value[0]
			};
		}
	}
}

