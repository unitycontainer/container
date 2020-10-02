using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

namespace Unity.BuiltIn
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, PropertyInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override InjectionMember<PropertyInfo, object>? GetInjected(RegistrationManager? registration) => registration?.Properties;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override ImportAttribute? GetImportAttribute(PropertyInfo info)
            => (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute), true);

        #endregion
    }
}
