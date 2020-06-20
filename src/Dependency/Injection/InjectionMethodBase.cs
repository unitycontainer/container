using System;
using System.Collections.Generic;
using System.Reflection;


namespace Unity.Injection
{
    public abstract class InjectionMethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]>, 
                                                             IComparable<TMemberInfo>
                                         where TMemberInfo : MethodBase
    {
        #region Fields

        internal static Func<TMemberInfo, bool> SupportedMembersFilter = 
            (TMemberInfo member) => !member.IsFamily && !member.IsPrivate;

        #endregion


        #region Constructors

        protected InjectionMethodBase(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        #endregion


        #region Overrides

        public abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        public override TMemberInfo? MemberInfo(Type type)
        {
            int          bestSoFar = -1;
            TMemberInfo? candidate = null;
            
            foreach (TMemberInfo member in DeclaredMembers(type))
            {
                var compatibility = CompareTo(member);
                
                if (0 == compatibility) return member;

                if (bestSoFar < compatibility)
                { 
                    candidate = member;
                    bestSoFar = compatibility;
                }
            }

            return candidate;
        }

        #endregion


        #region Matching

        public int CompareTo(TMemberInfo? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = Data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int rank = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = (int)Data![i].MatchTo(parameters[i]);

                if (0 > compatibility) return -1;
                rank += compatibility;
            }

            return (int)MatchRank.ExactMatch * parameters.Length == rank ? 0 : rank;
        }

        #endregion
    }
}
