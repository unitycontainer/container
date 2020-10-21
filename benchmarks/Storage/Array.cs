using BenchmarkDotNet.Attributes;
using System;
using System.Linq;

namespace Unity.Benchmarks.Storage
{
    [BenchmarkCategory("Storage")]
    [MemoryDiagnoser]
    public class ArrayBenchmarks
    {
        private Type[] Data;
        private object[] DataCopy;

        private Type[] Data1;
        private Type[] Data2;
        private Type[] Data3;
        private Type[] Data4;
        private Type[] Data5;

        [GlobalSetup]
        public void GlobalSetup()
        {
            Data = typeof(Type).Assembly
                               .DefinedTypes
                               .Take(500)
                               .ToArray();
        }

        //[IterationSetup]
        public void IterationSetup()
        {
            DataCopy = new object[5];
            Data1 = typeof(Type).Assembly.DefinedTypes.Take(500).ToArray();
            Data2 = typeof(Type).Assembly.DefinedTypes.Take(500).ToArray();
            Data3 = typeof(Type).Assembly.DefinedTypes.Take(500).ToArray();
            Data4 = typeof(Type).Assembly.DefinedTypes.Take(500).ToArray();
            Data5 = typeof(Type).Assembly.DefinedTypes.Take(500).ToArray();
        }

        [Benchmark]
        public object ArrayResize()
        {
            var size = Data.Length * 200;
            Array.Resize(ref Data1, size);

            return Data1;
        }


        [Benchmark]
        public object ArrayResizeMultiple()
        {
            var size = Data.Length * 200;

            Array.Resize(ref Data1, size + 1);
            Array.Resize(ref Data2, size + 2);
            Array.Resize(ref Data3, size + 3);
            Array.Resize(ref Data4, size + 4);
            Array.Resize(ref Data5, size + 5);

            return null;
        }

        [Benchmark]
        public object ArrayCopy()
        {
            var size = Data.Length * 200;
            var array = new Type[size];

            Array.Copy(Data, array, Data.Length);

            return array;
        }

        [Benchmark]
        public object ArrayCopyMultiple()
        {
            var size = Data.Length * 200;

            DataCopy[0] = new Type[size + 1];
            Array.Copy(Data, (Type[])DataCopy[0], Data.Length);

            DataCopy[1] = new Type[size + 2];
            Array.Copy(Data, (Type[])DataCopy[1], Data.Length);

            DataCopy[2] = new Type[size + 3];
            Array.Copy(Data, (Type[])DataCopy[2], Data.Length);

            DataCopy[3] = new Type[size + 4];
            Array.Copy(Data, (Type[])DataCopy[3], Data.Length);

            DataCopy[4] = new Type[size + 5];
            Array.Copy(Data, (Type[])DataCopy[4], Data.Length);

            return null;
        }

        [Benchmark]
        public virtual object Array_new()
        {
            return new object[10];
        }

        [Benchmark]
        public virtual object Array_Create()
        {
            return Array.CreateInstance(typeof(object), 10);
        }
    }
}
