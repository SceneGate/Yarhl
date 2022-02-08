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

    /// <summary>
    /// Entry in PO formats. Represents a translation unit.
    /// </summary>
    public class PoEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PoEntry"/> class.
        /// </summary>
        public PoEntry()
        {
            Original = string.Empty;
            Translated = string.Empty;
            Context = string.Empty;
            TranslatorComment = string.Empty;
            ExtractedComments = string.Empty;
            Reference = string.Empty;
            Flags = string.Empty;
            PreviousContext = string.Empty;
            PreviousOriginal = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoEntry"/> class.
        /// </summary>
        /// <param name="original">Original text to translate.</param>
        public PoEntry(string original)
            : this()
        {
            Original = original;
            Translated = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoEntry"/> class.
        /// </summary>
        /// <param name="entry">The entry to copy.</param>
        public PoEntry(PoEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            Original = entry.Original;
            Translated = entry.Translated;
            Context = entry.Context;
            TranslatorComment = entry.TranslatorComment;
            ExtractedComments = entry.ExtractedComments;
            Reference = entry.Reference;
            Flags = entry.Flags;
            PreviousContext = entry.PreviousContext;
            PreviousOriginal = entry.PreviousOriginal;
        }

        /// <summary>
        /// Gets or sets the original content to translate.
        /// </summary>
        /// <remarks>
        /// <para>Entries with the same original content will be merged.</para>
        /// </remarks>
        /// <value>The original content.</value>
        public string Original { get; set; }

        /// <summary>
        /// Gets or sets the translated content.
        /// </summary>
        /// <value>The translated content.</value>
        public string Translated { get; set; }

        /// <summary>
        /// Gets the translated text if any, otherwise the original text.
        /// </summary>
        /// <returns>The final text.</returns>
        public string Text {
            get {
                if (string.IsNullOrEmpty(Translated))
                    return Original;
                return Translated;
            }
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <remarks>
        /// <para>It's possible to have multiple entries with the same
        /// <see cref="Original"/> content if the context is different.</para>
        /// </remarks>
        /// <value>The context.</value>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the translators' comments.
        /// </summary>
        /// <value>The translator comment.</value>
        public string TranslatorComment { get; set; }

        /// <summary>
        /// Gets or sets the programmers' comments.
        /// </summary>
        /// <value>The extracted comments.</value>
        public string ExtractedComments { get; set; }

        /// <summary>
        /// Gets or sets the comments with reference to the origin of the content.
        /// </summary>
        /// <value>The reference.</value>
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the flags.
        /// </summary>
        /// <value>The flags.</value>
        public string Flags { get; set; }

        /// <summary>
        /// Gets or sets the previous context.
        /// </summary>
        /// <value>The previous context.</value>
        public string PreviousContext { get; set; }

        /// <summary>
        /// Gets or sets the previous original content.
        /// </summary>
        /// <value>The previous original content.</value>
        public string PreviousOriginal { get; set; }
    }
}
