using Microsoft.VisualStudio.TestTools.UnitTesting;
using Benchmark.Tests;
#if UNITY_V4
using Unity.v4;
#elif UNITY_V5
using Unity.v5;
#else
using Unity.v6;
#endif


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
