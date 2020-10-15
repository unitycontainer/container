using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public FieldProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(BindingFlags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjected<TMember>(RegistrationManager? registration)
            where TMember : class => Unsafe.As<TMember>(registration?.Fields);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override Type MemberType(FieldInfo member) => member.FieldType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override ImportData AsImportData(FieldInfo info, object? data) 
            => info.AsImportData(data);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(FieldInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
