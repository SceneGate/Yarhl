#!/usr/bin/env dotnet-script

#r "nuget: Yarhl, 3.1.1-preview.28"
#r "nuget: SceneGate.Ekona, 0.1.0-preview.13"
#r "nuget: LayTea, 0.1.0-preview.87"

using System.IO;
using SceneGate.Ekona.Containers.Rom;
using SceneGate.Games.ProfessorLayton.Containers;
using SceneGate.Games.ProfessorLayton.Texts.LondonLife;
using Yarhl.FileSystem;
using Yarhl.IO;
using Yarhl.Media.Text;

// ExportText(Args[0]);
ExportText("/store/Juegos/NDS/Profesor Layton - The Last Specter.nds");

/// <summary>
/// Export texts from Professor Layton London Life game.
/// </summary>
/// <param name="gameFilePath">Path to game file.</param>
void ExportText(string gameFilePath)
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

    // Old bug already fixed in latest preview. Required for Binary2Po.
    msgNode.Children["Script dialogs"].Stream.Position = 0;

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
