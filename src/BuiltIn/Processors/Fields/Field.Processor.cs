using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;

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

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override FieldInfo[] GetMembers(Type type) => type.GetFields(BindingFlags);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override InjectionMember<FieldInfo, object>? GetInjected(RegistrationManager? registration) => registration?.Fields;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Type MemberType(FieldInfo member) => member.FieldType;

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void SetValue(FieldInfo info, object target, object? value) => info.SetValue(target, value);


        #endregion
    }
}
