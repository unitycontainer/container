using System.Reflection;

namespace Unity.Container
{
    internal static partial class UnityDefaultBehaviorExtension
    {
        #region Default Get Members

        /// <summary>
        /// Retrieves public constructors for the <see cref="Type"/>
        /// </summary>
        private static ConstructorInfo[] GetConstructors(Type type)
            => type.GetConstructors(BindingFlags.Public |
                                    BindingFlags.Instance |
                                    BindingFlags.DeclaredOnly);

        /// <summary>
        /// Retrieves public fields for the <see cref="Type"/>
        /// </summary>
        private static FieldInfo[] GetFields(Type type)
            => type.GetFields(BindingFlags.Public |
                              BindingFlags.Instance);

        /// <summary>
        /// Retrieves public properties for the <see cref="Type"/>
        /// </summary>
        private static PropertyInfo[] GetProperties(Type type)
            => type.GetProperties(BindingFlags.Public |
                                  BindingFlags.Instance);

        /// <summary>
        /// Retrieves public methods for the <see cref="Type"/>
        /// </summary>
        private static MethodInfo[] GetMethods(Type type)
            => type.GetMethods(BindingFlags.Public |
                               BindingFlags.Instance |
                               BindingFlags.FlattenHierarchy);

        #endregion
    }
}
