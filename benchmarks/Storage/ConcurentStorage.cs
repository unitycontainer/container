using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Unity.Benchmarks.Storage
{
    public class ConcurentStorage
    {
        private Type[] Data;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            Data = typeof(Type).Assembly
                               .DefinedTypes
                               .Take(100)
                               .ToArray();
        }


        [Benchmark]
        public int Benchmark()
        {
            ConcurrentDictionary<uint, Type> dictionary = new ConcurrentDictionary<uint, Type>();

            foreach(var data in Data)
            //for (var i = 0; i < Length; i++)
            {
                //var data = Data[i];
                dictionary[(uint)data.GetHashCode()] = data;
            }

            return dictionary.Count;
        }

        [Benchmark]
        [Arguments(1)]
        [Arguments(4)]
        [Arguments(8)]
        [Arguments(16)]
        [BenchmarkCategory("parallel")]
        public int ParallelBenchmark(int threads)
        {
            ConcurrentDictionary<uint, Type> dictionary = new ConcurrentDictionary<uint, Type>();

            Parallel.ForEach(Data, new ParallelOptions() { MaxDegreeOfParallelism = threads }, (data) => 
            {
                dictionary[(uint)data.GetHashCode()] = data;
            });

            return dictionary.Count;
        }

    }
}
