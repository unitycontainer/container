using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity
{
    public static class SupportedMembers
    {
        /// <summary>
        /// Method to query <see cref="Type"/> for supported constructors
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<ConstructorInfo> SupportedConstructors(this Type type)
        {
            return type.GetConstructors(BindingFlags.NonPublic |
                                        BindingFlags.Public |
                                        BindingFlags.Instance)
                       .Where(ctor => !ctor.IsFamily &&
                                      !ctor.IsPrivate);
        }

        /// <summary>
        /// Method to query <see cref="Type"/> for supported fields
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<FieldInfo> SupportedFields(this Type type)
        {
            return type.GetFields(BindingFlags.NonPublic |
                                  BindingFlags.Public |
                                  BindingFlags.Instance)
                       .Where(member => !member.IsFamily &&
                                        !member.IsPrivate &&
                                        !member.IsInitOnly);
        }

        /// <summary>
        /// Method to query <see cref="Type"/> for supported properties
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<PropertyInfo> SupportedProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.NonPublic |
                                      BindingFlags.Public |
                                      BindingFlags.Instance)
                       .Where(member =>
                       {
                           if (!member.CanWrite || 0 != member.GetIndexParameters().Length)
                               return false;

                           var setter = member.GetSetMethod(true);
                           if (null == setter || setter.IsPrivate || setter.IsFamily)
                               return false;

                           return true;
                       });
        }


        /// <summary>
        /// Method to query <see cref="Type"/> for supported methods
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<MethodInfo> SupportedMethods(this Type type)
        {
            return type.GetMethods(BindingFlags.NonPublic |
                                   BindingFlags.Public |
                                   BindingFlags.Instance)
                       .Where(member => !member.IsFamily &&
                                        !member.IsPrivate);
        }
    }
}
