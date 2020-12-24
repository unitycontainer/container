using System;
using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class ConstructorProcessor : ParameterProcessor<ConstructorInfo>
    {
        #region Constants

        protected const string NoConstructorError = "Unable to select constructor";

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicyObservable policies)
            : base(policies)
        {
            Select = DefaultSelector;
            policies.Set<Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(DefaultSelector, OnSelectorChanged);
        }

        private void OnSelectorChanged(Type? target, Type type, object? policy)
        {
            if (policy is null) throw new ArgumentNullException(nameof(policy));
            Select = (Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>)policy;
        }

        #endregion
    }
}
