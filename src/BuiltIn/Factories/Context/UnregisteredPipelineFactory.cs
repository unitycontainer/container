using System;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class UnregisteredPipelineFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;

            //policies.Set(typeof(Defaults.UnregisteredPipelineFactory), (Defaults.UnregisteredPipelineFactory)Factory);
        }

        public static PipelineProcessor Factory(ref PipelineContext context)
        {
            throw new NotImplementedException();
            //return (ref PipelineContext c) => { };
        }
    }
}
