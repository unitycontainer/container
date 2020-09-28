using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A class that holds the collection of information
    /// for a constructor, so that the container can
    /// be configured to call this constructor.
    /// </summary>
    public class InjectionConstructor : InjectionMethodBase<ConstructorInfo>
    {
        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="InjectionConstructor"/> that looks
        /// for a constructor with the given set of parameters.
        /// </summary>
        /// <param name="arguments">The values for the constructor's parameters, that will
        /// be used to create objects.</param>
        public InjectionConstructor(params object[] arguments)
            : base(".ctor", arguments)
        {
        }

        #endregion


        #region Overrides

        // TODO: Remove
        public override IEnumerable<ConstructorInfo> DeclaredMembers(Type type) => 
            type.GetConstructors(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance)
                .Where(SupportedMembersFilter);

        public override SelectionInfo<ConstructorInfo, object[]> SelectMember(ConstructorInfo[] members)
        {
            int bestSoFar = -1;
            ConstructorInfo? candidate = null;

            foreach (ConstructorInfo member in members)
            {
                var compatibility = CompareTo(member);

                if (0 == compatibility) return new SelectionInfo<ConstructorInfo, object[]>(member, Data);

                if (bestSoFar < compatibility)
                {
                    candidate = member;
                    bestSoFar = compatibility;
                }
            }

            // TODO: fix
            return new SelectionInfo<ConstructorInfo, object[]>(candidate, Data);
        }

        #endregion

    }
}
