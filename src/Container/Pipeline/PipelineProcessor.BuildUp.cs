using System.Runtime.CompilerServices;
using Unity.Resolution;

namespace Unity.Pipeline
{
    public abstract partial class PipelineProcessor
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="pipeline">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp(ref PipelineContext pipeline)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="pipeline">Context of the build operation.</param>
        public virtual void PostBuildUp(ref PipelineContext pipeline)
        {
        }
    }
}
