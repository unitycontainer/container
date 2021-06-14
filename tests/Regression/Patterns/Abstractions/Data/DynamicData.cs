using System.Linq;
using System.Collections.Generic;
using System;

namespace Regression
{
    public abstract partial class PatternBase
    {
        public static IEnumerable<object[]> Test_Type_Data 
            => Test_Data_Set.Select(set => (object[])set);

        public static IEnumerable<object[]> Type_Compatibility_Data
#if BEHAVIOR_V4
            => Test_Type_Data.Where(set => set[1] is Type type && !type.IsValueType);
#else
            => Test_Type_Data;
#endif

        public static IEnumerable<object[]> BuiltInTypes_Data 
            => Unity_BuiltIn_Types.Select(type => new object[] { type.Name, type });

        public static IEnumerable<object[]> Unity_Recognized_Types_Data
            => Unity_Recognized_Types.Select(type => new object[] { type.Name, type });

        public static IEnumerable<object[]> Unity_Unrecognized_Types_Data 
            => Unity_Unrecognized_Types.Where(type => !type.IsGenericTypeDefinition)
                                       .Select(type => new object[] { type.Name, type });

        public static IEnumerable<object[]> InvalidTypes_Data 
            => Unity_Unrecognized_Types.Select(type => new object[] { type.Name, type });
    }
}

