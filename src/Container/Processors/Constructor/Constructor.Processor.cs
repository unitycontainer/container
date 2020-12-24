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

        protected UnitySelector<ConstructorInfo[], ConstructorInfo?> SelectionHandler { get; set; }

        #endregion


        #region Constructors

        public ConstructorProcessor(IPolicyObservable policies)
            : base(policies)
        {
            SelectionHandler = policies.Get<ConstructorInfo, UnitySelector<ConstructorInfo[], ConstructorInfo?>>(OnSelectorChanged)!;
        }

        private void OnSelectorChanged(Type? target, Type type, object? policy)
        {
            if (policy is null) throw new ArgumentNullException(nameof(policy));
            SelectionHandler = (UnitySelector<ConstructorInfo[], ConstructorInfo?>)policy;
        }

        #endregion
    }
}
