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


        protected override void Execute<TContext, TDescriptor>(ref TContext context, ref TDescriptor descriptor, ref ImportData data) 
            => descriptor.MemberInfo.Invoke(context.Existing, (object[]?)data.Value);
    }
}
