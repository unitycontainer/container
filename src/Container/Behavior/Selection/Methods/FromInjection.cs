﻿using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class DefaultSelectors
    {
        #region Selection


        public static int MethodInfoFromInjected(InjectionMethodBase<MethodInfo> method, MethodBase[] members)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                if (method.Name != member.Name) continue;

                if (-1 == bestSoFar && 0 == method.Data!.Length)
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


        #endregion
    }
}
