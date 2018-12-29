using System;
using Unity.Builder;

namespace Unity.Policy
{
    [Obsolete("IBuildPlanPolicy has been deprecated, please use ResolveDelegateFactory instead", true)]
    public interface IBuildPlanPolicy
    {
        void BuildUp(ref BuilderContext context);
    }
}
