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

        public ConstructorProcessor(IPolicyList defaults)
            : base(defaults)
        {
            Select = ((Defaults)defaults).GetOrAdd<Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(DefaultSelector, 
                (target, type, policy) => Select = (Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>)(policy ?? 
                    throw new ArgumentNullException(nameof(policy))));
        }

        #endregion
    }
}
