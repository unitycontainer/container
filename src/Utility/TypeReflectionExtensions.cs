using System;
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
    }
}
