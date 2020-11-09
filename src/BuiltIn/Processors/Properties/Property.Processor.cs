﻿using System;
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
            : base(defaults, GetProperties, DefaultImportProvider, DefaultImportParser)
        {
        }

        #endregion


        #region Implementation


        private static PropertyInfo[] GetProperties(Type type) => type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override TMember? GetInjected<TMember>(RegistrationManager? registration) 
            where TMember : class => Unsafe.As<TMember>(registration?.Properties);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override Type MemberType(PropertyInfo member) => member.PropertyType;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <inheritdoc/>
        protected override void SetValue(PropertyInfo info, object target, object? value) => info.SetValue(target, value);

        #endregion
    }
}
