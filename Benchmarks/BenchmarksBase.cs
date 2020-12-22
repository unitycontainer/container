using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY_V4
namespace Unity.v4
#elif UNITY_V5
namespace Unity.v5
#else
namespace Unity.v6
#endif
{
    public class BenchmarksBase
    {
    }
}
