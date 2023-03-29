using System.Reflection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        protected override void InjectionInfoProvider<TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}
