using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// An <see cref="InjectionMember"/> that configures the
    /// container to call a method as part of buildup.
    /// </summary>
    public class InjectionMethod : InjectionMethodBase<MethodInfo>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InjectionMethod"/> instance which will configure
        /// the container to call the given method with the given parameters.
        /// </summary>
        /// <param name="methodName">Name of the method to call.</param>
        /// <param name="arguments">Parameter values for the method.</param>
        public InjectionMethod(string methodName, params object[] arguments)
            : base(methodName, arguments)
        {
        }

        #endregion


        #region Matching

        public override int SelectFrom(MethodInfo[] members)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                if (Name != member.Name) continue;

                if (-1 == bestSoFar && 0 == Data!.Length)
                {   // If no data, match by name
                    bestSoFar = 0;
                    position = index;
                }

                // Calculate compatibility
                var compatibility = CompareTo(member);
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
