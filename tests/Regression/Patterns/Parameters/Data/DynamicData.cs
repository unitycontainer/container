using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
#endif

namespace Parameters
{
    public abstract partial class Pattern
    {
        #region Baseline Test Type

        public static IEnumerable<object[]> Parameters_Test_Data
        {
            get
            {
                var @namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                foreach (var member in MemberInfo_Namespace_Names)
                {
                    var method = Type.GetType($"{typeof(Regression.PatternBase).FullName}+{member}")
                                     .GetMethod("GetInjectionValue")
                                     .CreateDelegate(typeof(Func<object, InjectionMember>));

                    foreach (var annotation in Annotation_Category_Names)
                    {
                        foreach (var nameInfo in BaselineTestType_Names)
                        {
                            var definition = GetTestType(nameInfo.Item1, annotation, member, @namespace);

                            if (definition is null) continue;

                            foreach (var set in Test_Data_Set)
                            {
#if BEHAVIOR_V4
                                if (set.Type.IsValueType) continue;
#endif
                                yield return new object[]
                                {
                                    set.Type,           // Type
                                    definition,         // Test Type Definition
                                    member,             // Constructors, Methods, etc.
                                    annotation,         // Implicit, Optional, Required
                                    method,             // Injection Method
                                    set.Registered,     // Registered
                                    set.Named,          // Named
                                    set.Injected,       // Injected
                                    set.Default,        // default
                                    nameInfo.Item2      // Is Named
                                };
                            }
                        }
                    }
                }
            }
        }

        #endregion


        #region Arrays

        public static IEnumerable<object[]> Array_Parameters_Data
        {
            get
            {
                var @namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                foreach (var member in MemberInfo_Namespace_Names)
                {
                    var method = Type.GetType($"{typeof(Regression.PatternBase).FullName}+{member}")
                                     .GetMethod("GetInjectionValue")
                                     .CreateDelegate(typeof(Func<object, InjectionMember>));

                    foreach (var annotation in Annotation_Category_Names)
                    {
                        foreach (var nameInfo in BaselineArrayType_Names)
                        {
                            var definition = GetTestType(nameInfo.Item1, annotation, member, @namespace);

                            if (definition is null) continue;

                            foreach (var set in Test_Data_Set)
                            {
                                yield return new object[]
                                {
                                    set.Type,           // Type
                                    definition,         // Test Type Definition
                                    member,             // Constructors, Methods, etc.
                                    annotation,         // Implicit, Optional, Required
                                    method,             // Injection Method
                                    set.Registered,     // Registered
                                    set.Named,          // Named
                                    set.Injected,       // Injected
                                    set.Default,        // default
                                    nameInfo.Item2      // Is Named
                                };
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
