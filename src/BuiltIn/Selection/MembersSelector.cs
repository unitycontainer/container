using System;
using System.Reflection;

namespace Unity.BuiltIn
{
    public static class MembersSelector
    {
        #region Default Get Members

        /// <summary>
        /// Determines constructors selected by default when 
        /// <see cref="Type.GetConstructors"/> is called
        /// </summary>
        public static ConstructorInfo[] GetConstructors(Type type)
            => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);


        /// <summary>
        /// Determines methods selected by default when 
        /// <see cref="Type.GetMethods"/> is called
        /// </summary>
        public static MethodInfo[] GetMethods(Type type)
            => type.GetMethods(BindingFlags.Public |
                               BindingFlags.Instance |
                               BindingFlags.FlattenHierarchy |
                               BindingFlags.DeclaredOnly);


        /// <summary>
        /// Determines fields selected by default when 
        /// <see cref="Type.GetFields"/> is called
        /// </summary>
        public static FieldInfo[] GetFields(Type type)
            => type.GetFields(BindingFlags.Public | BindingFlags.Instance);


        /// <summary>
        /// Determines properties selected by default when 
        /// <see cref="Type.GetProperties"/> is called
        /// </summary>
        public static PropertyInfo[] GetProperties(Type type)
            => type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        #endregion
    }
}
