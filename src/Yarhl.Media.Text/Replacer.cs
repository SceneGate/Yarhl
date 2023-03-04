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
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// Replaces substrings in a text from a map.
    /// </summary>
    public class Replacer
    {
        readonly IList<ReplacerEntry> map;

        /// <summary>
        /// Initializes a new instance of the <see cref="Replacer"/> class.
        /// </summary>
        public Replacer()
        {
            map = new List<ReplacerEntry>();
        }

        /// <summary>
        /// Gets the map for the replacements.
        /// </summary>
        /// <returns>The map for the replacements.</returns>
        public ReadOnlyCollection<ReplacerEntry> Map {
            get { return new ReadOnlyCollection<ReplacerEntry>(map); }
        }

        /// <summary>
        /// Add or replace an entry in the map.
        /// </summary>
        /// <param name="src">The source field for the entry.</param>
        /// <param name="dst">The destination field for the entry.</param>
        public void Add(string src, string dst)
        {
            if (string.IsNullOrEmpty(src))
                throw new ArgumentNullException(nameof(src));

            if (string.IsNullOrEmpty(dst))
                throw new ArgumentNullException(nameof(dst));

            int index = FindMapEntry(src, true);
            if (index == -1)
                map.Add(new ReplacerEntry(src, dst));
            else
                map[index] = new ReplacerEntry(src, dst);
        }

        /// <summary>
        /// Remove an entry from the map.
        /// </summary>
        /// <param name="src">The original field from the entry.</param>
        public void Remove(string src)
        {
            if (string.IsNullOrEmpty(src))
                throw new ArgumentNullException(nameof(src));

            int index = FindMapEntry(src, true);
            if (index == -1)
                throw new KeyNotFoundException("Key not found: " + src);

            map.RemoveAt(index);
        }

        /// <summary>
        /// Transform a text replacing the chars from source to destination.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <remarks>
        /// <para>When multiple original fields in the map have same start, the
        /// later map entry will have preference.</para>
        /// </remarks>
        /// <returns>The transformed text.</returns>
        public string TransformForward(string text)
        {
            return Transform(text, true);
        }

        /// <summary>
        /// Transform a text with replacing chars from destination to source.
        /// </summary>
        /// <param name="text">The text to transform.</param>
        /// <remarks>
        /// <para>When multiple original fields in the map have same start, the
        /// later map entry will have preference.</para>
        /// </remarks>
        /// <returns>The transformed text.</returns>
        public string TransformBackward(string text)
        {
            return Transform(text, false);
        }

        string Transform(string text, bool isOriginalText)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            // Get a list of matches by order of the map
            IDictionary<int, int> matches = MatchMap(text, isOriginalText);

            // From the list of matches, rebuild the string
            StringBuilder builder = new StringBuilder();
            int textIdx = 0;
            while (textIdx < text.Length) {
                if (!matches.ContainsKey(textIdx)) {
                    builder.Append(text[textIdx++]);
                } else {
                    ReplacerEntry entry = map[matches[textIdx]];
                    string original = isOriginalText ? entry.Original : entry.Modified;
                    string modified = isOriginalText ? entry.Modified : entry.Original;

                    // Append the modified in the new string and skip the original
                    builder.Append(modified);
                    textIdx += original.Length;
                }
            }

            return builder.ToString();
        }

        IDictionary<int, int> MatchMap(string text, bool originalText)
        {
            var comparison = StringComparison.Ordinal;
            var matches = new Dictionary<int, int>();
            for (int i = 0; i < map.Count; i++) {
                string original = originalText ? map[i].Original : map[i].Modified;
                int matchIdx = text.IndexOf(original, comparison);
                while (matchIdx != -1) {
                    matches[matchIdx] = i;
                    matchIdx = text.IndexOf(original, matchIdx + 1, comparison);
                }
            }

            return matches;
        }

        int FindMapEntry(string key, bool source)
        {
            int index = -1;
            for (int i = 0; i < map.Count && index == -1; i++) {
                var entryKey = source ? map[i].Original : map[i].Modified;
                index = (entryKey == key) ? i : -1;
            }

            return index;
        }
    }
}
