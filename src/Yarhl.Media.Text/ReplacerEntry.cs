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
    /// Entry of the map of replacements.
    /// </summary>
    public struct ReplacerEntry : IEquatable<ReplacerEntry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplacerEntry"/> struct.
        /// </summary>
        /// <param name="original">The original string to replace.</param>
        /// <param name="modified">The modified string.</param>
        public ReplacerEntry(string original, string modified)
        {
            Original = original;
            Modified = modified;
        }

        /// <summary>
        /// Gets the original string.
        /// </summary>
        /// <returns>Original string.</returns>
        public string Original { get; }

        /// <summary>
        /// Gets the modified string.
        /// </summary>
        /// <returns>Modified string.</returns>
        public string Modified { get; }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="entry1">First entry to compare.</param>
        /// <param name="entry2">Second entry to compare.</param>
        /// <returns>
        /// true if the first object is equal to the second object;
        /// otherwise, false.
        /// </returns>
        public static bool operator ==(ReplacerEntry entry1, ReplacerEntry entry2)
        {
            return entry1.Equals(entry2);
        }

        /// <summary>
        /// Determines whether two object instances are different.
        /// </summary>
        /// <param name="entry1">First entry to compare.</param>
        /// <param name="entry2">Second entry to compare.</param>
        /// <returns>
        /// true if the first object is different to the second object;
        /// otherwise, false.
        /// </returns>
        public static bool operator !=(ReplacerEntry entry1, ReplacerEntry entry2)
        {
            return !entry1.Equals(entry2);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return Original.GetHashCode() ^ Modified.GetHashCode();
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="obj">
        /// The object to compare with the current object.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the current object;
        /// otherwise, false.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj is not ReplacerEntry otherEntry) {
                return false;
            }

            return Equals(otherEntry);
        }

        /// <summary>
        /// Determines whether two object instances are equal.
        /// </summary>
        /// <param name="other">
        /// The instance to compare with the current instance.
        /// </param>
        /// <returns>
        /// true if the specified object is equal to the current object;
        /// otherwise, false.
        /// </returns>
        public bool Equals(ReplacerEntry other)
        {
            return other.Original == Original && other.Modified == Modified;
        }
    }
}
