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
namespace Yarhl.Examples.FileSystem;

using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

public static class NodeExamples
{
    public static void Overview()
    {
        #region Overview
        // Create node from a disk file
        using Node game = NodeFactory.FromFile("game.nds", FileOpenMode.Read);

        // Use the `Binary2NitroRom` converter to convert the binary format
        // into node containers (virtual file system tree).
        game.TransformWith<Binary2NitroRom>();

        // Now we can access to every game file. For instance, we can export one file
        Node gameFile = game.Children["data"].Children["graphics"].Children["map.bin"];

        // Same FileStream but reading from different offsets.
        // No disk writing was required.
        bool isSame = gameFile.Stream.BaseStream == game.Stream.BaseStream;
        #endregion
    }

    public static void Children()
    {
        #region AccessChildren
        using Node node = NodeFactory.FromDirectory("inputs/");

        Node childByIndex = node.Children[0];
        Node childByName = node.Children["menu.txt"];
        Node subChild = node.Children["maps"].Children["map1.scr"];

        foreach (Node child in node.Children) {
            // ...
        }

        for (int i = 0; i < node.Children.Count; i++) {
            Node child = node.Children[i];
            // ...
        }
        #endregion
    }

    public static void AddRemove()
    {
        #region AddRemove
        using Node root = NodeFactory.CreateContainer("root");

        Node child = new Node("file1", new Po());
        root.Add(child);

        Node childSameName = new Node("file1", new BinaryFormat());
        root.Add(childSameName); // now root.Children[0] has BinaryFormat

        Node child2 = new Node("file2");
        root.Add(child2);

        // The nodes are NOT disposed
        root.Remove("file1");
        bool found = root.Remove(child2); // true
        bool notFound = root.Remove("IDontExists"); // false

        // or alternatively
        root.RemoveChildren(dispose: true);
        #endregion
    }

    public static void Transform()
    {
        #region Transform
        using Node text = NodeFactory.FromFile("input.bin");

        text.TransformWith(new XorEncryptor("password"));
        text.TransformWith<Binary2Texts>();
        #endregion

        #region TransformChain
        using Node graphics = NodeFactory.FromFile("graphics.bin.lz")
            .TransformWith(new XorEncryptor("password"))
            .TransformWith<LzDecompressor>()
            .TransformWith<Narc2Container>();
        #endregion
    }

    private sealed class Binary2NitroRom : IConverter<IBinary, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(IBinary source) =>
            throw new NotImplementedException();
    }

    private sealed class Binary2Texts : IConverter<IBinary, Po>
    {
        public Po Convert(IBinary source) => throw new NotImplementedException();
    }

    private sealed class LzDecompressor : IConverter<IBinary, IBinary>
    {
        public IBinary Convert(IBinary source) => throw new NotImplementedException();
    }

    private sealed class XorEncryptor : IConverter<IBinary, IBinary>
    {
        public XorEncryptor(string password)
        {
        }

        public IBinary Convert(IBinary source) => throw new NotImplementedException();
    }

    private sealed class Narc2Container : IConverter<IBinary, NodeContainerFormat>
    {
        public NodeContainerFormat Convert(IBinary source) => throw new NotImplementedException();
    }
}
