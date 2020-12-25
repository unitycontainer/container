using BenchmarkDotNet.Attributes;
using Benchmarks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pipeline
{
    public partial class TestPipelineBenchmarks
    {
        [TestMethod]
        public void Pipeline_Create_Unknown()
            => Assert.IsNotNull(Benchmark.Pipeline_Create_Unknown());

        [TestMethod]
        public void Pipeline_Create_Registered()
            => Assert.IsNotNull(Benchmark.Pipeline_Create_Registered());
    }
}
