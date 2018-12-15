using System;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember<TMemberInfo, object[]>
                                      where TMemberInfo : MethodBase
    {
        #region Constructors

        protected MethodBaseMember(params object[] arguments)
            : base(arguments)
        {
        }

        #endregion


        #region Implementation

        public override (TMemberInfo, object[]) Select(Type type)
        {
            var methodHasOpenGenericParameters = MemberInfo.GetParameters()
                                                     .Select(p => p.ParameterType.GetTypeInfo())
                                                     .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var info = MemberInfo.DeclaringType.GetTypeInfo();
            if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
                return (MemberInfo, Data);

#if NETSTANDARD1_0
            var typeInfo = type.GetTypeInfo();
            var parameterTypes = MemberInfo.GetParameters()
                                           .Select(pi => pi.ParameterType.GetClosedParameterType(typeInfo.GenericTypeArguments))
                                           .ToArray();
            var member = DeclaredMembers(type).Single(m => m.Name.Equals(MemberInfo.Name) && ParametersMatch(m.GetParameters(), parameterTypes));
            if (null != member) return (member, Data);

            bool ParametersMatch(ParameterInfo[] parameters, Type[] closedConstructorParameterTypes)
            {
                if ((parameters ?? throw new ArgumentNullException(nameof(parameters))).Length !=
                    (closedConstructorParameterTypes ?? throw new ArgumentNullException(nameof(closedConstructorParameterTypes))).Length)
                {
                    return false;
                }

                return !parameters.Where((t, i) => !t.ParameterType.Equals(closedConstructorParameterTypes[i])).Any();
            }

#else
            foreach (var member in DeclaredMembers(type))
            {
                if (member.MetadataToken == MemberInfo.MetadataToken)
                    return (member, Data);
            }
#endif
            // TODO: 5.9.0 Implement correct error message
            throw new InvalidOperationException("No such member");
        }

        protected override bool MemberInfoMatch(TMemberInfo info, object[] data)
        {
            var parameters = info.GetParameters();

            // TODO: optimize
            if ((data?.Length ?? 0) != parameters.Length) return false;

            for (var i = 0; i < (data?.Length ?? 0); i++)
            {
                if (Matches(data?[i], parameters[i].ParameterType))
                    continue;

                return false;
            }

            return true;
        }

        #endregion


        #region Signature matching

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
    }
}
