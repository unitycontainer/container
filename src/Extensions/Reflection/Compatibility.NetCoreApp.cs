using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    internal static class Compatibility_NetCoreApp_1_0
    {
        #region Type

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInterface(this Type type) => type.GetTypeInfo().IsInterface;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsClass(this Type type) => type.GetTypeInfo().IsClass;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAbstract(this Type type) => type.GetTypeInfo().IsAbstract;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPrimitive(this Type type) => type.GetTypeInfo().IsPrimitive;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEnum(this Type type) => type.GetTypeInfo().IsEnum;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Type[] GetGenericArguments(this Type type) => type.GetTypeInfo().GetGenericArguments();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericType(this Type type) => type.GetTypeInfo().IsGenericType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsValueType(this Type type) => type.GetTypeInfo().IsValueType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsGenericTypeDefinition(this Type type) => type.GetTypeInfo().IsGenericTypeDefinition;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ContainsGenericParameters(this Type type) => type.GetTypeInfo().ContainsGenericParameters;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAssignableFrom(this Type match, Type type) => match.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSubclassOf(this Type match, Type type) => match.GetTypeInfo().IsSubclassOf(type);

        #endregion


        #region Custom Attribute

        public static Attribute GetCustomAttribute(this MemberInfo info, Type type)
        {
            return info.GetCustomAttributes()
                       .Where(a => a.GetType()
                                    .GetTypeInfo()
                                    .IsAssignableFrom(type.GetTypeInfo()))
                       .FirstOrDefault();
        }

        #endregion


        #region Member Info

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

        public static FieldInfo? GetField(this Type type, string name)
        {
            return type.GetField(name);
        }

        #endregion
    }


    #region Missing Types
    internal interface ICloneable
    {
        object Clone();
    }

    #endregion

}
