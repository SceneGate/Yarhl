// Copyright (c) 2023 SceneGate

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
namespace Yarhl.IO;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yarhl.FileFormat;

/// <content>
/// Obsoleted methods in version 4.
/// To be removed before next major.
/// </content>
public partial class DataStream
{
    /// <summary>
    /// Move the position of the Stream.
    /// </summary>
    /// <param name="shift">Distance to move position.</param>
    /// <param name="mode">Mode to move position.</param>
    [Obsolete("Use the overload with SeekOrigin.")]
    public void Seek(long shift, SeekMode mode)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(DataStream));

        switch (mode) {
            case SeekMode.Current:
                _ = Seek(shift, SeekOrigin.Current);
                break;
            case SeekMode.Start:
                _ = Seek(shift, SeekOrigin.Begin);
                break;
            case SeekMode.End:
                _ = Seek(shift, SeekOrigin.End);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }
    }

    /// <summary>
    /// Push the current position into a stack and move to a new one.
    /// </summary>
    /// <param name="shift">Distance to move position.</param>
    /// <param name="mode">Mode to move position.</param>
    [Obsolete("Use the overload with SeekOrigin.")]
    public void PushToPosition(long shift, SeekMode mode)
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(DataStream));

        positionStack.Push(Position);
        Seek(shift, mode);
    }

    /// <summary>
    /// Run a method in a specific position.
    /// This command will move into the position, run the method and return
    /// to the current position.
    /// </summary>
    /// <param name="action">Action to run.</param>
    /// <param name="position">Position to move.</param>
    /// <param name="mode">Mode to move position.</param>
    [Obsolete("Use the overload with SeekOrigin.")]
    public void RunInPosition(Action action, long position, SeekMode mode)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        PushToPosition(position, mode);
        action();
        PopPosition();
    }

    /// <summary>
    /// Reads a format from this stream.
    /// </summary>
    /// <returns>The format read.</returns>
    /// <typeparam name="T">The type of the format to read.</typeparam>
    [Obsolete("ConvertFormat.To() is obsoleted. Use the converter directly.")]
    public T ReadFormat<T>()
    {
        if (Disposed)
            throw new ObjectDisposedException(nameof(DataStream));

        T format;
        using (var binary = new BinaryFormat(this))
            format = ConvertFormat.To<T>(binary);
        return format;
    }

}
