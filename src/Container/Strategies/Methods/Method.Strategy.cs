using System.Reflection;
using Unity.Extension;
using Unity.Injection;

namespace Unity.Container
{
    public partial class MethodStrategy : ParameterStrategy<MethodInfo>
    {
        #region Constructors

        public MethodStrategy(IPolicies policies)
            : base(policies)
        {
        }

        #endregion


        protected override InjectionMember<MethodInfo, object[]>? InjectedMembers(RegistrationManager? manager)
            => manager?.Methods;
    }
}
