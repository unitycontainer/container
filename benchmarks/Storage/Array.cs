using BenchmarkDotNet.Attributes;
using System;

namespace Unity.Benchmarks.Storage
{
    [BenchmarkCategory("Storage")]
    public class ArrayBenchmarks
    {
        [Params(10, 1000)]
        public int Size { get; set; }

        [Benchmark]
        public virtual object Array_new()
        {
            return new object[Size];
        }

        [Benchmark]
        public virtual object Array_Uninitialized()
        {
            return GC.AllocateUninitializedArray<object>(Size);
        }

        [Benchmark]
        public virtual object Array_Create()
        {
            return Array.CreateInstance(typeof(object), Size);
        }
    }
}
