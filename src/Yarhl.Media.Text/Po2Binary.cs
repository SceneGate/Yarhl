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
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Po to Binary converter.
    /// </summary>
    public class Po2Binary : IConverter<Po, BinaryFormat>
    {
        /// <summary>
        /// Convert the specified PO into a Binary stream.
        /// </summary>
        /// <param name="source">Source PO.</param>
        /// <returns>The converted stream.</returns>
        /// <remarks>
        /// It writes the header only if <see cref="PoHeader.ProjectIdVersion"/>
        /// is not empty.
        /// </remarks>
        public BinaryFormat Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat binary = new BinaryFormat();
            TextDataWriter writer = new TextDataWriter(binary.Stream);

            if (!string.IsNullOrEmpty(source.Header.ProjectIdVersion))
                WriteHeader(source.Header, writer);

            foreach (var entry in source.Entries) {
                writer.WriteLine();
                WriteEntry(entry, writer);
            }

            return binary;
        }

        static void WriteHeader(PoHeader header, TextDataWriter writer)
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

        static void WriteEntry(PoEntry entry, TextDataWriter writer)
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

        static void WriteIfNotEmpty(TextDataWriter writer, string format, string content)
        {
            if (!string.IsNullOrEmpty(content)) {
                var lines = content.Split('\n');
                foreach (string line in lines) {
                    writer.WriteLine(format, line);
                }
            }
        }

        static void WriteWrappedString(TextDataWriter writer, string content)
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
    }
}
