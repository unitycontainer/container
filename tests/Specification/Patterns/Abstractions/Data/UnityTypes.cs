using System;
using System.Collections.Generic;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Regression
{
    public abstract partial class PatternBase
    {
        public static IEnumerable<Type> Unity_BuiltIn_Types
        {
            get
            {
                yield return typeof(IUnityContainer);
#if !UNITY_V4 && !UNITY_V5
                yield return typeof(IUnityContainerAsync);
                yield return typeof(IServiceProvider);
#endif
            }
        }

        public static IEnumerable<Type> Unity_Unrecognized_Types
        {
            get
            {
                yield return typeof(Unresolvable);
                yield return typeof(string);

#if !BEHAVIOR_V4 // Unity v4 did not support optional value types
                yield return typeof(int);
                yield return typeof(bool);
                yield return typeof(long);
                yield return typeof(short);
                yield return typeof(float);
                yield return typeof(double);
                // TODO: typeof(TestStruct)
#endif
                yield return typeof(List<>);
                yield return typeof(Type);
                yield return typeof(ICloneable);
                yield return typeof(Delegate);
            }
        }


        public static IEnumerable<Type> Unity_Recognized_Types
        {
            get
            {
                yield return typeof(object);
                yield return typeof(Lazy<IUnityContainer>);
                yield return typeof(Func<IUnityContainer>);
                yield return typeof(object[]);
#if !BEHAVIOR_V4
                yield return typeof(List<int>);
                yield return typeof(List<string>);
                yield return typeof(IEnumerable<IUnityContainer>);
#endif
            }
        }
    }
}
