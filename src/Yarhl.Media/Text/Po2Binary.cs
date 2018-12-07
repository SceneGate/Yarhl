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
namespace Yarhl.Media.Text
{
    using System;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Po to Binary converter.
    /// </summary>
    public class Po2Binary :
        IConverter<Po, BinaryFormat>,
        IConverter<BinaryFormat, Po>
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
                po.Add(entry);
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
            writer.WriteLine(@"""PO-Revision-Date: {0}\n""", header.RevisionDate ?? string.Empty);
            writer.WriteLine(@"""Last-Translator: {0}\n""", header.LastTranslator ?? string.Empty);
            writer.WriteLine(@"""Language-Team: {0}\n""", header.LanguageTeam ?? string.Empty);
            writer.WriteLine(@"""Language: {0}\n""", header.Language ?? string.Empty);
            writer.WriteLine(@"""MIME-Version: {0}\n""", PoHeader.MimeVersion);
            writer.WriteLine(@"""Content-Type: {0}\n""", PoHeader.ContentType);
            writer.WriteLine(@"""Content-Transfer-Encoding: {0}\n""", PoHeader.ContentTransferEncoding);
            WriteIfNotEmpty(writer, @"""Plural-Forms: {0}\n""", header.PluralForms);

            foreach (var entry in header.Extensions)
                writer.WriteLine(@"""X-{0}: {1}\n""", entry.Key, entry.Value);
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
                int nextIdx = content.IndexOf("\\n", idx, StringComparison.Ordinal);
                int end = nextIdx != -1 ? nextIdx + 2 : content.Length;
                writer.WriteLine("\"{0}\"", content.Substring(idx, end - idx));

                idx = nextIdx + 2;
            } while (idx != 1);
        }

        static PoEntry ReadEntry(TextReader reader)
        {
            // Skip all the blank lines before the block of text
            string line = string.Empty;
            while (reader.PeekLine()?.Trim().Length == 0)
                reader.ReadLine();

            // If nothing to read, EOF
            if (reader.Stream.EndOfStream)
                return null;

            PoEntry entry = new PoEntry();
            while (!reader.Stream.EndOfStream) {
                // Get the next line
                line = reader.ReadLine();

                // If it's blank, then this block finished
                if (string.IsNullOrWhiteSpace(line))
                    break;

                ParseLine(reader, entry, line);
            }

            return entry;
        }

        static void ParseLine(TextReader reader, PoEntry entry, string line)
        {
            string[] fields = line.Split(new[] { ' ' }, 2);
            if (fields.Length != 2)
                throw new FormatException("Invalid line format: " + line);

            switch (fields[0]) {
                case "#":
                    entry.TranslatorComment = fields[1].TrimStart();
                    break;
                case "#.":
                    entry.ExtractedComments = fields[1];
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

        static string ReadMultiLineContent(TextReader reader, string currentLine)
        {
            StringBuilder content = new StringBuilder(ParseMultiLine(currentLine));

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
