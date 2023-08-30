﻿// Copyright (c) 2022 SceneGate

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
using System.IO;
using SceneGate.Ekona.Containers.Rom;
using SceneGate.Games.ProfessorLayton.Containers;
using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Yarhl.Examples;

internal static class Introduction
{
    /// <summary>
    /// Export texts from Professor Layton London Life game.
    /// </summary>
    /// <param name="gameFilePath">Path to game file.</param>
    internal static void ExportText(string gameFilePath)
    {
        #region Demo1
        // Read game file system
        Node game = NodeFactory.FromFile(gameFilePath, FileOpenMode.Read)
            .TransformWith<Binary2NitroRom>();

        // Navigate to the container that has our text file and unpack it.
        Node msgNode = Navigator.SearchNode(game, "data/ll/common/ll_common.darc")
            .TransformWith<BinaryDarc2Container>() // binary -> file system (container)
            .Children[2]                           // text file is the third file
            .TransformWith<DencDecompression>();   // the file is compressed with LZSS

        // Convert its proprietary binary format into industry-standard translation format PO.
        // As it's a huge text file, the converter splits the content into different files.
        msgNode.TransformWith<Binary2MessageCollection>()
            .TransformWith<MessageCollection2PoContainer, LondonLifeRegion>(LondonLifeRegion.Usa);

        foreach (var children in msgNode.Children) {
            children.TransformWith<Po2Binary>()
                .Stream.WriteTo(Path.Combine("outputs", "london_life", $"{children.Name}.po"));
        }
        #endregion

        #region Demo2
        Node textNode = msgNode.Children["Script dialogs"];

        // Converts back to PO format as in previous demo we serialized into binary.
        textNode.TransformWith<Binary2Po>();

        // Get format object.
        Po po = textNode.GetFormatAs<Po>();

        // Change one translation entry.
        po.Entries[0].Translated = "Hello world!";

        // Save the file again
        textNode.TransformWith<Po2Binary>()
            .Stream.WriteTo(Path.Combine("outputs", "london_life", "translated.po"));
        #endregion
    }
}