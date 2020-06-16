using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal interface ICloneable
    {
        object Clone();
    }


    internal static class Compatibility_NetCoreApp_1_0
    {
        public static FieldInfo? GetField(this Type type, string name)
        { 
            return type.GetField(name);
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAssignableFrom(this Type match, Type type)
        {
            return match.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }


        public static TInfo? GetMemberFromInfo<TInfo>(this TInfo info, Type type)
            where TInfo : MethodBase
        {
            System.Collections.Generic.IEnumerable<MethodBase> members = info.MemberType switch
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
    }
}
