using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Threading;

namespace Unity.Benchmarks.Storage
{
    [ShortRunJob]
    public class Access1
    {
        private object NullValue = null;
        private object Value = new object();
        private Type[] Data;
        private ThreadLocal<Type[]> threadLocal;
        private AsyncLocal<Type[]>  AsyncLocal;
        
        [GlobalSetup]
        public void GlobalSetup()
        {
            Data = typeof(Type).Assembly
                               .DefinedTypes
                               .Take(100)
                               .ToArray();

            threadLocal = new ThreadLocal<Type[]>();
            AsyncLocal = new AsyncLocal<Type[]>();
        }


        [Benchmark]
        public object ThreadLocalGet()
        {
            return threadLocal.Value;
        }

        [Benchmark]
        public object ThreadLocalSet()
        {
            threadLocal.Value = Data;
            return threadLocal.Value;
        }

        [Benchmark]
        public object AsyncLocalGet()
        {
            return AsyncLocal.Value;
        }

        [Benchmark]
        public object AsyncLocalSet()
        {
            AsyncLocal.Value = Data;
            return AsyncLocal.Value;
        }
        /*
        |                  Method |     Mean |     Error |    StdDev |
        |------------------------ |---------:|----------:|----------:|
        |          IsNullPositive | 3.478 ns | 6.0999 ns | 0.3344 ns |
        |          IsNullNegative | 2.596 ns | 0.0088 ns | 0.0005 ns |
        |    IsNullEqualsPositive | 4.096 ns | 2.5911 ns | 0.1420 ns |
        |    IsNullEqualsNegative | 2.583 ns | 0.1992 ns | 0.0109 ns |
        | IsNullReferencePositive | 2.625 ns | 0.5162 ns | 0.0283 ns |
        | IsNullReferenceNegative | 3.638 ns | 4.4972 ns | 0.2465 ns |

        |                  Method |     Mean |      Error |    StdDev |
        |------------------------ |---------:|-----------:|----------:|
        |          IsNullPositive | 3.322 ns |  3.7914 ns | 0.2078 ns |
        |          IsNullNegative | 2.620 ns |  0.2892 ns | 0.0158 ns |
        |    IsNullEqualsPositive | 2.556 ns |  0.8518 ns | 0.0467 ns |
        |    IsNullEqualsNegative | 4.020 ns |  2.2082 ns | 0.1210 ns |
        | IsNullReferencePositive | 2.584 ns |  0.6802 ns | 0.0373 ns |
        | IsNullReferenceNegative | 2.975 ns | 12.3169 ns | 0.6751 ns |

        |                  Method |     Mean |     Error |    StdDev |
        |------------------------ |---------:|----------:|----------:|
        |          IsNullNegative | 2.611 ns | 0.0765 ns | 0.0042 ns |
        |          IsNullPositive | 2.664 ns | 0.9897 ns | 0.0542 ns |
        |    IsNullEqualsPositive | 2.556 ns | 0.1839 ns | 0.0101 ns |
        |    IsNullEqualsNegative | 3.942 ns | 1.1830 ns | 0.0648 ns |
        | IsNullReferencePositive | 2.569 ns | 0.4384 ns | 0.0240 ns |
        | IsNullReferenceNegative | 2.584 ns | 0.6566 ns | 0.0360 ns |
        */
        [Benchmark]
        public object IsNullNegative()
        {
            return Value is null;
        }

        [Benchmark]
        public object IsNullPositive()
        {
            return NullValue is null;
        }

        [Benchmark]
        public object IsNullEqualsPositive()
        {
            return NullValue == null;
        }
        
        [Benchmark]
        public object IsNullEqualsNegative()
        {
            return Value == null;
        }

        [Benchmark]
        public object IsNullReferencePositive()
        {
            return ReferenceEquals(NullValue, null);
        }

        [Benchmark]
        public object IsNullReferenceNegative()
        {
            return ReferenceEquals(Value, null);
        }

        Type type = typeof(BenchmarkAttribute);

        [Benchmark]
        public object TypeReferenceEquals()
        {
            return ReferenceEquals(type, typeof(BenchmarkAttribute));
        }

        [Benchmark]
        public object TypeReferenceEqualsNegative()
        {
            return ReferenceEquals(type, typeof(Access1));
        }


        [Benchmark]
        public object TypeEquals()
        {
            return typeof(BenchmarkAttribute).TypeHandle.Equals(type.TypeHandle);
        }

        [Benchmark]
        public object TypeEqualsNegative()
        {
            return typeof(Access1).TypeHandle.Equals(type.TypeHandle);
        }

        [Benchmark]
        public object TypeHandleEquals()
        {
            return type.TypeHandle.Value == typeof(BenchmarkAttribute).TypeHandle.Value;
        }

        [Benchmark]
        public object TypeHandleNegative()
        {
            return type.TypeHandle.Value == typeof(Access1).TypeHandle.Value;
        }

    }
}
