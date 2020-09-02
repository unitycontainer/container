using System;
using System.Reflection;
using Unity.Container;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        #region Constants

        const string NoConstructor = "Unable to select constructor";
        
        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults)
        {
            // Add constructor selector to default policies and subscribe to notifications

            var selector = (ConstructorSelectorDelegate?)defaults.Get(typeof(ConstructorSelectorDelegate));
            if (null == selector)
            {
                SelectConstructor = DefaultConstructorSelector;
                defaults.Set(typeof(ConstructorSelectorDelegate),
                                   (ConstructorSelectorDelegate)DefaultConstructorSelector,
                                   (policy) => SelectConstructor = (ConstructorSelectorDelegate)policy);
            }
            else
                SelectConstructor = selector;
        }

        #endregion


        #region Implementation

        protected override ConstructorInfo[] GetMembers(Type type) => type.GetConstructors(BindingFlags);

        protected override DependencyInfo OnGetDependencyInfo(ConstructorInfo info) 
            => new DependencyInfo(info.GetCustomAttribute(typeof(InjectionConstructorAttribute)));

        #endregion
    }
}
