using System.Reflection;

namespace Unity.Container
{
    public partial class ConstructorStrategy<TContext>
    {
        public override void ProvideInfo<TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}
