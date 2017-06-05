//
// XmlExtension.cs
//
// Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
// Copyright (c) 2017 Benito Palacios Sánchez
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Libgame.FileFormat
{
    using System;
    using System.Text;
    using System.Xml.Linq;

    /// <summary>
    /// Extension methods for XML.
    /// </summary>
    public static class XmlExtension
    {
        /// <summary>
        /// Gets the default spaces per XML level.
        /// </summary>
        /// <value>The spaces per XML level.</value>
        public static int XmlSpacesPerLevel => 2;

        /// <summary>
        /// Gets the escape space.
        /// </summary>
        /// <value>The escape space.</value>
        public static string EscapeSpace => "{!SP}";

        /// <summary>
        /// Sets the concatenated text to this element indented for human-readibility.
        /// </summary>
        /// <param name="entry">XML entry to set text.</param>
        /// <param name="val">Value to indent and set.</param>
        /// <param name="indent">Indentation level.</param>
        public static void SetIndentedValue(this XElement entry, string val, int indent)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            if (val == null)
                throw new ArgumentNullException(nameof(val));

            StringBuilder text = new StringBuilder(val);

            // Escape 'invalid' spaces
            if (val.Contains("\n")) {
                ReplaceStartingSpaces(text);
                ReplaceTrailingSpaces(text);
                IndentNewLines(text, indent);
            }

            // Escape weird ASCII related-spaces chars
            text.Replace("\v", "[@!!0B]"); // Vertical tab

            entry.Value = text.ToString();
        }

        /// <summary>
        /// Gets the concatenated text to this element removing any indentation.
        /// </summary>
        /// <returns>The value without indentation.</returns>
        /// <param name="entry">XML entry to get text.</param>
        public static string GetIndentedValue(this XElement entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            StringBuilder str = new StringBuilder(entry.Value);

            // Remove new line indentation
            if (entry.Value.Contains("\n")) {
                RemoveStartingSpaces(str);
                RemoveTrailingSpaces(str);

                str.Replace("\n ", "\n");       // Remove spaces after
                str.Replace(" \n", "\n");       // and before new line
                if (str[0] == '\n')             // Remove first new line char
                    str.Remove(0, 1);
                if (str[str.Length - 1] == '\n') // Remove last new line char
                    str.Remove(str.Length - 1, 1);
            }

            str.Replace("{!SP}", " ");
            str.Replace("[@!!0B]", "\v");

            return str.ToString();
        }

        static void IndentNewLines(StringBuilder text, int indent)
        {
            string indentation = new string(' ', indent * XmlSpacesPerLevel);
            string indentationEnd = new string(' ', (indent - 1) * XmlSpacesPerLevel);

            text.Replace("\n", "\n" + indentation)
                .Insert(0, "\n" + indentation)
                .Insert(text.Length, "\n" + indentationEnd);
        }

        static void ReplaceStartingSpaces(StringBuilder str)
        {
            int idx = 0;
            do {
                // While we detect a space and not a letter, replace it.
                while (idx < str.Length && str[idx] == ' ') {
                    str.Remove(idx, 1);
                    str.Insert(idx, EscapeSpace);
                    idx += EscapeSpace.Length;
                }

                // Go to next line
                idx = str.ToString().IndexOf('\n', idx) + 1;
            } while (idx != 0);
        }

        static void ReplaceTrailingSpaces(StringBuilder str)
        {
            int idx = str.Length - 1;
            while (idx > 0) {
                // While we are on a space, replace it
                while (idx > 0 && str[idx] == ' ') {
                    str.Remove(idx, 1);
                    str.Insert(idx, EscapeSpace);
                    idx--;
                }

                // Go to previous line
                idx = str.ToString().LastIndexOf('\n', idx) - 1;
            }
        }

        static void RemoveStartingSpaces(StringBuilder str)
        {
            int newLine = 0;
            do {
                // Remove consecutive spaces
                while (newLine + 1 < str.Length && str[newLine] == ' ' && str[newLine + 1] == ' ')
                    str.Remove(newLine, 1);

                // Go to next line
                newLine = str.ToString().IndexOf('\n', newLine) + 1;
            } while (newLine != 0);
        }

        static void RemoveTrailingSpaces(StringBuilder str)
        {
            int newLine = str.Length - 1;
            while (newLine > 0) {
                // Remove consecutive spaces
                while (str[newLine] == ' ' && str[newLine - 1] == ' ')
                    str.Remove(newLine, 1);

                // Go to previous line
                newLine = str.ToString().LastIndexOf('\n', newLine) - 1;
            }
        }
    }
}
