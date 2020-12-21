using System;
using System.Reflection;

namespace Unity.Container
{
    public partial class ConstructorProcessor : ParameterProcessor<ConstructorInfo>
    {
        #region Constants

        protected const string NoConstructorError = "Unable to select constructor";

        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults)
        {
            Select = defaults.GetOrAdd<Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(DefaultSelector, 
                (target, type, policy) => Select = (Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>)policy);
        }

        #endregion
    }
}
