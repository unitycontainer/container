using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Unity
{
    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class.
    /// </summary>
    internal static class ContainerReflectionExtensions
    {

        public static IEnumerable<FieldInfo> DeclaredFields(this Type type)
        {
            TypeInfo? info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredFields)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<PropertyInfo> DeclaredProperties(this Type type)
        {
            TypeInfo? info = type.GetTypeInfo();
            while (null! != info!)
            {
                foreach (var member in info.DeclaredProperties)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }

        public static IEnumerable<MethodInfo> DeclaredMethods(this Type type)
        {
            TypeInfo? info = type.GetTypeInfo();
            while (null != info)
            {
                foreach (var member in info.DeclaredMethods)
                    yield return member;

                info = info.BaseType?.GetTypeInfo();
            }
        }
    }
}

