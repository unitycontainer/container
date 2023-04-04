﻿using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class PropertyProcessor : MemberProcessor<PropertyInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public PropertyProcessor(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void BuildUpMember<TContext>(ref TContext context, ref InjectionInfoStruct<PropertyInfo> info)
        {
            if (info.InjectedValue.IsValue) info.MemberInfo.SetValue(context.Existing, info.InjectedValue.Value);
        }

        /// <inheritdoc/>
        protected override InjectionMember<PropertyInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Properties;

        /// <inheritdoc/>
        protected override Type GetMemberType(PropertyInfo info) => info.PropertyType;  

        #endregion
    }
}
