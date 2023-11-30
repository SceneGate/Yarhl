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
namespace Yarhl.Plugins;

/// <summary>
/// Provides information about a type that implements a generic base type.
/// </summary>
/// <param name="Name">The name of the implementation type. Shortcut for Type.FullName.</param>
/// <param name="Type">The implementation type.</param>
/// <param name="GenericBaseType">The actual generic base type with type parameters implemented.</param>
/// <param name="GenericTypeParameters">
/// The collection of the type parameters in the generic base type implemented.
/// </param>
public record GenericTypeImplementationInfo(
    string Name,
    Type Type,
    Type GenericBaseType,
    IReadOnlyList<Type> GenericTypeParameters)
    : TypeImplementationInfo(Name, Type);
