using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    // TODO: Requires integration
    public static class SupportedMembers
    {
        /// <summary>
        /// Method to query <see cref="Type"/> for supported constructors
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ConstructorInfo> SupportedConstructors(this Type type) =>
             type.GetConstructors();


        /// <summary>
        /// Method to query <see cref="Type"/> for supported fields
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<FieldInfo> SupportedFields(this Type type) =>
            type.GetFields()
                .Where(member => !member.IsFamily &&
                                 !member.IsPrivate &&
                                 !member.IsInitOnly &&
                                 !member.IsStatic);

        /// <summary>
        /// Method to query <see cref="Type"/> for supported properties
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> SupportedProperties(this Type type) =>
             type.GetProperties()
                 .Where(member =>
                 {
                     if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                         return false;

                     var setter = member.GetSetMethod(true);
                     if (setter is null || setter.IsPrivate || setter.IsFamily)
                         return false;

                     return true;
                 });


        /// <summary>
        /// Method to query <see cref="Type"/> for supported methods
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<MethodInfo> SupportedMethods(this Type type) =>
             type.GetMethods()
                 .Where(member => !member.IsFamily && !member.IsPrivate && !member.IsStatic);
    }
}
