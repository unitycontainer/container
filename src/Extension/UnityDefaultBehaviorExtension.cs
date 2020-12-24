using System;
using System.Reflection;

namespace Unity.Extension
{
    /// <summary>
    /// This extension supplies the default behavior of the UnityContainer API
    /// by handling the context events and setting policies.
    /// </summary>
    public class UnityDefaultBehaviorExtension
    {
        /// <summary>
        /// Install the default container behavior into the container.
        /// </summary>
        public static void Initialize(ExtensionContext context)
        {
            // Set Member Selectors: GetConstructors(), GetFields(), etc.
            context.Policies.Set<GetMembersDelegate<ConstructorInfo>>(GetConstructors);
            context.Policies.Set<GetMembersDelegate<PropertyInfo>>(GetProperties);
            context.Policies.Set<GetMembersDelegate<MethodInfo>>(GetMethods);
            context.Policies.Set<GetMembersDelegate<FieldInfo>>(GetFields);

            // TODO: Set default Constructor selector

            #region Factories

            BuiltIn.PipelineFactory.Setup(context);
            BuiltIn.LazyFactory.Setup(context);
            BuiltIn.FuncFactory.Setup(context);

            #endregion
        }


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
