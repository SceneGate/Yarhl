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
namespace Yarhl.Examples.Tutorial;

using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

public static class Program
{
    public static void FakeMain(string[] args)
    {
        #region OpenFile
        DataStream fileStream = DataStreamFactory.FromFile(args[0], FileOpenMode.Read);
        using var binaryFormat = new BinaryFormat(fileStream);
        #endregion

        #region Deserialize
        var deserializer = new Binary2Txti();
        var txti = deserializer.Convert(binaryFormat);

        Console.WriteLine($"Number of entries: {txti.Entries.Count}");
        Console.WriteLine($"First text: '{txti.Entries[0].Text}'");
        #endregion
    }

    public static void FakeMain2(string[] args)
    {
        #region ExportNodes
        // Create a node from a file (BinaryFormat)
        using Node node = NodeFactory.FromFile(args[0], FileOpenMode.Read);

        // Transform chaining conversions fluent-like.
        node.TransformWith(new Binary2Txti())
            .TransformWith(new Txti2Po())
            .TransformWith(new Po2Binary());

        // Save our new binary data into disk
        node.Stream!.WriteTo(args[1]);
        #endregion
    }
}
