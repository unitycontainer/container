using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity.Injection
{
    public delegate bool MatchPredicate(object[] arguments, ParameterInfo[] parameters);

    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember, IEquatable<TMemberInfo>
                                      where TMemberInfo : MethodBase
    {
        #region Fields

        private readonly object[] _arguments;
        protected abstract string Designation { get; }

        #endregion


        #region Constructors

        protected MethodBaseMember(params object[] arguments)
        {
            _arguments = arguments ?? new object[0];
        }

        #endregion


        #region Public Members

        public TMemberInfo Info
        {
            get;
            protected set;
        }

        public abstract TMemberInfo GetInfo(Type type);

        public virtual object[] GetParameters() => _arguments;

        #endregion


        #region IEquatable

        public abstract bool Equals(TMemberInfo other);

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

                if (null != Info)
                {
                    var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
                    var message = $"The type {mappedToType.FullName} does not have a {Designation} that takes these parameters ({signature}).";
                    throw new InvalidOperationException(message);
                }

                Info = member;
            }

            if (null == Info)
            {
                var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
                var message = $"The type {mappedToType.FullName} does not have a {Designation} that takes these parameters ({signature}).";
                throw new InvalidOperationException(message);
            }
        }

        #endregion


        #region Type matching

        protected virtual bool Matches(object parameter, Type match)
        {
            switch (parameter)
            {
                // TODO: Replace with IEquatable
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

        protected virtual object[] ParametersFromArguments(object[] arguments, ParameterInfo[] parameters)
        {
            return null;
        }

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

        protected abstract IEnumerable<TMemberInfo> DeclaredMembers(TypeInfo info);

        #endregion
    }
}
