using System;
using System.Threading;
#if UNITY_V4
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Regression
{
    public struct TestStruct
    {
        public int Integer;
        public object Instance;

        public TestStruct(int value, object instance)
        {
            Integer = value;
            Instance = instance;
        }
    }
}


