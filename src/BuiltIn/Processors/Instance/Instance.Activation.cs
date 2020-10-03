using System.Diagnostics;
using Unity.Container;

namespace Unity.BuiltIn
{
    public partial class InstanceProcessor
    {
        public override void PreBuild(ref PipelineContext context)
        {
            Debug.Assert(null == context.Target);
            Debug.Assert(null != context.Registration);
            Debug.Assert(RegistrationCategory.Instance == context.Registration.Category);

            context.Target = context.Registration.Instance;
        }
    }
}
