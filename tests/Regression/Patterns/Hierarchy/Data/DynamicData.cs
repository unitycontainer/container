using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity.Injection;
#endif

namespace Container
{
    public abstract partial class Pattern
    {
        #region Baseline Test Type

        public static IEnumerable<object[]> Hierarchy_Test_Data
        {
            get
            {
                var @namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                foreach (var member in MemberInfo_Namespace_Names)
                {
                    foreach (var annotation in Annotation_Category_Names)
                    {
                        // Exclude implicit fields and properties
                        if (IMPORT_IMPLICIT.Equals(annotation)) continue;

                        foreach (var nameInfo in BaselineTestType_Names)
                        {
                            var definition = GetTestType(nameInfo.Item1, annotation, member, @namespace);

                            yield return new object[]
                            {
                                definition.Name,
                                definition,         // Test Type Definition
                                nameInfo.Item2      // Is Named
                            };
                        }
                    }
                }
            }
        }


        public static IEnumerable<object[]> Hierarchy_Unnamed_Data
        {
            get
            {
                var @namespace = MethodBase.GetCurrentMethod().DeclaringType.Namespace;
                foreach (var member in MemberInfo_Namespace_Names)
                {
                    foreach (var annotation in Annotation_Category_Names)
                    {
                        // Exclude implicit fields and properties
                        if (IMPORT_IMPLICIT.Equals(annotation)) continue;

                        var definition = GetTestType("BaselineTestType`2", annotation, member, @namespace);

                        yield return new object[] { definition };
                    }
                }
            }
        }

        #endregion
    }
}
