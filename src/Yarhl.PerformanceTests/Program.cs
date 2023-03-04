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
#pragma warning disable SA1200 // False positive of namespace and using
using System.CommandLine;
using BenchmarkDotNet.Running;
#pragma warning restore SA1200

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("", "SA1516", Justification = "Broken blank line rule")]

var encodingPerf = new Command("encoding", "Run the perf test for encodings");
encodingPerf.SetHandler(() => BenchmarkRunner.Run<Yarhl.PerformanceTests.Encodings.EncodingSpan>());

var streamComparePerf = new Command("stream-compare", "Run the perf test for DataStream compare");
streamComparePerf.SetHandler(() => BenchmarkRunner.Run<Yarhl.PerformanceTests.IO.DataStreamCompare>());

var streamReadWritePerf = new Command("stream-rw", "Run the perf test for DataStream Read/Write");
streamReadWritePerf.SetHandler(() => BenchmarkRunner.Run<Yarhl.PerformanceTests.IO.DataStreamReadWriteTests>());

var rootCommand = new RootCommand("Yarhl performance tests")
{
    encodingPerf,
    streamComparePerf,
    streamReadWritePerf,
};

return rootCommand.Invoke(args);
