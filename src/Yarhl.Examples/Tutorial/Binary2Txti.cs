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

#region Class
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;

public class Binary2Txti : IConverter<IBinary, TxtiFormat>
{
#endregion

    #region ValidateHeader
    public TxtiFormat Convert(IBinary source)
    {
        var reader = new DataReader(source.Stream);

        if (reader.ReadString(bytesCount: 4) != "TXTI") {
            throw new FormatException("Invalid format");
        }
        #endregion

        #region ReadEntries
        var txtiFormat = new TxtiFormat();

        int numEntries = reader.ReadInt32();
        for (int i = 0; i < numEntries; i++) {
            var entry = new TextEntry();
            entry.Id = reader.ReadUInt16();
            entry.Text = reader.ReadString(Encoding.Unicode);

            txtiFormat.Entries.Add(entry);
        }

        return txtiFormat;
        #endregion
    }
}
