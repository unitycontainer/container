using System;
using Unity.Builder;
using Unity.Strategies;

namespace Unity.Tests.v5.ObjectBuilder.Utility
{
    internal class ActivatorCreationStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(ref BuilderContext context)
        {
            if (context.Existing == null)
            {
                context.Existing = Activator.CreateInstance(context.Type);
            }
        }
    }
}
