using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

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
            if (info.InjectedValue.IsValue) 
                info.MemberInfo.SetValue(context.Existing, info.InjectedValue.Value);
        }

        protected override BuilderStrategyPipeline SetMemberValueResolver(FieldInfo info, ResolverPipeline pipeline)
            => (ref BuilderContext context) => info.SetValue(context.Existing, pipeline(ref context));


        protected override InjectionMember<FieldInfo, object>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Fields;

        protected override Type GetMemberType(FieldInfo info) => info.FieldType;

        #endregion
    }
}
