using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Processors
{
    public partial class FieldProcessor<TContext> : MemberProcessor<TContext, FieldInfo, object>
        where TContext : IBuilderContext
    {
        #region Constructors

        /// <inheritdoc/>
        public FieldProcessor(IPolicies policies)
            : base(policies)
        { }

        #endregion


        #region Implementation

        /// <inheritdoc/>
        protected override void BuildUp<TDescriptor>(ref TContext context, ref TDescriptor info, ref ValueData data)
        {
            if (data.IsValue) info.MemberInfo.SetValue(context.Existing, data.Value);
        }

        protected override InjectionMember<FieldInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Fields;

        protected override Type GetMemberType(FieldInfo info) => info.FieldType;

        #endregion
    }
}
