using System.Reflection;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Processors
{
    public partial class MethodProcessor : ParameterProcessor<MethodInfo>
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
