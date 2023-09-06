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

using System.Collections.ObjectModel;
using Yarhl.FileFormat;

public static class Formats
{
    #region FormatImpl
    public class GameTextFormat : IFormat
    {
        public Collection<string> Texts { get; init; }

        public int SceneId { get; set; }
    }
    #endregion

    #region FormatWrapper
    public class SoundFormat : ThirdPartyWave, IFormat
    {
    }
    #endregion

    public class ThirdPartyWave
    {
    }

    #region CloneableFormat
    public class Image : ICloneableFormat
    {
        public int Width { get; set; }

        public int Height { get; set; }

        public byte[] IndexedPixels { get; set; }

        public object DeepClone()
        {
            return new Image {
                Width = Width,
                Height = Height,
                IndexedPixels = IndexedPixels.ToArray(),
            };
        }
    }
    #endregion
}
