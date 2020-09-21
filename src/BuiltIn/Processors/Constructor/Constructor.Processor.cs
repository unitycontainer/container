using System;
using System.Reflection;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        #region Constants

        protected const string NoConstructorError = "Unable to select constructor";

        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults)
        {
            // Add constructor selector to default policies and subscribe to notifications

            var selector = (ConstructorSelector?)defaults.Get(typeof(ConstructorSelector));
            if (null == selector)
            {
                Select = DefaultConstructorSelector;
                defaults.Set(typeof(ConstructorSelector),
                                   (ConstructorSelector)DefaultConstructorSelector,
                                   (policy) => Select = (ConstructorSelector)policy);
            }
            else
                Select = selector;
        }

        #endregion


        #region Implementation

        protected override ConstructorInfo[] GetMembers(Type type) => type.GetConstructors(BindingFlags);

        protected override DependencyInfo OnGetDependencyInfo(ConstructorInfo info) 
            => new DependencyInfo(info.GetCustomAttribute(typeof(InjectionConstructorAttribute)));

        #endregion
    }
}
