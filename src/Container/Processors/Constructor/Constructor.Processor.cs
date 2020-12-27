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


        #region Fields

        protected SelectorDelegate<ConstructorInfo[], ConstructorInfo?> SelectionHandler { get; set; }

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicies policies)
            : base(policies)
        {
            SelectionHandler = policies.Get<ConstructorInfo, SelectorDelegate<ConstructorInfo[], ConstructorInfo?>>(OnSelectorChanged)!;
        }

        private void OnSelectorChanged(Type? target, Type type, object? policy)
        {
            if (policy is null) throw new ArgumentNullException(nameof(policy));
            SelectionHandler = (SelectorDelegate<ConstructorInfo[], ConstructorInfo?>)policy;
        }

        #endregion
    }
}
