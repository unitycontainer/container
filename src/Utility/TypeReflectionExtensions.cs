using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity
{
    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class in the .NET for Windows Store apps.
    /// </summary>
    internal static class TypeReflectionExtensions
    {


        // TODO: Requires optimization
        /// <summary>
        /// If this type is an open generic, use the
        /// given <paramref name="genericArguments"/> array to
        /// determine what the required closed type is and return that.
        /// </summary>
        /// <remarks>If the parameter is not an open type, just
        /// return this parameter's type.</remarks>
        /// <param name="typeToReflect"></param>
        /// <param name="genericArguments">Type arguments to substitute in for
        /// the open type parameters.</param>
        /// <returns>Corresponding closed type of this parameter.</returns>
        public static Type GetClosedParameterType(this Type typeToReflect, Type[] genericArguments)
        {
            // Prior version of the framework returned both generic type arguments and parameters
            // through one mechanism, now they are broken out.  May want to consider different reflection
            // helpers instead of this case statement.

            var info = typeToReflect.GetTypeInfo();
            if (info.IsGenericType && info.ContainsGenericParameters)
            {
                Type[] typeArgs = info.IsGenericTypeDefinition ? info.GenericTypeParameters : info.GenericTypeArguments;
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

            if (info.IsGenericParameter)
            {
                return genericArguments[typeToReflect.GenericParameterPosition];
            }

            return typeToReflect;
        }


        public static Type GetArrayParameterType(this Type typeToReflect, Type[] genericArguments)
        {
            var rank = typeToReflect.GetArrayRank();
            var element = typeToReflect.GetElementType();
            var type = element.IsArray ? element.GetArrayParameterType(genericArguments)
                                       : genericArguments[element.GenericParameterPosition];

            return 1 == rank ? type.MakeArrayType() : type.MakeArrayType(rank);
        }


        /// <summary>
        /// Given a generic argument name, return the corresponding type for this
        /// closed type. For example, if the current type is SomeType&lt;User&gt;, and the
        /// corresponding definition was SomeType&lt;TSomething&gt;, calling this method
        /// and passing "TSomething" will return typeof(User).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterName">Name of the generic parameter.</param>
        /// <returns>Type of the corresponding generic parameter, or null if there
        /// is no matching name.</returns>
        public static Type GetNamedGenericParameter(this Type type, string parameterName)
        {
            TypeInfo openType = type.GetGenericTypeDefinition().GetTypeInfo();
            Type result = null;
            int index = -1;

            foreach (var genericArgumentType in openType.GenericTypeParameters)
            {
                if (genericArgumentType.GetTypeInfo().Name == parameterName)
                {
                    index = genericArgumentType.GenericParameterPosition;
                    break;
                }
            }
            if (index != -1)
            {
                result = type.GetTypeInfo().GenericTypeArguments[index];
            }
            return result;
        }
    }
}
