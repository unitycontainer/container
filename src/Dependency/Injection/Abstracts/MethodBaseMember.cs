using System;
using System.Linq;
using System.Reflection;

namespace Unity.Injection
{
    public abstract class MethodBaseMember<TMemberInfo> : InjectionMember<TMemberInfo, object[]>
                                      where TMemberInfo : MethodBase
    {
        #region Constructors

        protected MethodBaseMember(string name, params object[] arguments)
            : base(name, arguments)
        {
        }

        protected MethodBaseMember(TMemberInfo info, params object[] arguments)
            : base(info, MatchParameters(info, arguments) ?? 
                         throw new ArgumentException("Provided arguments do not match signature"))
        {
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


        #region Overrides

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

        protected override bool MatchMemberInfo(TMemberInfo info, object[] data)
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

        protected override void ValidateInjectionMember(Type type)
        {
            if (null != MemberInfo) return;

            // TODO:  Extended matching


            // If nothing helps throw

            // TODO: 5.9.0 Implement correct error message
            var signature = "xxx";//string.Join(", ", _arguments?.Select(t => t.Name) ?? );
            var message = $"The type {type.FullName} does not have a {typeof(TMemberInfo).Name} that takes these parameters ({signature}).";
            throw new InvalidOperationException(message);

        }

        #endregion


        #region Implementation

        protected static object[] MatchParameters(TMemberInfo info, params object[] arguments)
        {
            // TODO: Implement validation and extended matching
            return arguments;
        }

        protected static string Signature(object[] arguments)
        {
            // TODO: 5.9.0 Implement properly
            return "InjectionConstructor";
        }

        #endregion
    }
}
