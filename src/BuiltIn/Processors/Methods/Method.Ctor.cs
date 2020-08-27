using System;
using System.Reflection;
using Unity.Container;
using Unity.Pipeline;

namespace Unity.BuiltIn
{
    public partial class MethodProcessor : MethodBaseProcessor<MethodInfo>
    {
        #region Constructors

        public MethodProcessor(Defaults defaults)
            : base(defaults)
        {
        }

        #endregion


        #region Implementation

        protected override MethodInfo[] GetMembers(Type type) => type.GetMethods(SupportedBindingFlags);

        protected override bool DefaultAnnotationPredicate(MethodInfo member) => member.IsDefined(typeof(InjectionMethodAttribute));

        #endregion
    }
}
