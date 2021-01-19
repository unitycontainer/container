using System.ComponentModel.Composition;
using System.Reflection;

namespace Unity.Container
{
    public partial class ConstructorStrategy
    {
        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (descriptor.MemberInfo.IsDefined(typeof(ImportingConstructorAttribute)))
                descriptor.IsImport = true;
        }
    }
}
