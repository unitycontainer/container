using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class MethodStrategy<TContext> : ParameterStrategy<TContext, MethodInfo>
        where TContext : IBuilderContext
    {
        #region Constructors

        public MethodStrategy(IPolicies policies)
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
