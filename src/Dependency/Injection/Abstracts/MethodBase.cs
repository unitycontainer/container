using System;
using System.Linq;
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


        #region IMatch

        public override bool Match(TMemberInfo other)
        {
            var length = Data?.Length ?? 0;
            var parameters = other.GetParameters();

            if (length != parameters.Length) return false;

            for (var i = 0; i < length; i++)
            {
                var parameter = parameters[i];
                var value = Data![i];
                var match = value switch
                {
                    null => null == parameter.ParameterType,

                    IMatch<ParameterInfo> matchParam
                              => matchParam.Match(parameter),

                    IMatch<Type> matchType
                              => matchType.Match(parameter.ParameterType),

                    Type _ when typeof(Type).Equals(parameter.ParameterType)
                              => true,

                    Type type => MatchesType(type, parameter.ParameterType),
                    _ => MatchesObject(value, parameter.ParameterType),
                };

                if (match) continue;

                return false;
            }

            return true;
        }

        #endregion


        #region Overrides

        public override TMemberInfo MemberInfo(Type type)
        {
            if (null == Selection) throw new InvalidOperationException($"{GetType().Name} is not initialized");

            var methodHasOpenGenericParameters = Selection.GetParameters()
#if NETSTANDARD1_0 || NETCOREAPP1_0
                                                          .Select(p => p.ParameterType.GetTypeInfo())
#else
                                                          .Select(p => p.ParameterType)
#endif
                                                          .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var info = Selection.DeclaringType!.GetTypeInfo();
            if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
                return Selection;

#if NETSTANDARD
            var typeInfo = type.GetTypeInfo();
            var parameterTypes = Selection.GetParameters()
                                          .Select(pi => GetClosedParameterType(pi.ParameterType, typeInfo.GenericTypeArguments))
                                          .ToArray();

            var member = DeclaredMembers(type).Single(m => m.Name.Equals(Selection.Name) && ParametersMatch(m.GetParameters(), parameterTypes));
            if (null != member) return member;

            bool ParametersMatch(ParameterInfo[] parameters, Type[] closedConstructorParameterTypes)
            {
                if ((parameters ?? throw new ArgumentNullException(nameof(parameters))).Length !=
                    (closedConstructorParameterTypes ?? throw new ArgumentNullException(nameof(closedConstructorParameterTypes))).Length)
                {
                    return false;
                }

                return !parameters.Where((t, i) => !t.ParameterType.Equals(closedConstructorParameterTypes[i])).Any();
            }

            Type GetClosedParameterType(Type typeToReflect, Type[] genericArguments)
            {
                // Prior version of the framework returned both generic type arguments and parameters
                // through one mechanism, now they are broken out.  May want to consider different reflection
                // helpers instead of this case statement.

                var tInfo = typeToReflect.GetTypeInfo();
                if (tInfo.IsGenericType && tInfo.ContainsGenericParameters)
                {
                    Type[] typeArgs = tInfo.IsGenericTypeDefinition ? tInfo.GenericTypeParameters : tInfo.GenericTypeArguments;
                    for (int i = 0; i < typeArgs.Length; ++i)
                    {
                        typeArgs[i] = (genericArguments ?? throw new ArgumentNullException(nameof(genericArguments)))[typeArgs[i].GenericParameterPosition];
                    }
                    return typeToReflect.GetGenericTypeDefinition().MakeGenericType(typeArgs);
                }

                if (typeToReflect.IsArray)
                {
                    return typeToReflect.GetArrayParameterType(genericArguments);
                }

                if (tInfo.IsGenericParameter)
                {
                    return genericArguments[typeToReflect.GenericParameterPosition];
                }

                return typeToReflect;
            }

#else
            foreach (var member in DeclaredMembers(type))
            {
                if (member.MetadataToken == Selection.MetadataToken)
                    return member;
            }
#endif
            throw new InvalidOperationException($"Error selecting member on type {type}");
        }

        #endregion
    }
}
