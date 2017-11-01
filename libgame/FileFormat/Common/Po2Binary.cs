//
// Po2Binary.cs
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
namespace Libgame.FileFormat.Common
{
    using System;
    using System.Linq;
    using System.Text;
    using Libgame.IO;
    using Mono.Addins;

    /// <summary>
    /// Po to Binary converter.
    /// </summary>
    [Extension]
    public class Po2Binary : IConverter<Po, BinaryFormat>, IConverter<BinaryFormat, Po>
    {
        /// <summary>
        /// Convert the specified PO into a Binary stream.
        /// </summary>
        /// <returns>The converted stream.</returns>
        /// <param name="source">Source PO.</param>
        public BinaryFormat Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat binary = new BinaryFormat();
            TextWriter writer = new TextWriter(binary.Stream);

            if (source.Header != null)
                WriteHeader(source.Header, writer);

            foreach (var entry in source.Entries) {
                writer.WriteLine();
                WriteEntry(entry, writer);
            }

            return binary;
        }

        /// <summary>
        /// Convert the specified Binary stream into a PO object.
        /// </summary>
        /// <returns>The converted PO object.</returns>
        /// <param name="source">Source binary stream.</param>
        public Po Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            TextReader reader = new TextReader(source.Stream);
            Po po = new Po();

            // Read the header if any
            PoEntry entry = ReadEntry(reader);
            if (entry == null)
                return po;

            if (entry.Original.Length == 0)
                po.Header = Entry2Header(entry);
            else
                po.Add(entry);

            // Read other entries
            while ((entry = ReadEntry(reader)) != null) {
                if (entry.Original.Length != 0)
                    po.Add(entry);
                else
                    throw new FormatException("Original field must be filled");
            }

            return po;
        }

        static void WriteHeader(PoHeader header, TextWriter writer)
        {
            writer.WriteLine(@"msgid """"");
            writer.WriteLine(@"msgstr """"");
            writer.WriteLine(@"""Project-Id-Version: {0}\n""", header.ProjectIdVersion ?? string.Empty);
            writer.WriteLine(@"""Report-Msgid-Bugs-To: {0}\n""", header.ReportMsgidBugsTo ?? string.Empty);
            writer.WriteLine(@"""POT-Creation-Date: {0}\n""", header.CreationDate ?? string.Empty);
            writer.WriteLine(@"""POT-Revision-Date: {0}\n""", header.RevisionDate ?? string.Empty);
            writer.WriteLine(@"""Last-Translator: {0}\n""", header.LastTranslator ?? string.Empty);
            writer.WriteLine(@"""Language-Team: {0}\n""", header.LanguageTeam ?? string.Empty);
            writer.WriteLine(@"""Language: {0}\n""", header.Language ?? string.Empty);
            writer.WriteLine(@"""Content-Type: {0}\n""", header.ContentType);
            writer.WriteLine(@"""Content-Transfer-Encoding: {0}\n""", header.ContentTransferEncoding);
            WriteIfNotEmpty(writer, @"""Plural-Forms: {0}\n""", header.PluralForms);
        }

        static void WriteEntry(PoEntry entry, TextWriter writer)
        {
            WriteIfNotEmpty(writer, "#  {0}", entry.TranslatorComment);
            WriteIfNotEmpty(writer, "#. {0}", entry.ExtractedComments);
            WriteIfNotEmpty(writer, "#: {0}", entry.Reference);
            WriteIfNotEmpty(writer, "#, {0}", entry.Flags);
            WriteIfNotEmpty(writer, "#| msgctxt {0}", entry.PreviousContext);
            WriteIfNotEmpty(writer, "#| msgid {0}", entry.PreviousOriginal);

            if (!string.IsNullOrEmpty(entry.Context)) {
                writer.Write("msgctxt ");
                WriteWrappedString(writer, entry.Context);
            }

            writer.Write("msgid ");
            WriteWrappedString(writer, entry.Original);

            writer.Write("msgstr ");
            WriteWrappedString(writer, entry.Translated);
        }

        static void WriteIfNotEmpty(TextWriter writer, string format, string content)
        {
            if (!string.IsNullOrEmpty(content))
                writer.WriteLine(format, content);
        }

        static void WriteWrappedString(TextWriter writer, string content)
        {
            int idx = 0;
            content = content.Replace("\n", "\\n");
            content = content.Replace("\"", "\\\"");

            if (content.Contains("\\n"))
                writer.WriteLine("\"\"");

            do {
                int nextIdx = content.IndexOf("\\n", idx, StringComparison.InvariantCulture);
                int end = nextIdx != -1 ? nextIdx + 2 : content.Length;
                writer.WriteLine("\"{0}\"", content.Substring(idx, end - idx));

                idx = nextIdx + 2;
            } while (idx != 1);
        }

        static PoEntry ReadEntry(TextReader reader)
        {
            // Skip all the blank lines before the block of text
            string line = string.Empty;
            while (!reader.Stream.EndOfStream && string.IsNullOrWhiteSpace(reader.PeekLine()))
                reader.ReadLine();

            // If nothing to read, EOF
            if (reader.Stream.EndOfStream)
                return null;

            PoEntry entry = new PoEntry();
            StringComparison comparison = StringComparison.Ordinal;
            while (!reader.Stream.EndOfStream) {
                // Get the next line
                line = reader.ReadLine();

                // If it's blank, then this block finished
                if (string.IsNullOrWhiteSpace(line))
                    break;

                switch (line.Substring(0, 3)) {
                case "#  ":
                    entry.TranslatorComment = line.Substring(3);
                    break;

                case "#. ":
                    entry.ExtractedComments = line.Substring(3);
                    break;

                case "#: ":
                    entry.Reference = line.Substring(3);
                    break;

                case "#, ":
                    entry.Flags = line.Substring(3);
                    break;

                case "#| ":
                    if (line.StartsWith("#| msgctxt ", comparison))
                        entry.PreviousContext = line.Substring("#| msgctxt ".Length);
                    else if (line.StartsWith("#| msgid ", comparison))
                        entry.PreviousOriginal = line.Substring("#| msgid ".Length);
                    else
                        throw new FormatException("Unknown line '" + line + "'");
                    break;

                case "msg":
                    if (line.StartsWith("msgctxt ", comparison))
                        entry.Context = ReadMultiLineContent(reader, line);
                    else if (line.StartsWith("msgid ", comparison))
                        entry.Original = ReadMultiLineContent(reader, line);
                    else if (line.StartsWith("msgstr ", comparison))
                        entry.Translated = ReadMultiLineContent(reader, line);
                    else
                        throw new FormatException("Unknown line '" + line + "'");
                    break;

                default:
                    throw new FormatException("Unknown line '" + line + "'");
                }
            }

            return entry;
        }

        static PoHeader Entry2Header(PoEntry entry)
        {
            StringComparison comparison = StringComparison.Ordinal;
            PoHeader header = new PoHeader();
            foreach (string line in entry.Translated.Split('\n')) {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("Project-Id-Version: ", comparison))
                    header.ProjectIdVersion = line.Substring("Project-Id-Version: ".Length);
                else if (line.StartsWith("Report-Msgid-Bugs-To: ", comparison))
                    header.ReportMsgidBugsTo = line.Substring("Report-Msgid-Bugs-To: ".Length);
                else if (line.StartsWith("POT-Creation-Date: ", comparison))
                    header.CreationDate = line.Substring("POT-Creation-Date: ".Length);
                else if (line.StartsWith("POT-Revision-Date: ", comparison))
                    header.RevisionDate = line.Substring("POT-Revision-Date: ".Length);
                else if (line.StartsWith("Last-Translator: ", comparison))
                    header.LastTranslator = line.Substring("Last-Translator: ".Length);
                else if (line.StartsWith("Language-Team: ", comparison))
                    header.LanguageTeam = line.Substring("Language-Team: ".Length);
                else if (line.StartsWith("Language: ", comparison))
                    header.Language = line.Substring("Language: ".Length);
                else if (line.StartsWith("Plural-Forms: ", comparison))
                    header.PluralForms = line.Substring("Plural-Forms: ".Length);
                else if (line.StartsWith("Content-Type: ", comparison)) {
                    if (line != "Content-Type: text/plain; charset=UTF-8")
                        throw new FormatException("Invalid Content-Type");
                } else if (line.StartsWith("Content-Transfer-Encoding: ", comparison)) {
                    if (line != "Content-Transfer-Encoding: 8bit")
                        throw new FormatException("Invalid Content-Transfer-Encoding");
                } else
                    throw new FormatException("Unknown header: " + line);
            }

            return header;
        }

        static string ReadMultiLineContent(TextReader reader, string currentLine)
        {
            // Remove the line ID (msgid, msgstr, msgctxt) and quotes
            currentLine = currentLine.Substring(currentLine.IndexOf(' ') + 1);
            currentLine = ParseMultiLine(currentLine);

            StringBuilder content = new StringBuilder(currentLine);
            while (!reader.Stream.EndOfStream && reader.Peek() == '"')
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
