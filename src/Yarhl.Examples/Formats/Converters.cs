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
namespace Yarhl.Examples.Formats;

using Yarhl.Media.Text;

public class Converters
{
    public void SerializePo()
    {
        #region SerializePo
        // Create a test PO model format to convert.
        var poFormat = new Po(new PoHeader("software", "SceneGate", "en-US"));
        poFormat.Add(new PoEntry("Hello, world!"));

        // Create a new converter instance
        var po2binary = new Po2Binary();

        // Convert!
        using var binaryPoFormat = po2binary.Convert(poFormat);

        // Binary format is a wrapper over a DataStream (enhanced System.IO.Stream)
        // We can now save the Stream into a file
        binaryPoFormat.Stream.WriteTo("strings.po");
        #endregion
    }
}
