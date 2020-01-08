using System;
using System.Collections.Generic;
using System.Reflection;


namespace Unity
{

    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class.
    /// </summary>
    internal static class ContainerReflectionExtensions
    {
        #pragma warning disable CS8604
        #pragma warning disable CS8625

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
            while (null != info)
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

        #pragma warning restore CS8625
        #pragma warning restore CS8604
    }
}

