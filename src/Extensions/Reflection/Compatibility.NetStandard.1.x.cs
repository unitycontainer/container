using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal static class Compatibility_NetStandard_1
    {
        public static Attribute GetCustomAttribute(this MemberInfo info, Type type)
        {
            return info.GetCustomAttributes()
                       .Where(a => a.GetType()
                                    .GetTypeInfo()
                                    .IsAssignableFrom(type.GetTypeInfo()))
                       .FirstOrDefault();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericTypeDefinition(this Type type) => type.GetTypeInfo().IsGenericTypeDefinition;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsGenericParameters(this Type type) => type.GetTypeInfo().ContainsGenericParameters;


        public static TInfo? GetMemberFromInfo<TInfo>(this TInfo info, Type type)
            where TInfo : MethodBase
        {
            IEnumerable<MethodBase> members = info.MemberType switch
            {
                MemberTypes.Constructor => type.GetConstructors(BindingFlags.NonPublic |
                                                                BindingFlags.Public |
                                                                BindingFlags.Instance),
                MemberTypes.Method => type.GetMethods(BindingFlags.NonPublic |
                                                           BindingFlags.Public |
                                                           BindingFlags.Instance),
                _ => throw new InvalidOperationException()
            };

            foreach (TInfo member in members)
            {
                if (member.MetadataToken != info.MetadataToken) continue;

                return member;
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ConstructorInfo> GetConstructors(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredConstructors
                       .Where(ctor => !ctor.IsStatic);
        }

        public static FieldInfo[] GetFields(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredFields
                       .Where(field => !field.IsStatic)
                       .ToArray();
        }

        public static PropertyInfo[] GetProperties(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredProperties
                       .ToArray();
        }

        public static MethodInfo[] GetMethods(this Type type, BindingFlags _)
        {
            return type.GetTypeInfo()
                       .DeclaredMethods
                       .Where(method => !method.IsStatic)
                       .ToArray();
        }

        public static FieldInfo? GetField(this Type type, string name)
        {
            return type.GetTypeInfo()
                       .DeclaredFields
                       .FirstOrDefault(p => p.Name == name);
        }

        public static PropertyInfo? GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo()
                       .DeclaredProperties
                       .FirstOrDefault(p => p.Name == name);
        }

        public static MethodInfo GetSetMethod(this PropertyInfo info, bool _)
        {
            return info.SetMethod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAssignableFrom(this Type match, Type type)
        { 
            return match.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }
    }

    [Flags]
    internal enum BindingFlags
    {
        Default = 0,
        IgnoreCase = 1,
        DeclaredOnly = 2,
        Instance = 4,
        Static = 8,
        Public = 16,
        NonPublic = 32,
        FlattenHierarchy = 64,
        InvokeMethod = 256,
        CreateInstance = 512,
        GetField = 1024,
        SetField = 2048,
        GetProperty = 4096,
        SetProperty = 8192,
        PutDispProperty = 16384,
        PutRefDispProperty = 32768,
        ExactBinding = 65536,
        SuppressChangeType = 131072,
        OptionalParamBinding = 262144,
        IgnoreReturn = 16777216,
        DoNotWrapExceptions = 33554432
    }
}