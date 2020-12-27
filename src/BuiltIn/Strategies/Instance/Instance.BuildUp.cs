using System.Diagnostics;

namespace Unity.Container
{
    public partial class InstanceProcessor
    {
        public override void PreBuildUp<TContext>(ref TContext context)
        {
            // TODO: Proper validation
            Debug.Assert(null == context.Target);
            Debug.Assert(null != context.Registration);
            Debug.Assert(RegistrationCategory.Instance == context.Registration?.Category);

            context.Target = context.Registration?.Instance;
        }
    }
}
