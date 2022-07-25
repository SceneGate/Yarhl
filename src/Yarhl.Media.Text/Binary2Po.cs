// Copyright (c) 2019 SceneGate

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
namespace Yarhl.Media.Text
{
    using System;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Binary to Po converter.
    /// </summary>
    public class Binary2Po : IConverter<IBinary, Po>
    {
        /// <summary>
        /// Convert the specified Binary stream into a PO object.
        /// </summary>
        /// <returns>The converted PO object.</returns>
        /// <param name="source">Source binary stream.</param>
        public Po Convert(IBinary source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Stream.Position = 0;

            TextDataReader reader = new TextDataReader(source.Stream);
            Po po = new Po();

            // Read the header if any
            PoEntry? entry = ReadEntry(reader);
            if (entry == null)
                return po;

            if (entry.Original.Length == 0)
                po.Header = Entry2Header(entry);
            else
                po.Add(entry);

            // Read other entries
            while ((entry = ReadEntry(reader)) != null) {
                po.Add(entry);
            }

            return po;
        }

        static PoEntry? ReadEntry(TextDataReader reader)
        {
            // Skip all the blank lines before the block of text
            string line = string.Empty;
            while (!reader.Stream.EndOfStream && reader.PeekLine().Trim().Length == 0)
                reader.ReadLine();

            // If nothing to read, EOF
            if (reader.Stream.EndOfStream)
                return null;

            PoEntry entry = new PoEntry();
            while (reader.Stream.Position < reader.Stream.Length) {
                // Get the next line
                line = reader.ReadLine();

                // If it's blank, then this block finished
                if (string.IsNullOrWhiteSpace(line))
                    break;

                ParseLine(reader, entry, line);
            }

            return entry;
        }

        static void ParseLine(TextDataReader reader, PoEntry entry, string line)
        {
            string[] fields = line.Split(new[] { ' ' }, 2);
            if (fields.Length != 2)
                throw new FormatException("Invalid line format: " + line);

            switch (fields[0]) {
                case "#":
                    entry.TranslatorComment = ReadMultiLineComment(
                        reader,
                        fields[1].TrimStart(),
                        "# ");
                    break;
                case "#.":
                    entry.ExtractedComments = ReadMultiLineComment(
                        reader,
                        fields[1],
                        "#.");
                    break;
                case "#:":
                    entry.Reference = fields[1];
                    break;
                case "#,":
                    entry.Flags = fields[1];
                    break;

                case "#|":
                    string[] subfields = fields[1].Split(new[] { ' ' }, 2);
                    if (subfields[0] == "msgctxt")
                        entry.PreviousContext = subfields[1];
                    else if (subfields[0] == "msgid")
                        entry.PreviousOriginal = subfields[1];
                    else
                        throw new FormatException("Unknown previous field: " + line);
                    break;

                case "msgctxt":
                    entry.Context = ReadMultiLineContent(reader, fields[1]);
                    break;
                case "msgid":
                    entry.Original = ReadMultiLineContent(reader, fields[1]);
                    break;
                case "msgstr":
                    entry.Translated = ReadMultiLineContent(reader, fields[1]);
                    break;
                default:
                    throw new FormatException("Unknown line '" + line + "'");
            }
        }

        static PoHeader Entry2Header(PoEntry entry)
        {
            PoHeader header = new PoHeader();
            var option = StringSplitOptions.RemoveEmptyEntries;
            foreach (string line in entry.Translated.Split(new[] { '\n' }, option)) {
                var fields = line.Split(new[] { ": " }, 2, StringSplitOptions.None);
                if (fields.Length != 2)
                    throw new FormatException("Invalid format line: " + line);

                ParseHeaderLine(header, fields[0], fields[1]);
            }

            return header;
        }

        static void ParseHeaderLine(PoHeader header, string key, string value)
        {
            switch (key) {
                case "Project-Id-Version":
                    header.ProjectIdVersion = value;
                    break;
                case "Report-Msgid-Bugs-To":
                    header.ReportMsgidBugsTo = value;
                    break;

                case "POT-Creation-Date":
                    header.CreationDate = value;
                    break;
                case "PO-Revision-Date":
                    header.RevisionDate = value;
                    break;

                case "Last-Translator":
                    header.LastTranslator = value;
                    break;
                case "Language-Team":
                    header.LanguageTeam = value;
                    break;
                case "Language":
                    header.Language = value;
                    break;

                case "Plural-Forms":
                    header.PluralForms = value;
                    break;

                case "MIME-Version":
                    if (value != "1.0")
                        throw new FormatException("Invalid MIME version");
                    break;

                case "Content-Type":
                    if (value != "text/plain; charset=UTF-8")
                        throw new FormatException("Invalid Content-Type");
                    break;
                case "Content-Transfer-Encoding":
                    if (value != "8bit")
                        throw new FormatException("Invalid Content-Transfer-Encoding");
                    break;

                default:
                    // Ignore extended / tool-specific fields
                    if (key.Length > 2 && key.Substring(0, 2) == "X-") {
                        header.Extensions[key.Substring(2)] = value;
                        break;
                    }

                    throw new FormatException("Unknown header key: " + key);
            }
        }

        static string ReadMultiLineComment(TextDataReader reader, string line, string comment)
        {
            StringBuilder builder = new StringBuilder(line + "\n");
            while (reader.PeekToToken(" ") == comment) {
                // We just remove the comment token and take advantage that
                // there is an space after it.
                builder.Append(reader.ReadLine().Substring(comment.Length) + "\n");
            }

            var result = builder.ToString();

            // Delete the last newline and fix white space from newline.
            return result.Remove(result.Length - 1, 1).Replace("\n ", "\n");
        }

        static string ReadMultiLineContent(TextDataReader reader, string currentLine)
        {
            StringBuilder content = new StringBuilder(ParseMultiLine(currentLine));

            while ((reader.Stream.Position < reader.Stream.Length) && reader.Peek() == '"')
                content.Append(ParseMultiLine(reader.ReadLine()));

            return content.ToString();
        }

        static string ParseMultiLine(string line)
        {
            if (line.Length < 2)
                throw new FormatException("Invalid line quotes");

            if (line[0] != '"' || line[line.Length - 1] != '"')
                throw new FormatException("Line quotes in invalid position");

            line = line.Substring(1, line.Length - 2);
            return line.Replace("\\n", "\n").Replace("\\\"", "\"");
        }
    }
}
