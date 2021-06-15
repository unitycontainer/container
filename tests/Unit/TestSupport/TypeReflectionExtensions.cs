using System;
using System.Linq;
using System.Reflection;

namespace Unity.Tests.v5.TestSupport
{
    public static class TypeReflectionExtensions
    {
        public static ConstructorInfo GetMatchingConstructor(this Type type, Type[] constructorParamTypes)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Where(c => c.GetParameters().Select(p => p.ParameterType).SequenceEqual(constructorParamTypes))
                .FirstOrDefault();
        }
    }
}
