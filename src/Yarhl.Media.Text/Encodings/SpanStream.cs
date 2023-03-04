// Copyright (c) 2020 SceneGate

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
namespace Yarhl.Media.Text.Encodings;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

/// <summary>
/// Efficient stream-like implemented with Span.
/// </summary>
/// <typeparam name="T">The type of the data of the stream.</typeparam>
public readonly ref struct SpanStream<T>
{
    readonly Span<T> buffer;
    readonly Counter position;
    readonly bool hasBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpanStream{T}"/> struct.
    /// </summary>
    /// <param name="buffer">The buffer to write.</param>
    internal SpanStream(Span<T> buffer)
    {
        this.buffer = buffer;
        hasBuffer = !buffer.IsEmpty;

        // Position must be a reference type otherwise every time we pass this
        // instance in a method we will lose the last position back.
        position = new Counter();
    }

    /// <summary>
    /// Gets the length of the current stream.
    /// </summary>
    public int Length => position.Count;

    /// <summary>
    /// Writes a value in the stream.
    /// </summary>
    /// <param name="data">Value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(T data)
    {
        // For performance reasons we create a local variable to avoid
        // accessing the reference type many times.
        int count = position.Count;
        if (hasBuffer) {
            buffer[count] = data;
        }

        position.Count = count + 1;
    }

    /// <summary>
    /// Writes a sequence of values in the stream.
    /// </summary>
    /// <param name="data">The data to write in the stream.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<T> data)
    {
        // For performance reasons we create a local variable to avoid
        // accessing the reference type many times.
        int count = position.Count;
        if (hasBuffer) {
            data.CopyTo(buffer[count..]);
        }

        position.Count = count + data.Length;
    }

    private sealed class Counter
    {
        // It's a public variable for performance reasons
        [SuppressMessage("", "SA1401:Fields should be private", Justification = "Internal and for performance reason")]
        internal int Count;
    }
}
