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
namespace Yarhl.Examples.IO;

using Yarhl.IO;

public static class DataStreamExamples
{
    public static void CreateSubStreamConstructor()
    {
        #region SubStreamConstructor
        var baseStream = new FileStream("container.bin", FileMode.Open);
        using var file1Stream = new DataStream(baseStream, 0x100, 0x2C0);
        using var file2Stream = new DataStream(baseStream, 0x3C0, 0x80);
        #endregion
    }

    public static void CreateSubStreamSlice()
    {
        #region SubStreamSlice
        DataStream containerStream = DataStreamFactory.FromFile("container.bin", FileOpenMode.Read);
        using DataStream file1Stream = containerStream.Slice(0x100, 0x2C0);
        using DataStream file2Stream = containerStream.Slice(0x3C0, 0x80);
        using DataStream lastFileStream = containerStream.Slice(0x8400);
        #endregion
    }
}
