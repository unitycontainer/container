using System;
using Unity.Container;
using Unity.Extension;
using Unity.Resolution;

namespace Unity.BuiltIn
{
    public static class LazyFactory
    {
        public static void Setup(ExtensionContext context)
        {
            var policies = (Defaults)context.Policies;
            
            policies.Set<PipelineFactory<Contract>>(typeof(Lazy<>), Factory);
        }

        private static ResolveDelegate<PipelineContext> Factory(ref Contract data)
        {
            throw new NotImplementedException();
        }
    }
}
