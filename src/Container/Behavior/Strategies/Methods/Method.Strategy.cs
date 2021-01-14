using System.Reflection;
using Unity.Extension;

namespace Unity.Container
{
    public partial class MethodStrategy : ParameterStrategy<MethodInfo>
    {
        #region Constructors

        public MethodStrategy(IPolicies policies)
            : base(policies)
        {
        }

        #endregion


        protected override void Execute(MethodInfo info, object target, object? arguments)
        {
            info.Invoke(target, (object[])arguments!);
        }
    }
}
