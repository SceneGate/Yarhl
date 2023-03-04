// Copyright (c) 2019 SceneGate

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
namespace Yarhl.PerformanceTests.IO
{
    using BenchmarkDotNet.Attributes;
    using Yarhl.IO;

    public class DataStreamCompare
    {
        DataStream stream1;
        DataStream stream2;

        [Params(32, 1024, 5 * 1024, 80 * 1024, 1024 * 1024)]
        public long Length { get; set; }

        [GlobalSetup]
        public void SetUp()
        {
            stream1 = new DataStream();
            for (int i = 0; i < Length; i++) {
                stream1.WriteByte((byte)(i % 256));
            }

            stream2 = new DataStream();
            stream1.WriteTo(stream2);
        }

        [GlobalCleanup]
        public void CleanUp()
        {
            stream1?.Dispose();
            stream2?.Dispose();
        }

        [Benchmark]
        public void Compare()
        {
            stream1.Compare(stream2);
        }
    }
}
