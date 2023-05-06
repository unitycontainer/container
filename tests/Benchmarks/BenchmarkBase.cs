using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class BenchmarkBase
    {
#if UNITY_V4
        public const string VERSION = " v4";
#elif UNITY_V5
        public const string VERSION = " v5";
#elif UNITY_V6
        public const string VERSION = " v6";
#else
        public const string VERSION = "";
#endif
    }
}
