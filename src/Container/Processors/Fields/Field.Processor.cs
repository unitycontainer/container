﻿using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Container
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, FieldInfo, object>
    {
        #region Constructors

        static FieldProcessor()
        {
            GetMemberType = (member) => member.FieldType;
            GetDeclaringType = (member) => member.DeclaringType!;
        }

        /// <inheritdoc/>
        public FieldProcessor(Defaults defaults)
            : base(defaults, (type) => type.GetFields(BindingFlags.Public | BindingFlags.Instance),
                             DefaultImportProvider)
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjectedMembers<TMember>(RegistrationManager? registration)
            where TMember : class => Unsafe.As<TMember>(registration?.Fields);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(FieldInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}