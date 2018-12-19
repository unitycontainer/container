using System;
using Unity.Builder;

namespace Unity.Policy
{
    /// <summary>
    /// A policy that can create and return an <see cref="IBuildPlanPolicy"/>
    /// for the given build key.
    /// </summary>
    public interface IBuildPlanCreatorPolicy 
    {
        /// <summary>
        /// Create a build plan using the given context and build key.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns>The build plan.</returns>
        IBuildPlanPolicy CreatePlan(ref BuilderContext context, Type type, string name);
    }
}
