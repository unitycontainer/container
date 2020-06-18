using System;
using System.Collections.Generic;
using System.Reflection;


namespace Unity.Injection
{
    public abstract class MethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]>, 
                                                    IComparable<TMemberInfo>
                                where TMemberInfo : MethodBase
    {
        #region Fields

        internal static Func<TMemberInfo, bool> SupportedMembersFilter = 
            (TMemberInfo member) => !member.IsFamily && !member.IsPrivate;

        private TMemberInfo? _info;

        #endregion


        #region Constructors

        protected MethodBase(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        protected MethodBase(TMemberInfo info, params object[] arguments)
            : base((info ?? throw new ArgumentNullException(nameof(info))).Name, arguments)
        {
            _info = info;
        }

        #endregion


        #region Public Members

        public virtual TMemberInfo? MemberInfo() => _info;

        #endregion


        #region Overrides

        public abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        public override TMemberInfo? MemberInfo(Type type)
        {
            if (null != _info)
            { 
                return _info.DeclaringType == type
                    ? _info
                    : _info.GetMemberFromInfo(type);
            }

            var candidate = 0;
            TMemberInfo? selection = null;
            foreach (TMemberInfo member in DeclaredMembers(type))
            {
                var compatibility = CompareTo(member);
                
                if (0 == compatibility) return member;

                if (candidate < compatibility)
                { 
                    selection = member;
                    candidate = compatibility;
                }
            }

            return selection;
        }

        #endregion


        #region Matching

        public int CompareTo(TMemberInfo? other)
        {
            System.Diagnostics.Debug.Assert(null != other);

            var length = Data?.Length ?? 0;
            var parameters = other!.GetParameters();

            if (length != parameters.Length) return -1;

            int result = 0;
            for (var i = 0; i < length; i++)
            {
                var compatibility = Data![i].CompareTo(parameters[i]);

                if (-1 == compatibility) return -1;
                if (result < compatibility) result = compatibility;
            }

            return result;
        }

        #endregion
    }
}
