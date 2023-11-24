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
namespace Yarhl.FileSystem;

using System;
using System.Collections.Generic;
using System.Linq;
using Yarhl.FileFormat;

/// <summary>
/// Extension methods for nodes.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    /// Iterate and transform the nodes in the collection with the given converter.
    /// </summary>
    /// <typeparam name="TConv">The type of the converter.</typeparam>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <returns>The same collection.</returns>
    /// <remarks>
    /// It creates a new instance of the converter for each node.
    /// It performs the conversion inmediately, not IEnumerable-styled.
    /// </remarks>
    public static NavigableNodeCollection<Node> TransformCollectionWith<TConv>(this NavigableNodeCollection<Node> nodes)
            where TConv : IConverter, new()
    {
        foreach (Node node in nodes) {
            _ = node.TransformWith<TConv>();
        }

        return nodes;
    }

    /// <summary>
    /// Iterate and transform the nodes in the collection with the given converter.
    /// </summary>
    /// <returns>This same collection.</returns>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <param name="converterType">The type of the converter to use.</param>
    /// <param name="args">
    /// Arguments for the constructor of the type if any.
    /// </param>
    /// <remarks>
    /// It creates a new instance of the converter for each node.
    /// </remarks>
    public static NavigableNodeCollection<Node> TransformCollectionWith(
        this NavigableNodeCollection<Node> nodes,
        Type converterType,
        params object?[] args)
    {
        foreach (Node node in nodes) {
            _ = node.TransformWith(converterType, args);
        }

        return nodes;
    }

    /// <summary>
    /// Iterate and transform the nodes in the collection with the given converter.
    /// </summary>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <param name="converter">Convert to use.</param>
    /// <returns>The same collection.</returns>
    /// <remarks>It re-uses the same converter instance for each node.</remarks>
    public static NavigableNodeCollection<Node> TransformCollectionWith(
        this NavigableNodeCollection<Node> nodes,
        IConverter converter)
    {
        foreach (Node node in nodes) {
            _ = node.TransformWith(converter);
        }

        return nodes;
    }

    /// <summary>
    /// Creates a new IEnumerable to transform the nodes with the given converter.
    /// </summary>
    /// <typeparam name="TConv">The type of the converter.</typeparam>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <returns>The same collection.</returns>
    /// <remarks>
    /// It creates a new instance of the converter for each node.
    /// It returns a new IEnumerable and will run the conversion when iterated.
    /// </remarks>
    public static IEnumerable<Node> TransformWith<TConv>(this IEnumerable<Node> nodes)
            where TConv : IConverter, new()
    {
        return nodes.Select(n => n.TransformWith<TConv>());
    }

    /// <summary>
    /// Creates a new IEnumerable to transform the nodes with the given converter.
    /// </summary>
    /// <returns>This same collection.</returns>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <param name="converterType">The type of the converter to use.</param>
    /// <param name="args">
    /// Arguments for the constructor of the type if any.
    /// </param>
    /// <remarks>
    /// It creates a new instance of the converter for each node.
    /// </remarks>
    public static IEnumerable<Node> TransformWith(
        this IEnumerable<Node> nodes,
        Type converterType,
        params object?[] args)
    {
        return nodes.Select(n => n.TransformWith(converterType, args));
    }

    /// <summary>
    /// Creates a new IEnumerable to transform the nodes with the given converter.
    /// </summary>
    /// <param name="nodes">The collection of nodes to transform.</param>
    /// <param name="converter">Convert to use.</param>
    /// <returns>The same collection.</returns>
    /// <remarks>It re-uses the same converter instance for each node.</remarks>
    public static IEnumerable<Node> TransformWith(this IEnumerable<Node> nodes, IConverter converter)
    {
        return nodes.Select(n => n.TransformWith(converter));
    }
}
