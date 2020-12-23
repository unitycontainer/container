using Microsoft.VisualStudio.TestTools.UnitTesting;
using Benchmark.Tests;
using Unity.Benchmarks;

namespace Continer
{
    [TestClass]
    public partial class TestCreate : BenchmarkTestBase<Create>
    {
        [TestMethod]
        public void New_UnityContainer() 
            => Assert.IsNotNull(Benchmark.New_UnityContainer());

        [TestMethod]
        public void CreateChildContainer() 
            => Assert.IsNotNull(Benchmark.CreateChildContainer());
    }
}
