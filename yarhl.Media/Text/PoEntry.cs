//
// PoEntry.cs
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
namespace Yarhl.Media
{
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
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoEntry"/> class.
        /// </summary>
        /// <param name="original">Original text to translate.</param>
        public PoEntry(string original)
        {
            Original = original;
            Translated = string.Empty;
        }

        /// <summary>
        /// Gets or sets the original content to translate.
        /// </summary>
        /// <value>The original content.</value>
        public string Original { get; set; }

        /// <summary>
        /// Gets or sets the translated content.
        /// </summary>
        /// <value>The translated content.</value>
        public string Translated { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public string Context { get; set; }

        /// <summary>
        /// Gets or sets the translator comment.
        /// </summary>
        /// <value>The translator comment.</value>
        public string TranslatorComment { get; set; }

        /// <summary>
        /// Gets or sets the extracted comments.
        /// </summary>
        /// <value>The extracted comments.</value>
        public string ExtractedComments { get; set; }

        /// <summary>
        /// Gets or sets the reference.
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
