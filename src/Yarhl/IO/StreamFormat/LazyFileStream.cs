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
    using System.IO;

    internal sealed class LazyFileStream : StreamWrapper
    {
        readonly string path;
        readonly FileOpenMode mode;
        readonly long initialLength;

        bool isInitialized;

        public LazyFileStream(string path, FileOpenMode mode)
        {
            this.path = path;
            this.mode = mode;

            // Before the stream is initialized, we can get the
            // length so we can report it wihthout opening the file.
            var info = new FileInfo(path);
            initialLength = info.Exists ? info.Length : 0;
        }

        public override long Length => BaseStream?.Length ?? initialLength;

        public override void SetLength(long length)
        {
            if (!isInitialized) {
                Initialize();
            }

            base.SetLength(length);
        }

        public override int Read(byte[] buffer, int index, int count)
        {
            if (!isInitialized) {
                Initialize();
            }

            return base.Read(buffer, index, count);
        }

        public override byte ReadByte()
        {
            if (!isInitialized) {
                Initialize();
            }

            return base.ReadByte();
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            if (!isInitialized) {
                Initialize();
            }

            base.Write(buffer, index, count);
        }

        public override void WriteByte(byte data)
        {
            if (!isInitialized) {
                Initialize();
            }

            base.WriteByte(data);
        }

        void Initialize()
        {
            isInitialized = true;
            BaseStream = new FileStream(path, mode.ToFileMode(), mode.ToFileAccess());
        }
    }
}
