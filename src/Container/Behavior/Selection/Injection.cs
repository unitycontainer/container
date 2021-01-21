using System;
using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        public static int SelectInjectedConstructor(InjectionMember<ConstructorInfo, object[]> member, ConstructorInfo[] members, ref Span<int> indexes)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var compatibility = CompareTo(member.Data, members[index]);

                if (0 == compatibility) return index;

                if (bestSoFar < compatibility)
                {
                    position = index;
                    bestSoFar = compatibility;
                }
            }

            return position;
        }


        public static int SelectInjectedMethod(InjectionMember<MethodInfo, object[]> method, MethodInfo[] members, ref Span<int> indexes)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                if (method.Name != member.Name) continue;

                if (-1 == bestSoFar && method.Data is null)
                {   // If no data, match by name
                    bestSoFar = 0;
                    position = index;
                }

                // Calculate compatibility
                var compatibility = CompareTo(method.Data, member);
                if (0 == compatibility) return index;

                if (bestSoFar < compatibility)
                {
                    position = index;
                    bestSoFar = compatibility;
                }
            }

            return position;
        }


        private static int SelectInjectedField(InjectionMember<FieldInfo, object> field, FieldInfo[] members, ref Span<int> indexes)
        {
            for (var index = 0; index < members.Length; index++)
            {
                if (field.Name == members[index].Name) return index;
            }

            return -1;
        }


        public static int SelectInjectedProperty(InjectionMember<PropertyInfo, object> field, PropertyInfo[] members, ref Span<int> indexes)
        {
            for (var index = 0; index < members.Length; index++)
            {
                if (field.Name == members[index].Name) return index;
            }

            return -1;
        }
    }
}
