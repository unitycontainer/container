using System;
using System.Collections.Generic;
using System.Reflection;

namespace Unity
{
    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class.
    /// </summary>
    internal static class TypeReflectionExtensions
    {
        public static Type GetArrayParameterType(this Type typeToReflect, Type[] genericArguments)
        {
            var rank = typeToReflect.GetArrayRank();
            var element = typeToReflect.GetElementType();
            var type = element.IsArray ? element.GetArrayParameterType(genericArguments)
                                       : genericArguments[element.GenericParameterPosition];

            return 1 == rank ? type.MakeArrayType() : type.MakeArrayType(rank);
        }

        public static IEnumerable<FieldInfo> GetDeclaredFields(this Type type)
        {
            var info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredFields)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<PropertyInfo> GetDeclaredProperties(this Type type)
        {
            var info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredProperties)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<MethodInfo> GetDeclaredMethods(this Type type)
        {
            var info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredMethods)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }
    }
}
