using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;

namespace Unity.Injection
{
    public delegate bool MatchPredicate(object[] arguments, ParameterInfo[] parameters);

    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember,
                                                          ISelect<TMemberInfo, object[]>,
                                                          IEquatable<TMemberInfo>
                                      where TMemberInfo : MethodBase
    {
        #region Fields

        private readonly object[] _arguments;

        #endregion


        #region Constructors

        protected MethodBaseMember(params object[] arguments)
        {
            _arguments = arguments ?? new object[0];
        }

        #endregion


        #region Protected Members

        protected TMemberInfo MemberInfo;

        #endregion


        #region InjectionMember

        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            var typeInfo = mappedToType.GetTypeInfo();
            var predicate = 0 == _arguments.Length
                ? (MatchPredicate)((_, parameters) => 0 == parameters.Length)
                : /* policies.Get<MatchPredicate>() ?? */MatchArgumentsToParameters;

            foreach (var member in DeclaredMembers(typeInfo))
            {
                if (!predicate(_arguments, member.GetParameters())) continue;
                //var parameters = ParametersFromArguments(_arguments, member.GetParameters());
                //if (null == parameters) continue;

                if (null != MemberInfo)
                {
                    var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
                    var message = $"The type {mappedToType.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
                    throw new InvalidOperationException(message);
                }

                MemberInfo = member;
            }

            Validate(mappedToType);
        }

        #endregion


        #region ISelect

        public virtual (TMemberInfo, object[]) Select(Type type)
        {
            var methodHasOpenGenericParameters = MemberInfo.GetParameters()
                                                     .Select(p => p.ParameterType.GetTypeInfo())
                                                     .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var info = MemberInfo.DeclaringType.GetTypeInfo();
            if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
                return (MemberInfo, _arguments);

#if NETSTANDARD1_0
            var typeInfo = type.GetTypeInfo();
            var parameterTypes = Info.GetClosedParameterTypes(typeInfo.GenericTypeArguments);
            var member = DeclaredMembers(typeInfo).Single(m => m.Name.Equals(Info.Name) &&
                                                               m.GetParameters().ParametersMatch(parameterTypes));
#else
            foreach (var member in DeclaredMembers(type.GetTypeInfo()))
            {
                if (member.MetadataToken == MemberInfo.MetadataToken)
                    return (member, _arguments);
            }
#endif
            // TODO: 5.9.0 Implement correct error message
            throw new InvalidOperationException("No such member");
        }

        #endregion


        #region IEquatable

        public virtual bool Equals(TMemberInfo other)
        {
            return other?.MetadataToken == MemberInfo.MetadataToken;
        }

        #endregion


        #region Type matching

        protected virtual bool MatchArgumentsToParameters(object[] arguments, ParameterInfo[] parameters)
        {
            // TODO: optimize
            if ((_arguments?.Length ?? 0) != parameters.Length) return false;

            for (var i = 0; i < (_arguments?.Length ?? 0); i++)
            {
                if (Matches(_arguments?[i], parameters[i].ParameterType))
                    continue;

                return false;
            }

            return true;
        }

        protected virtual bool Matches(object parameter, Type match)
        {
            switch (parameter)
            {
                // TODO: 5.9.0 Replace with IEquatable
                case InjectionParameterValue injectionParameter:
                    return injectionParameter.MatchesType(match);

                case Type type:
                    return MatchesType(type, match);

                default:
                    return MatchesObject(parameter, match);
            }
        }

        protected static bool MatchesType(Type type, Type match)
        {
            if (null == type) return true;

            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            if (matchInfo.IsAssignableFrom(typeInfo)) return true;
            if ((typeInfo.IsArray || typeof(Array) == type) &&
               (matchInfo.IsArray || match == typeof(Array)))
                return true;

            if (typeInfo.IsGenericType && typeInfo.IsGenericTypeDefinition && matchInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == matchInfo.GetGenericTypeDefinition())
                return true;

            return false;
        }

        protected static bool MatchesObject(object parameter, Type match)
        {
            var type = parameter is Type ? typeof(Type) : parameter?.GetType();

            if (null == type) return true;

            var typeInfo = type.GetTypeInfo();
            var matchInfo = match.GetTypeInfo();

            if (matchInfo.IsAssignableFrom(typeInfo)) return true;
            if ((typeInfo.IsArray || typeof(Array) == type) &&
                (matchInfo.IsArray || match == typeof(Array)))
                return true;

            if (typeInfo.IsGenericType && typeInfo.IsGenericTypeDefinition && matchInfo.IsGenericType &&
                typeInfo.GetGenericTypeDefinition() == matchInfo.GetGenericTypeDefinition())
                return true;

            return false;
        }

        #endregion


        #region Implementation

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(TypeInfo info);

        protected virtual void Validate(Type type)
        {
            if (null != MemberInfo) return;

            // TODO: 5.9.0 Implement correct error message
            var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
            var message = $"The type {type.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
            throw new InvalidOperationException(message);
        }

        #endregion
    }
}
