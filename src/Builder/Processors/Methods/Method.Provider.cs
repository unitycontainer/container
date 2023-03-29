using System.Reflection;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext>
    {
        protected override void InjectionInfoProvider<TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionMethodAttribute)))
                descriptor.IsImport = true;
        }
    }
}
