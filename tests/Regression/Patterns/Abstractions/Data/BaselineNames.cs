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
        public static IEnumerable<string> MemberInfo_Namespace_Names
        {
            get
            {
                yield return CONSTRUCTORS;
                yield return METHODS;
#if !UNITY_V4
                yield return FIELDS;
#endif
                yield return PROPERTIES;
            }
        }

        public static IEnumerable<string> Annotation_Category_Names
        {
            get
            {
                yield return IMPORT_IMPLICIT;
                yield return IMPORT_REQUIRED;
                yield return IMPORT_OPTIONAL;
            }
        }

        public static IEnumerable<(string, bool)> BaselineTestType_Names
        {
            get
            {
                yield return ("BaselineTestType`1", false);
                yield return ("BaselineTestTypeNamed`1", true);
            }
        }

        public static IEnumerable<(string, bool)> BaselineArrayType_Names
        {
            get
            {
                yield return ("BaselineArrayType`1", false);
            }
        }
    }
}
