// (c) Copyright, Real-Time Innovations, 2017.
// All rights reserved.
//
// No duplications, whole or partial, manual or electronic, may be made
// without express written permission.  Any such copies, or
// revisions thereof, must display this notice unaltered.
// This code contains trade secrets of Real-Time Innovations, Inc.
// namespace RTI.Connector
namespace Libgame.FileFormat.Common
{
    using System;
    using System.Globalization;
    using Libgame.IO;
    using Mono.Addins;

    [Extension]
    public class Po2Binary : IConverter<Po, BinaryFormat>
    {
        public BinaryFormat Convert(Po source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            BinaryFormat binary = new BinaryFormat();
            DataWriter writer = new DataWriter(binary.Stream);

            if (source.Header != null)
                WriteHeader(source.Header, writer);

            foreach (var entry in source.Entries) {
                writer.Write("\n", false);
                WriteEntry(entry, writer);
            }

            return binary;
        }

        static void WriteHeader(PoHeader header, DataWriter writer)
        {
            writer.Write("msgid \"\"\n", false);
            writer.Write("msgstr \"\"\n", false);
            WriteIfNotEmpty(writer, "\"Project-Id-Version: {0}\\n\"\n", header.ProjectIdVersion);
            WriteIfNotEmpty(writer, "\"Report-Msgid-Bugs-To: {0}\\n\"\n", header.ReportMsgidBugsTo);
            WriteIfNotEmpty(writer, "\"POT-Creation-Date: {0}\\n\"\n", header.CreationDate);
            WriteIfNotEmpty(writer, "\"POT-Revision-Date: {0}\\n\"\n", header.RevisionDate);
            WriteIfNotEmpty(writer, "\"Last-Translator: {0}\\n\"\n", header.LastTranslator);
            WriteIfNotEmpty(writer, "\"Language-Team: {0}\\n\"\n", header.LanguageTeam);
            WriteIfNotEmpty(writer, "\"Language: {0}\\n\"\n", header.Language);
            WriteIfNotEmpty(writer, "\"Content-Type: {0}\\n\"\n", header.ContentType);
            WriteIfNotEmpty(writer, "\"Content-Transfer-Encoding: {0}\\n\"\n", header.ContentTransferEncoding);
            WriteIfNotEmpty(writer, "\"Plural-Forms: {0}\\n\"\n", header.PluralForms);
        }

        static void WriteEntry(PoEntry entry, DataWriter writer)
        {
            WriteIfNotEmpty(writer, "#  {0}\n", entry.TranslatorComment);
            WriteIfNotEmpty(writer, "#. {0}\n", entry.ExtractedComments);
            WriteIfNotEmpty(writer, "#: {0}\n", entry.Reference);
            WriteIfNotEmpty(writer, "#, {0}\n", entry.Flags);
            WriteIfNotEmpty(writer, "#| msgctxt {0}\n", entry.PreviousContext);
            WriteIfNotEmpty(writer, "#| msgid {0}\n", entry.PreviousOriginal);

            if (!string.IsNullOrEmpty(entry.Context)) {
                writer.Write("msgctxt ", false);
                WriteWrappedString(writer, entry.Context);
            }

            writer.Write("msgid ", false);
            WriteWrappedString(writer, entry.Original);

            writer.Write("msgstr ", false);
            WriteWrappedString(writer, entry.Translated);
        }

        static void WriteIfNotEmpty(DataWriter writer, string format, string content)
        {
            if (!string.IsNullOrEmpty(content))
                writer.Write(string.Format(CultureInfo.InvariantCulture, format, content), false);
        }

        static void WriteWrappedString(DataWriter writer, string content)
        {
            int idx = 0;
            content = content.Replace("\n", "\\n");
            content = content.Replace("\"", "\\\"");

            if (content.Contains("\\n"))
                writer.Write("\"\"\n", false);

            do {
                int nextIdx = content.IndexOf("\\n", idx, StringComparison.InvariantCulture);
                int end = nextIdx != -1 ? nextIdx + 2 : content.Length;
                writer.Write("\"" + content.Substring(idx, end - idx) + "\"\n", false);

                idx = nextIdx + 2;
            } while (idx != 1);
        }
    }
}
