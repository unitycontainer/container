using System;
using System.Reflection;
using Unity.Container;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class ConstructorProcessor : MethodBaseProcessor<ConstructorInfo>
    {
        #region Delegates

        public delegate ConstructorInfo ConstructorSelectorDelegate(ConstructorInfo[] members, ref ResolveContext context);

        #endregion


        #region Fields

        protected ConstructorSelectorDelegate SelectConstructor;
        
        #endregion


        #region Constructors

        public ConstructorProcessor(Defaults defaults)
            : base(defaults)
        {
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

        protected override ConstructorInfo[] GetMembers(Type type) => type.GetConstructors(SupportedBindingFlags);

        protected override bool DefaultAnnotationPredicate(ConstructorInfo member) => member.IsDefined(typeof(InjectionConstructorAttribute));

        #endregion
    }
}
