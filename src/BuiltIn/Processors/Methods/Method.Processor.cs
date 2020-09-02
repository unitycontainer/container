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

        protected override MethodInfo[] GetMembers(Type type) => type.GetMethods(BindingFlags);

        protected override DependencyInfo OnGetDependencyInfo(MethodInfo info)
            => new DependencyInfo(info.GetCustomAttribute(typeof(InjectionMethodAttribute)));

        #endregion
    }
}
