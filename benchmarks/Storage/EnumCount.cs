using BenchmarkDotNet.Attributes;
using System;

namespace Unity.Benchmarks.Storage
{
    [BenchmarkCategory("Storage")]
    public class EnumBenchmarks
    {
        [Benchmark]
        public int DeclaredFields()
        {
            return typeof(TestEnum).GetFields().Length;
        }

        [Benchmark]
        public int GetNames()
        {
            return Enum.GetNames(typeof(TestEnum)).Length;
        }


        [Benchmark]
        public int GetValues()
        {
            return Enum.GetValues(typeof(TestEnum)).Length;
        }

        public enum TestEnum : int
        {
            Zero,
            One,
            Two,
            Three,
            Four
        }
    }
}
