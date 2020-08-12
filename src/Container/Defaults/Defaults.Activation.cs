using System;
using Unity.Pipeline;

namespace Unity.Container
{
    public partial class Defaults
    {
        private delegate object? ActivationDelegate(PipelineProcessor[] chain, ref ResolveContext context);

        private object? ActivationPipeline(PipelineProcessor[] chain, ref ResolveContext context)
        {
            throw new NotImplementedException();
        }
    }
}
