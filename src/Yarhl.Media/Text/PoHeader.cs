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
    using System.Collections.Generic;

    /// <summary>
    /// Header for PO translation format.
    /// </summary>
    public class PoHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PoHeader"/> class.
        /// </summary>
        public PoHeader()
        {
            ProjectIdVersion = string.Empty;
            ReportMsgidBugsTo = string.Empty;
            Language = string.Empty;
            CreationDate = DateTime.Now.ToShortDateString();
            RevisionDate = string.Empty;
            LastTranslator = string.Empty;
            LanguageTeam = string.Empty;
            PluralForms = string.Empty;
            Extensions = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoHeader"/> class.
        /// </summary>
        /// <param name="id">Identifier of the project.</param>
        /// <param name="reporter">Address to report bugs.</param>
        /// <param name="lang">Translation language.</param>
        public PoHeader(string id, string reporter, string lang)
            : this()
        {
            ProjectIdVersion = id;
            ReportMsgidBugsTo = reporter;
            Language = lang;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoHeader"/> class.
        /// </summary>
        /// <param name="header">The header to copy.</param>
        public PoHeader(PoHeader header)
            : this()
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));

            ProjectIdVersion = header.ProjectIdVersion;
            ReportMsgidBugsTo = header.ReportMsgidBugsTo;
            Language = header.Language;
            CreationDate = header.CreationDate;
            RevisionDate = header.RevisionDate;
            LastTranslator = header.LastTranslator;
            LanguageTeam = header.LanguageTeam;
            PluralForms = header.PluralForms;
            foreach (KeyValuePair<string, string> extension in header.Extensions)
            {
                Extensions[extension.Key] = extension.Value;
            }
        }

        /// <summary>
        /// Gets the type of the content and encoding.
        /// </summary>
        /// <value>The type of the content.</value>
        public static string ContentType => "text/plain; charset=UTF-8";

        /// <summary>
        /// Gets the content transfer encoding.
        /// </summary>
        /// <value>The content transfer encoding.</value>
        public static string ContentTransferEncoding => "8bit";

        /// <summary>
        /// Gets the version of MIME.
        /// </summary>
        /// <value>The version of MIME.</value>
        public static string MimeVersion => "1.0";

        /// <summary>
        /// Gets or sets the project identifier version.
        /// </summary>
        /// <value>The project identifier version.</value>
        public string ProjectIdVersion { get; set; }

        /// <summary>
        /// Gets or sets the address and name to report bugs in the string format.
        /// </summary>
        /// <value>The address to report bugs to.</value>
        public string ReportMsgidBugsTo { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>The creation date.</value>
        public string CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the revision date.
        /// </summary>
        /// <value>The revision date.</value>
        public string RevisionDate { get; set; }

        /// <summary>
        /// Gets or sets the last translator name.
        /// </summary>
        /// <value>The last translator name.</value>
        public string LastTranslator { get; set; }

        /// <summary>
        /// Gets or sets the team translation name.
        /// </summary>
        /// <value>The team name.</value>
        public string LanguageTeam { get; set; }

        /// <summary>
        /// Gets or sets the target language.
        /// </summary>
        /// <value>The target language.</value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the plural forms.
        /// </summary>
        /// <value>The plural forms.</value>
        public string PluralForms { get; set; }

        /// <summary>
        /// Gets a dictionary with custom header metadata.
        /// </summary>
        /// <value>The dictionary for the metadata.</value>
        public IDictionary<string, string> Extensions { get; private set; }
    }
}
