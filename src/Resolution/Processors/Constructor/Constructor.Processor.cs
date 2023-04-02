using System.Reflection;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class ConstructorProcessor : ParameterProcessor<ConstructorInfo>
    {


        #region Fields

        protected Func<UnityContainer, ConstructorInfo[], ConstructorInfo?> SelectAlgorithmically;

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicies policies)
            : base(policies)
        {
            SelectAlgorithmically = policies.GetOrAdd(AlgorithmicSelector, OnSelectAlgorithmicallyChanged);
        }

        #endregion


        #region Implementation

        protected override InjectionMember<ConstructorInfo, object[]>[]? GetInjectedMembers(RegistrationManager? manager)
                => manager?.Constructors;

        #endregion


        #region Policy Changes

        private void OnSelectAlgorithmicallyChanged(Type? target, Type type, object? policy)
            => SelectAlgorithmically = (Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>)(policy
            ?? throw new ArgumentNullException(nameof(policy)));

        #endregion
    }
}
