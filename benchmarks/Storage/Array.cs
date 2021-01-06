using BenchmarkDotNet.Attributes;
using System;
using Unity.Extension;

namespace Unity.Benchmarks.Storage
{
    [BenchmarkCategory("Storage")]
    public class ArrayBenchmarks
    {
        [Benchmark]
        public virtual object Test_ImportData()
        {
            var result = FromImportData();

            return result.IsValue ? result.Value : null;
        }

        [Benchmark]
        public virtual object Test_Out()
        {
            var result = FromOut(out var data);

            return result ? data : null;
        }


        ImportData FromImportData() => new ImportData(0, ImportType.Value);


        bool FromOut(out object value)
        {
            value = 0;
            return true;
        }

    }
}
