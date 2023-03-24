using System;
using System.Reflection;

namespace Unity.Container
{
    internal static partial class Selection
    {
        #region Default Get Members

        /// <summary>
        /// Determines constructors selected by default when 
        /// <see cref="Type.GetConstructors"/> is called
        /// </summary>
        private static ConstructorInfo[] GetConstructors(Type type)
            => type.GetConstructors(BindingFlags.Public | 
                                    BindingFlags.Instance | 
                                    BindingFlags.DeclaredOnly);


        /// <summary>
        /// Determines methods selected by default when 
        /// <see cref="Type.GetMethods"/> is called
        /// </summary>
        private static MethodInfo[] GetMethods(Type type)
            => type.GetMethods(BindingFlags.Public |
                               BindingFlags.Instance |
                               BindingFlags.FlattenHierarchy);

        /// <summary>
        /// Determines fields selected by default when 
        /// <see cref="Type.GetFields"/> is called
        /// </summary>
        private static FieldInfo[] GetFields(Type type)
            => type.GetFields(BindingFlags.Public | 
                              BindingFlags.Instance);


        /// <summary>
        /// Determines properties selected by default when 
        /// <see cref="Type.GetProperties"/> is called
        /// </summary>
        private static PropertyInfo[] GetProperties(Type type)
            => type.GetProperties(BindingFlags.Public | 
                                  BindingFlags.Instance);

        #endregion
    }
}
