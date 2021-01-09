using System;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class ConstructorStrategy : ParameterStrategy<ConstructorInfo>
    {
        #region Fields

        protected SelectorDelegate<UnityContainer, ConstructorInfo[], ConstructorInfo?> SelectAlgorithmically;

        #endregion


        #region Constructors

        public ConstructorStrategy(IPolicies policies)
            : base(policies)
        {
            SelectAlgorithmically = policies.Get<ConstructorInfo, SelectorDelegate<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(
                                             OnAlgorithmChanged)!;
        }

        #endregion


        #region Policy Changes

        private void OnAlgorithmChanged(Type? target, Type type, object? policy)
        {
            if (policy is null) throw new ArgumentNullException(nameof(policy));
            SelectAlgorithmically = (SelectorDelegate<UnityContainer, ConstructorInfo[], ConstructorInfo?>)policy;
        }

        #endregion
    }
}
