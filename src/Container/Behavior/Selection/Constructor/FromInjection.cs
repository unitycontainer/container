using System.Reflection;
using Unity.Injection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        #region Selection


        public static int ConstructorInfoFromInjected(InjectionMethodBase<ConstructorInfo> member, MethodBase[] members)
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


        #endregion
    }
}
