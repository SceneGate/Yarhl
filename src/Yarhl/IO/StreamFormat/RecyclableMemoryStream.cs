// Copyright (c) 2019 SceneGate Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
namespace Yarhl.IO.StreamFormat
{
    using Microsoft.IO;

    internal sealed class RecyclableMemoryStream : StreamWrapper
    {
        static readonly RecyclableMemoryStreamManager Manager = CreateManager();

        public RecyclableMemoryStream()
            : base(Manager.GetStream())
        {
        }

        public override void SetLength(long length)
        {
            long oldLength = Length;
            int additionalLength = (int)(length - oldLength);
            base.SetLength(length);

            if (additionalLength > 0) {
                long oldPos = Position;
                Position = oldLength;

                // TODO: Write in blocks
                byte[] clearData = new byte[additionalLength];
                Write(clearData, 0, additionalLength);

                Position = oldPos;
            }
        }

        static RecyclableMemoryStreamManager CreateManager()
        {
            return new RecyclableMemoryStreamManager();
        }
    }
}
