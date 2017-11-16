// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Lifetime;
using Unity.Policy;

namespace Unity.ObjectBuilder.Strategies
{
    /// <summary>
    /// Represents a strategy for mapping build keys in the build up operation.
    /// </summary>
    public class BuildKeyMappingStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation.  Looks for the <see cref="IBuildKeyMappingPolicy"/>
        /// and if found maps the build key for the current operation.
        /// </summary>
        /// <param name="context">The context for the operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            var policy = context.Policies.Get<IBuildKeyMappingPolicy>(context.BuildKey);

            if (policy != null)
            {
                context.BuildKey = policy.Map(context.BuildKey, context);
            }

            if (context.BuildKey == context.OriginalBuildKey ||
                0 < ((PolicyList)context.Policies).Count) return;

            ILifetimePolicy lifetimePolicy = context.Policies.Get<ILifetimePolicy>(context.BuildKey, out _);
            var existing = lifetimePolicy?.GetValue();
            if (existing != null)
            {
                context.Existing = existing;
                context.BuildComplete = true;
            }
        }
    }
}
