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

public static class NodeFactoryExamples
{
    public static void Constructor()
    {
        #region Constructor
        IFormat nodeFormat = CreateNodeFormat();
        using var node = new Node("myNode", nodeFormat);

        using var nodeWithoutFormat = new Node("myNode2");
        #endregion
    }

    public static void FromBinary()
    {
        #region BinaryData
        using Node binMemoryNode = NodeFactory.FromMemory("memory");

        byte[] data = new byte[] { 0xBA, 0xAD, 0xCA, 0xFE, 0xD0, 0x0D };
        using Node binArrayNode = NodeFactory.FromArray("bad coffe", data);

        using Node binSubArrayNode = NodeFactory.FromArray("coffee", data, 2, 2);

        var fileStream = new FileStream("file.bin", FileMode.Open);
        using Node binStreamNode = NodeFactory.FromStream("stream", fileStream);

        using Node binSubStreamNode = NodeFactory.FromSubstream("substream", fileStream, 0x100, 0x80);
        #endregion
    }

    public static void FromFiles()
    {
        #region Files
        using Node fileNode = NodeFactory.FromFile("inputs/file.bin", FileOpenMode.Read);

        using Node textsNode = NodeFactory.FromFile("inputs/file.bin", "texts", FileOpenMode.ReadWrite);

        FileInfo fileInfo = fileNode.Tags["FileInfo"];
        #endregion
    }

    public static void CreateContainer()
    {
        #region Container
        using Node container = NodeFactory.CreateContainer("graphics");
        #endregion
    }

    public static void FromDirectory()
    {
        #region Directory1
        // File system:
        // inputs/
        // |- menu.txt
        // |- logo.png
        // |- maps/
        // |--- names.txt
        // |--- world.png
        // |--- scenarios/
        // |----- map1.png
        // |--- regular_font.ttf

        using Node inputNode = NodeFactory.FromDirectory("inputs/");
        Console.WriteLine(inputNode.Name); // input
        Console.WriteLine(inputNode.Children.Count); // 2: menu.txt and logo.png

        using Node topTexts = NodeFactory.FromDirectory("inputs/", "*.txt");
        Console.WriteLine(inputNode.Children.Count); // 1: menu.txt
        #endregion

        #region Directory2
        // Same as 'topTexts' with custom name
        using Node topTextsWithName = NodeFactory.FromDirectory("inputs/", "*.txt", "texts");

        // Node with full hierarchy and filter
        using Node images = NodeFactory.FromDirectory("inputs/", "*.png", "images", true);
        Node map1 = images.Children["maps"].Children["scenarios"].Children["map1.png"];
        #endregion

        #region Directory3
        static bool IsScenarioImageOrFont(string path) =>
            (path.StartsWith("inputs/maps/scenarios") && path.EndsWith(".png"))
            || path.EndsWith(".ttf");

        using Node scenarios = NodeFactory.FromDirectory("inputs/", IsScenarioImageOrFont, "data", true);
        #endregion
    }

    public static void CreateHierarchy()
    {
        #region CreateHierarchy
        using Node root = NodeFactory.CreateContainer("root");

        string childPath = "data/gfx/scene1";
        using Node child = new Node("child1", CreateNodeFormat());

        NodeFactory.CreateContainersForChild(root, childPath, child);
        Node sameChild = root.Children["data"].Children["gfx"].Children["scene1"].Children["child1"];
        #endregion
    }

    private static IFormat CreateNodeFormat() => new BinaryFormat();
}
