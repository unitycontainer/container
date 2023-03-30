using System.Reflection;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext> : ParameterProcessor<TContext, MethodInfo>
        where TContext : IBuilderContext
    {
        #region Constructors

        public MethodProcessor(IPolicies policies)
            : base(policies)
        {
        }

        #endregion


        #region Implementation

        protected override InjectionMember<MethodInfo, object[]>[]? GetInjectedMembers(RegistrationManager? manager)
            => manager?.Methods;

        #endregion
    }
}
