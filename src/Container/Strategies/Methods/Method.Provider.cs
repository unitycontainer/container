using System.Reflection;

namespace Unity.Container
{
    public partial class MethodStrategy
    {
        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionMethodAttribute)))
                descriptor.IsImport = true;
        }
    }
}
