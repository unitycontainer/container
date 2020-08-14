using System;
using Unity.Container;

namespace Unity.Pipeline
{
    public abstract class PipelineProcessor
    {
        #region Build

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        /// <returns>Returns intermediate value or policy</returns>
        public virtual void PreBuildUp(ref ResolveContext context)
        {
        }

        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PostBuildUp method is called when the chain has finished the PreBuildUp
        /// phase and executes in reverse order from the PreBuildUp calls.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public virtual void PostBuildUp(ref ResolveContext context)
        {
        }

        #endregion


        public static object? DefaultResolver(PipelineProcessor[] chain, ref ResolveContext context)
        {
            var i = -1;

            while (++i < chain.Length)
            {
                chain[i].PreBuildUp(ref context);
            }

            while (--i >= 0)
            {
                chain[i].PostBuildUp(ref context);
            }

            return context.Existing;
        }
    }
}
