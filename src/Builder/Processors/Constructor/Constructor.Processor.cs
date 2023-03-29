using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext> : ParameterProcessor<TContext, ConstructorInfo>
        where TContext : IBuilderContext
    {


        #region Fields

        protected MemberSelector<TContext, ConstructorInfo> SelectAlgorithmically;

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicies policies)
            : base(policies)
        {
            SelectAlgorithmically = policies.Get<MemberSelector<TContext, ConstructorInfo>>(OnSelectAlgorithmicallyChanged)
                ?? throw new InvalidOperationException(nameof(MemberSelector<TContext, ConstructorInfo>));
        }

        #endregion


        #region Implementation

        protected override InjectionMember<ConstructorInfo, object[]>[]? GetInjectedMembers(RegistrationManager? manager)
                => manager?.Constructors;

        #endregion


        #region Policy Changes

        private void OnSelectAlgorithmicallyChanged(Type? target, Type type, object? policy)
            => SelectAlgorithmically = (MemberSelector<TContext, ConstructorInfo>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
