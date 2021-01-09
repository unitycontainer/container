using System.Diagnostics;
using Unity.Extension;

namespace Unity.Container
{
    public partial class InstanceStrategy : BuilderStrategy
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            // TODO: Proper validation
            Debug.Assert(null == context.Existing);
            Debug.Assert(null != context.Registration);
            Debug.Assert(RegistrationCategory.Instance == context.Registration?.Category);

            context.Existing = context.Registration?.Instance;
        }
    }
}
