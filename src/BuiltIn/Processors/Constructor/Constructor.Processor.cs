using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : ParameterProcessor<ConstructorInfo>
    {
        #region Constants

        protected const string NoConstructorError = "Unable to select constructor";

        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults, GetConstructors)
        {
            Select = defaults.GetOrAdd<Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>>(DefaultSelector, 
                (policy) => Select = (Func<UnityContainer, ConstructorInfo[], ConstructorInfo?>)policy);
        }

        #endregion


        #region Implementation

        private static ConstructorInfo[] GetConstructors(Type type) => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

        #endregion
    }
}
