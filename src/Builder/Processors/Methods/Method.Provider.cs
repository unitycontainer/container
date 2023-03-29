using System.Reflection;

namespace Unity.Processors
{
    public partial class MethodProcessor<TContext>
    {
        public override void ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionMethodAttribute)))
                descriptor.IsImport = true;
        }
    }
}
