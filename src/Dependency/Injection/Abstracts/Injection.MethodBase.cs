using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    public abstract class InjectionMethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]>, 
                                                             IComparable<TMemberInfo>
                                         where TMemberInfo : MethodBase
    {
        #region Constructors

        protected InjectionMethodBase(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        #endregion


        #region Selection

        public virtual int SelectFrom(TMemberInfo[] members)
        {
            int position = -1;
            int bestSoFar = -1;

            for (var index = 0; index < members.Length; index++)
            {
                var compatibility = CompareTo(members[index]);

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


        #region Info

        public InvokeInfo<TMemberInfo> GetInvocationInfo(TMemberInfo member)
        {
            var parameters = member.GetParameters();

            if (0 == parameters.Length) return new InvokeInfo<TMemberInfo>(member);

            var imports = new InjectionInfo<ParameterInfo>[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
            {
                imports[i] = parameters[i].AsInjectionInfo(Data![i]);
            }

            return new InvokeInfo<TMemberInfo>(member, imports);
        }

        #endregion
    }
}
