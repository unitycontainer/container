// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using Microsoft.Practices.ObjectBuilder2;
using Unity;

namespace Microsoft.Practices.Unity
{
    /// <summary>
    /// A strategy that handles Hierarchical lifetimes across a set of parent/child
    /// containers.
    /// </summary>
    public class HierarchicalLifetimeStrategy : BuilderStrategy
    {
        /// <summary>
        /// Called during the chain of responsibility for a build operation. The
        /// PreBuildUp method is called when the chain is being executed in the
        /// forward direction.
        /// </summary>
        /// <param name="context">Context of the build operation.</param>
        public override void PreBuildUp(IBuilderContext context)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(context, "context");

            IPolicyList lifetimePolicySource;

            var activeLifetime = context.PersistentPolicies.Get<ILifetimePolicy>(context.BuildKey, out lifetimePolicySource);
            if (activeLifetime is HierarchicalLifetimeManager && !object.ReferenceEquals(lifetimePolicySource, context.PersistentPolicies))
            {
                // came from parent, add a new Hierarchical lifetime manager locally   
                var newLifetime = new HierarchicalLifetimeManager { InUse = true };
                context.PersistentPolicies.Set<ILifetimePolicy>(newLifetime, context.BuildKey);
                // Add to the lifetime container - we know this one is disposable
                context.Lifetime.Add(newLifetime);
            }
        }
    }
}
