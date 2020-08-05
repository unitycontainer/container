using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.BuiltIn;
using Unity.Lifetime;

namespace Unity.Benchmarks.Scope
{
    [LongRunJob]
    public class ScopeBenchmarks
    {
        static Unity.Container.Scope Scope;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Scope = new ContainerScope(100);
        }
    }
}
//|   Method |     Mean |    Error |   StdDev |   Median |
//|--------- |---------:|---------:|---------:|---------:|
//|      Get | 35.41 ns | 0.042 ns | 0.208 ns | 35.38 ns |
//| GetStack | 35.54 ns | 0.048 ns | 0.239 ns | 35.52 ns |

//|   Method |     Mean |     Error |    StdDev |   Median |
//|--------- |---------:|----------:|----------:|---------:|
//|      Get | 3.890 ns | 0.0103 ns | 0.0510 ns | 3.904 ns |
//| GetStack | 3.707 ns | 0.0072 ns | 0.0358 ns | 3.705 ns |

