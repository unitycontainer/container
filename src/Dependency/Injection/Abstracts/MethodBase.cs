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


        #region Overrides

        public override TMemberInfo MemberInfo(Type type)
        {
            var methodHasOpenGenericParameters = Selection.GetParameters()
                                                     .Select(p => p.ParameterType.GetTypeInfo())
                                                     .Any(i => i.IsGenericType && i.ContainsGenericParameters);

            var info = Selection.DeclaringType.GetTypeInfo();
            if (!methodHasOpenGenericParameters && !(info.IsGenericType && info.ContainsGenericParameters))
                return Selection;

#if NETSTANDARD1_0
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
