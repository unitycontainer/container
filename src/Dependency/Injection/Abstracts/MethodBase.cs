using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MethodBase<TMemberInfo> : InjectionMember<TMemberInfo, object[]>
                                where TMemberInfo : MethodBase
    {
        #region Constructors

        protected MethodBase(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        protected MethodBase(TMemberInfo info, params object[] arguments)
            : base(info, arguments)
        {
        }


        #endregion


        #region Overrides

        public abstract IEnumerable<TMemberInfo> DeclaredMembers(Type type);

        protected override TMemberInfo DeclaredMember(Type type)
        {

            throw new NotImplementedException();
        }

        public override TMemberInfo? MemberInfo(Type type)
        {
            if (null != Info)
                return type == Info.DeclaringType ? Info : DeclaredMember(type);

            foreach (var member in DeclaredMembers(type))
            {
                if (!Match(member)) continue;
                //if (!Data.MatchMemberInfo(member)) continue;

                return member;
            }

            return null;
        }

        #endregion


        #region IMatch

        public override bool Match(TMemberInfo other)
        {
            var length = Data?.Length ?? 0;
            var parameters = other.GetParameters();

            if (length != parameters.Length) return false;

            for (var i = 0; i < length; i++)
            {
                var parameter = parameters[i];
                var value     = Data![i];
                var match     = value switch
                {
#if NETSTANDARD1_6 || NETCOREAPP1_0
                    null => !parameter.ParameterType.GetTypeInfo().IsValueType || 
                            (null != Nullable.GetUnderlyingType(parameter.ParameterType)),
#else
                    null => !parameter.ParameterType.IsValueType || 
                            (null != Nullable.GetUnderlyingType(parameter.ParameterType)),
#endif
                    IMatch<ParameterInfo> matchParam
                              => matchParam.Match(parameter),

                    IMatch<Type> matchType
                              => matchType.Match(parameter.ParameterType),

                    Type _ when typeof(Type).Equals(parameter.ParameterType)
                              => true,

                    Type type => MatchesType(type, parameter.ParameterType),
                    _         => MatchesObject(value, parameter.ParameterType),
                };

                if (match) continue;

                return false;
            }

            return true;
        }

        #endregion
    }
}
