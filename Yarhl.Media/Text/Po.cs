//
// Po.cs
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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Mono.Addins;
    using Yarhl.FileFormat;

    /// <summary>
    /// Portable Object format for translations.
    /// </summary>
    [Extension]
    public class Po : Format
    {
        readonly IList<PoEntry> entries;
        readonly ReadOnlyCollection<PoEntry> readOnlyEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="Po"/> class.
        /// </summary>
        public Po()
        {
            entries = new List<PoEntry>();
            readOnlyEntries = new ReadOnlyCollection<PoEntry>(entries);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Po"/> class.
        /// </summary>
        /// <param name="header">PO header.</param>
        public Po(PoHeader header)
            : this()
        {
            Header = header;
        }

        /// <summary>
        /// Gets the format name.
        /// </summary>
        /// <value>The format name.</value>
        public override string Name => "Yarhl.common.po";

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public PoHeader Header { get; set; }

        /// <summary>
        /// Gets the entries.
        /// </summary>
        /// <value>The entries.</value>
        public ReadOnlyCollection<PoEntry> Entries => readOnlyEntries;

        /// <summary>
        /// Add the specified entry.
        /// </summary>
        /// <param name="item">Entry to add. The ID must be unique.</param>
        public void Add(PoEntry item)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Po));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            PoEntry sameEntry = entries.FirstOrDefault((entry) =>
                entry.Original == item.Original &&
                entry.Context == item.Context);
            if (sameEntry != null)
                MergeEntry(sameEntry, item);
            else
                entries.Add(item);
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

        void MergeEntry(PoEntry current, PoEntry newEntry)
        {
            if (current.Translated != newEntry.Translated)
                throw new InvalidOperationException(
                    "Tried to merge Po entries with same original text but " +
                    "different translations.");

            if (newEntry.Reference != null)
                current.Reference += "," + newEntry.Reference;
        }
    }
}
