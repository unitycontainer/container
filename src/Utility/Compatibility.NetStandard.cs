using System;

namespace Unity
{
    internal static class Compatibility_NetStandard
    {
        public static Type GetArrayParameterType(this Type typeToReflect, Type[] genericArguments)
        {
            var rank = typeToReflect.GetArrayRank();
            var element = typeToReflect.GetElementType();
            Type type;
            if (element.IsArray)
            {
                type = element.GetArrayParameterType(genericArguments);
            }
            else
            {
                type = genericArguments[element.GenericParameterPosition];
            }

            return 1 == rank ? type.MakeArrayType() : type.MakeArrayType(rank);
        }
    }
}
