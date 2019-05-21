using System;
using System.Diagnostics;
using System.Security;
using System.Threading.Tasks;
using Unity.Builder;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        private BuildPipelineAsync? GetPipeline(Type type, string? name)
        {
            var key = new HashKey(type, name);

            // Iterate through containers hierarchy
            for (UnityContainer? container = this; null != container; container = container._parent)
            {
                // Skip to the parent if no pipelines
                if (null == container._pipelines) continue;

                var registry = container._pipelines;

                // Check for exact match
                for (var i = registry.Buckets[key.HashCode % registry.Buckets.Length]; i >= 0; i = registry.Entries[i].Next)
                {
                    ref var candidate = ref registry.Entries[i];
                    if (candidate.Key != key) continue;

                    // Found it
                    return candidate.Pipeline;
                }
            }

            return null;
        }


        #region Root Build Pipeline

        [SecuritySafeCritical]
        public ValueTask<object?> DefaultBuildPipeline(ref PipelineContext context)
        {
            var builder = new PipelineBuilder(ref PipelineContext context);

            throw new NotImplementedException();
        }

        #endregion
    }
}
