namespace Yarhl.PerformanceTests.FileSystem;

using System;
using BenchmarkDotNet.Attributes;
using Yarhl.FileFormat;
using Yarhl.FileSystem;
using Yarhl.IO;

[MemoryDiagnoser]
public class NodeTransformWithInterfaces
{
    private EnhancedNode testNode;
    private NoopConverter testConverter;

    [GlobalSetup]
    public void Setup()
    {
        testNode = new EnhancedNode("myNode", new BinaryFormat());
        testConverter = new NoopConverter();
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        testNode.Dispose();
    }

    [Benchmark]
    public Node TransformWithDynamic()
    {
        return testNode.Candidate1_TransformWith(testConverter);
    }

    [Benchmark]
    public Node TransformWithTyped()
    {
        return testNode.Candidate2_TransformWith<BinaryFormat, BinaryFormat>(testConverter);
    }

    private sealed class NoopConverter :
        IConverter<BinaryFormat, BinaryFormat>,
        IConverter<NodeContainerFormat, BinaryFormat>
    {
        public BinaryFormat Convert(BinaryFormat source)
        {
            return source;
        }

        public BinaryFormat Convert(NodeContainerFormat source)
        {
            // Dummy so you can't omit the generics in the call.
            return new BinaryFormat();
        }
    }

    private sealed class EnhancedNode : Node
    {
        public EnhancedNode(string name, IFormat format)
            : base(name, format)
        {
        }

        public Node Candidate1_TransformWith(IConverter converter)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (Format is null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ConvertFormat.ValidateConverterType(converter.GetType(), Format.GetType());

            dynamic converterDyn = converter;
            dynamic source = Format;
            IFormat newFormat = converterDyn.Convert(source);

            ChangeFormat(newFormat);

            return this;
        }

        public Node Candidate2_TransformWith<TSrc, TDst>(IConverter<TSrc, TDst> converter)
            where TSrc : IFormat
            where TDst : IFormat
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Node));

            if (converter == null)
                throw new ArgumentNullException(nameof(converter));

            if (Format is null) {
                throw new InvalidOperationException(
                    "Cannot transform a node without format");
            }

            ConvertFormat.ValidateConverterType(converter.GetType(), Format.GetType());

            TDst newFormat = converter.Convert((TSrc)Format);
            ChangeFormat(newFormat);

            return this;
        }
    }
}
