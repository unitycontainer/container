using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember, IEquatable<TMemberInfo>
                                      where TMemberInfo : MethodBase
    {
        #region Fields

        private readonly object[] _parameters;

        #endregion


        #region Constructors

        protected MethodBaseMember(params object[] parameters)
        {
            _parameters = parameters;
        }

        #endregion


        #region Public Members

        public TMemberInfo Info
        {
            get;
            protected set;
        }

        public abstract TMemberInfo GetInfo(Type type);

        public abstract object[] GetParameters();

        #endregion


        #region IEquatable

        public abstract bool Equals(TMemberInfo other);

        #endregion


        #region InjectionMember

        public override void AddPolicies<TContext, TPolicyList>(Type registeredType, Type mappedToType, string name, ref TPolicyList policies)
        {
            if (null == Info) return;
            OnType(mappedToType ?? registeredType ?? throw new ArgumentNullException(nameof(registeredType)));
        }

        public override InjectionMember OnType(Type targetType)
        {
            var info = targetType?.GetTypeInfo() ?? throw new ArgumentNullException(nameof(targetType));
            var matches = DeclaredMembers(info).Where(ParametersMatch)
                                               .ToArray();

            return base.OnType(targetType);
        }

        #endregion


        #region Type matching

        protected virtual bool Matches(ParameterInfo[] parameters)
        {
            // TODO: optimize
            if ((_parameters?.Length ?? 0) != parameters.Length) return false;

            for (var i = 0; i < (_parameters?.Length ?? 0); i++)
            {
                if (Matches(_parameters?[i], parameters[i].ParameterType))
                    continue;

                return false;
            }

            return true;
        }

        protected virtual bool Matches(object parameter, Type match)
        {
            switch (parameter)
            {
                case InjectionParameter injectionParameter:
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

        protected virtual bool ParametersMatch(MethodBase info)
        {
            return Matches(info.GetParameters());
        }

        #endregion
    }
}
