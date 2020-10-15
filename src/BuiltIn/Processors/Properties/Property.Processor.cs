using System;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override PropertyInfo[] GetMembers(Type type) => type.GetProperties(BindingFlags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjected<TMember>(RegistrationManager? registration) 
            where TMember : class => Unsafe.As<TMember>(registration?.Properties);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override ImportAttribute? GetImportAttribute(PropertyInfo info)
            => (ImportAttribute?)info.GetCustomAttribute(typeof(ImportAttribute), true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override Type MemberType(PropertyInfo member) => member.PropertyType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override ImportData AsImportData(PropertyInfo info, object? data) 
            => info.AsImportData(data);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
