using System.Reflection;

namespace Unity.Processors
{
    public partial class ConstructorProcessor<TContext>
    {
        public override void ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}
