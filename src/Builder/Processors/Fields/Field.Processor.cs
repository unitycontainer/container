﻿using System.Reflection;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class FieldProcessor : MemberProcessor<FieldInfo, object>
    {
        #region Constructors

        /// <inheritdoc/>
        public FieldProcessor(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        protected override void BuildUpMember<TContext>(ref TContext context, ref InjectionInfoStruct<FieldInfo> info)
        {
            if (info.DataValue.IsValue) info.MemberInfo.SetValue(context.Existing, info.DataValue.Value);
        }

        protected override InjectionMember<FieldInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Fields;

        protected override Type GetMemberType(FieldInfo info) => info.FieldType;

        #endregion
    }
}
