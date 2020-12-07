﻿using System;
using System.Reflection;

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


        #region Matching

        public override MatchRank Match(TMemberInfo other)
            => throw new System.NotImplementedException();

        public abstract int SelectFrom(TMemberInfo[] members);

        // TODO: Integrate with match
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

        // TODO: Selection ?
        public override void GetImportInfo<TImport>(ref TImport import)
            => throw new System.NotImplementedException();
    }
}
