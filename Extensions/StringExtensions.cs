// -----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="none">
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
// <author>pleonex</author>
// <email>benito356@gmail.com</email>
// <date>18/09/2013</date>
// -----------------------------------------------------------------------
using System;
using System.Text;

namespace Libgame
{
	public static class StringExtensions
	{
		/// <summary>
		/// Gets the number of spaces per level of XML node.
		/// </summary>
		public static int XmlSpacesPerLevel {
			get { return 2; }
		}

		public static string GetPreviousPath(this string path)
		{
			return path.Substring(0, path.LastIndexOf(FileContainer.PathSeparator));
		}

		public static string ApplyTable(this string s, string tableName, bool originalToNew)
		{
			StringBuilder newString = new StringBuilder();

			foreach (char ch in s)
				newString.Append(ApplyTable(ch, tableName, originalToNew));

			return newString.ToString();
		}

		public static string ApplySpecialChars(this string s)
		{
			Configuration config = Configuration.GetInstance();
			StringBuilder str = new StringBuilder(s);

			// Ellipsis
			str.Replace("...",    config.Ellipsis);
			str.Replace("\x8163", config.Ellipsis);

			// Quotation marks
			bool startQuote = false;          
			for (int i = 0; i < str.Length; i++) {
				if (str[i] == '\"') {
					str[i] = !startQuote ? config.QuoteMarks[0] : config.QuoteMarks[1];
					startQuote = !startQuote;
				}
			}

			if (startQuote) {
				// TODO: It should give a warning instead of an error.
				//throw new System.IO.EndOfStreamException("No ending quote found.");
			}

			return str.ToString();
		}

		public static char ApplyTable(this char ch, string tableName, bool originalToNew)
		{
			Configuration config = Configuration.GetInstance();
			if (!config.Tables.ContainsKey(tableName))
				throw new ArgumentException("The table does not exist.");

			char[,] table = config.Tables[tableName];
			for (int i = 0; i < table.GetLength(0); i++) {
				if (originalToNew && table[i, 0] == ch)
					return table[i, 1];
				else if (!originalToNew && table[i, 1] == ch)
					return table[i, 0];
			}

			return ch;
		}

		/// <summary>
		/// Formats a string to make it more XML read-able (to XML).
		/// </summary>
		/// <param name="text">String to format.</param>
		/// <param name="level">Number of XML node levels.</param>
		/// <param name="furiStart">Char that indicates the start of furigana.</param>
		/// <param name="furiEnd">Char that indicates the end of furigana.</param>
		/// <returns>String formatted.</returns>
		public static string ToXmlString(this string s, int level, char furiOpen, char furiClose)
		{
			Configuration config = Configuration.GetInstance();
			string indentation = new string(' ', level * XmlSpacesPerLevel);
			string indentationEnd = new string(' ', (level - 1) * XmlSpacesPerLevel);

			s = s.ApplySpecialChars();
			StringBuilder str = new StringBuilder(s);

			// Furigana marks
			str.Replace(furiOpen,  config.FuriganaMarks[0]);
			str.Replace(furiClose, config.FuriganaMarks[1]);

			// Add indentation to view it better
			str.Replace("\n", "\n" + indentation);
			if (s.Contains("\n")) {
				str.Insert(0, "\n" + indentation);
				str.Append("\n" + indentationEnd);	// For close tag
			}

			return str.ToString();
		}

		/// <summary>
		/// Gets the original formatted string (from XML).
		/// </summary>
		/// <param name="text">Formatted string.</param>
		/// <param name="level">Number of XML node levels.</param>
		/// <param name="furiStart">Char that indicates the start of furigana.</param>
		/// <param name="furiEnd">Char that indicates the end of furigana.</param>
		/// <returns>Original string.</returns>
		public static string FromXmlString(this string s, int level, char furiOpen, char furiClose)
		{
			Configuration config = Configuration.GetInstance();
			int indenSize = level * XmlSpacesPerLevel;
			string indentation = new string(' ', indenSize);

			s = s.ApplySpecialChars();
			StringBuilder str = new StringBuilder(s);

			// Furigana marks
			str.Replace(config.FuriganaMarks[0], furiOpen);
			str.Replace(config.FuriganaMarks[1], furiClose);

			// Remove indentation
			if (s.Contains("\n")) {
				// Remove: '\n' + indentation at the start
				str.Remove(0, 1 + indenSize);     

				// Remove: '\n' + (indentation of one less level)  at the end
				int startIndex = str.Length - (1 + indenSize - XmlSpacesPerLevel);
				str.Remove(startIndex, str.Length - startIndex);

				str.Replace("\n" + indentation, "\n");
			}

			return str.ToString();
		}
	}
}

