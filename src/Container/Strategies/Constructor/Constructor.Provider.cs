using System.Reflection;

namespace Unity.Container
{
    public partial class ConstructorStrategy
    {
        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(InjectionConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}
