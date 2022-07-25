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
    using Yarhl.FileFormat;

    /// <summary>
    /// Portable Object format for translations.
    /// </summary>
    public class Po : ICloneableFormat
    {
        readonly IList<PoEntry> entries;
        readonly ReadOnlyCollection<PoEntry> readonlyEntries;
        readonly IDictionary<string, PoEntry> searchEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="Po"/> class.
        /// </summary>
        public Po()
        {
            Header = new PoHeader();
            entries = new List<PoEntry>();
            readonlyEntries = new ReadOnlyCollection<PoEntry>(entries);
            searchEntries = new Dictionary<string, PoEntry>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Po"/> class.
        /// </summary>
        /// <param name="headerArg">PO header.</param>
        public Po(PoHeader headerArg)
            : this()
        {
            Header = headerArg;
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public PoHeader Header {
            get;
            set;
        }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public ReadOnlyCollection<PoEntry> Entries => readonlyEntries;

        /// <summary>
        /// Add the specified entry.
        /// </summary>
        /// <param name="item">Entry to add. The ID must be unique.</param>
        public void Add(PoEntry item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrEmpty(item.Original))
                throw new FormatException(nameof(item.Original) + " is empty");

            string key = GetKey(item);
            if (searchEntries.ContainsKey(key)) {
                MergeEntry(searchEntries[key], item);
            } else {
                searchEntries[key] = item;
                entries.Add(item);
            }
        }

        /// <summary>
        /// Add the specified entries.
        /// </summary>
        /// <param name="items">Entries to add.</param>
        public void Add(IEnumerable<PoEntry> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (PoEntry entry in items)
                Add(entry);
        }

        /// <summary>
        /// Gets a <see cref="PoEntry"/> from the original text.
        /// </summary>
        /// <param name="original">Original text from the entry.</param>
        /// <param name="context">Context text from the entry.</param>
        /// <returns>The found entry or null if not found.</returns>
        public PoEntry? FindEntry(string original, string? context = null)
        {
            if (string.IsNullOrEmpty(original))
                throw new ArgumentNullException(nameof(original));

            string key = GetKey(original, context);
            return searchEntries.ContainsKey(key) ? searchEntries[key] : null;
        }

        /// <inheritdoc />
        public virtual object DeepClone()
        {
            var clonedHeader = new PoHeader(Header);
            Po clone = new Po(clonedHeader);

            foreach (PoEntry entry in entries)
            {
                clone.Add(new PoEntry(entry));
            }

            return clone;
        }

        static string GetKey(PoEntry entry)
        {
            return GetKey(entry.Original, entry.Context);
        }

        static string GetKey(string original, string? context)
        {
            return original + "||" + (context ?? string.Empty);
        }

        static void MergeEntry(PoEntry current, PoEntry newEntry)
        {
            if (current.Translated != newEntry.Translated) {
                throw new InvalidOperationException(
                        "Tried to merge Po entries with same original text but " +
                        "different translations.");
            }

            if (!string.IsNullOrEmpty(newEntry.Reference))
                current.Reference += "," + newEntry.Reference;
        }
    }
}
